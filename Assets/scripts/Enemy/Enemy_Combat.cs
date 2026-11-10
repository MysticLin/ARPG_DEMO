using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    public int damage = 1;
    public Transform attackPoint;
    public float weaponRange;
    public float knockbackForce;
    public float stunTime;
    public LayerMask playerLayer;
    public void Attack()
    {
        if (!enabled) return;
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);
       if(hits.Length==0)return;
        if (!hits[0].TryGetComponent<Player_Health>(out var player_Health))return;
        if(!player_Health.TryTakeDamage(damage))return;
        if (hits[0].TryGetComponent<Player_Move>(out var player_Move) && player_Move.enabled)
            player_Move.Knockback(transform, knockbackForce, stunTime);
    }
}
