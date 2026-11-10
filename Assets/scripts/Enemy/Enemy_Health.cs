using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy_Health : MonoBehaviour
{
    public int expReward = 3;
    public static Action<int> OnMonsterDefeated;
    [FormerlySerializedAs("maxHealth")]
    [SerializeField] private int StartMaxHealth = 3;
    [SerializeField] private float DestoryTime = 1.5f;
    [SerializeField] private Animator anim;
    [SerializeField] private Enemy_Combat combat;
    [SerializeField] private Enemy_Move move;
    [SerializeField] private Collider2D col;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Enemy_Knockback knock;
    public Health health { get; private set; }
    public bool isDead=>health != null&&health.IsDead;
    private void Awake()
    {
        health = new Health(StartMaxHealth);
        health.OnDied += HandleDead;
    }
    private void OnDestroy()
    {
        if(health!=null)health.OnDied -= HandleDead;
    }
    public bool ChangeHealth(int amount)
    {
        if(health==null||isDead)return false;
        if (amount < 0)
        {
            if(anim!=null)anim.SetTrigger("isHurt");
            health.TakeDamage(-amount);
        }
        else if(amount>0)health.Heal(amount);
       return !health.IsDead;
    }
    public void HandleDead()
    {
        if(rb!=null)rb.velocity=Vector2.zero;
        if(combat!=null)combat.enabled = false;
        if(move!=null)move.enabled = false;
        if (knock != null)
        {
            knock.Cancel();
            knock.enabled = false;
        }
        if(col!=null)col.enabled = false;
        OnMonsterDefeated?.Invoke(expReward);
        Destroy(gameObject, DestoryTime);
    }
}
