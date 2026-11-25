using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public Animator anim;
    public float cooldown = 2;
    public bool unlocked;
    [SerializeField] private Player_Move playerMove;
    private float attackPointBaseX;
    private float timer;
    private void Awake()
    {
        if (playerMove == null)
            playerMove = GetComponent<Player_Move>();
        if(!unlocked)
            enabled=false;
        if (attackPoint != null)
            attackPointBaseX = Mathf.Abs(attackPoint.localPosition.x);
    }
    private void Update()
    {
        if (timer > 0)
        {
            timer-=Time.deltaTime;
        }
    }
    public void Attack()
    {
        if (PauseService.Instance != null && PauseService.Instance.IsPaused) return;
        if (DialogueManager.IsDialogueActive) return;
        if (StatsManager.Instance == null || StatsManager.Instance.isDead) return;
        if (timer<=0)
        {
            anim.SetBool("isAttacking", true);
            timer = cooldown;
        }
       
    }
    public void DealDamage()
    {
        if (PauseService.Instance != null && PauseService.Instance.IsPaused) return;
        if (DialogueManager.IsDialogueActive) return;
        if (StatsManager.Instance == null || StatsManager.Instance.isDead) return;
        if (StatsManager.Instance==null)return;
        if (playerMove != null && attackPoint != null)
        {
            Vector3 point = attackPoint.localPosition;
            point.x = attackPointBaseX * playerMove.facingDirection;
            attackPoint.localPosition = point;
        }
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, StatsManager.Instance.weaponRange, enemyLayer);
        if (enemies.Length == 0)return;
        if (!enemies[0].TryGetComponent(out Enemy_Health enemyHealth))return;
        if (!enemyHealth.ChangeHealth(-StatsManager.Instance.damage)) return;
        if (enemies[0].TryGetComponent(out Enemy_Knockback enemyKnockback))
        {
            enemies[0].GetComponent<Enemy_Knockback>().Knockback(transform, StatsManager.Instance.knockbackForce, StatsManager.Instance.stunTime, StatsManager.Instance.knockbackTime);
        }
        Enemy_StatusEffects status = enemies[0].GetComponent<Enemy_StatusEffects>();
        if (status != null)
            SkillManager.Instance?.ApplyOnHit(status);
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
        if (attackPoint == null) return;
        float range = StatsManager.Instance != null ? StatsManager.Instance.weaponRange : 2f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, range);
    }
}
