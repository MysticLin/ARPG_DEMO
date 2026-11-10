using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public Animator anim;
    public float cooldown = 2;
    private float timer;
    private void Update()
    {
        if (timer > 0)
        {
            timer-=Time.deltaTime;
        }
    }
    public void Attack()
    {
        if (timer<=0)
        {
            anim.SetBool("isAttacking", true);
            timer = cooldown;
        }
       
    }
    public void DealDamage()
    {
        if(StatsManager.Instance==null)return;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, StatsManager.Instance.weaponRange, enemyLayer);
        if (enemies.Length == 0)return;
        if (!enemies[0].TryGetComponent(out Enemy_Health enemyHealth))return;
        if (!enemyHealth.ChangeHealth(-StatsManager.Instance.damage)) return;
        if (enemies[0].TryGetComponent(out Enemy_Knockback enemyKnockback))
        {
            enemies[0].GetComponent<Enemy_Knockback>().Knockback(transform, StatsManager.Instance.knockbackForce, StatsManager.Instance.stunTime, StatsManager.Instance.knockbackTime);
        }    
    }
    public void FinishAttacking()
    {
        anim.SetBool("isAttacking",false);
    }
    public void ResetState()
    {
        timer = 0;
        if (anim != null) anim.SetBool("isAttacking", false);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, StatsManager.Instance.weaponRange);
    }
}
