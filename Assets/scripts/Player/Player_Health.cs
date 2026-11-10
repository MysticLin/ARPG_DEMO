using System;
using UnityEngine;

public class Player_Health : MonoBehaviour
{
    public Animator HealthAnim;  
    public Health health;
    public float InvincibleTime=1;
    private float InvincibleTimeUntil;
    [SerializeField] private Player_Move move;
    [SerializeField] private Player_Combat combat;
    [SerializeField] private Player_Bow bow;
    [SerializeField] private Collider2D col;
    [SerializeField] private Rigidbody2D rb;
    private void OnEnable() => Bind();
    private void Start()=>Bind();
    private void Bind()
    {
        if (StatsManager.Instance == null) return;
        Health newhealth = StatsManager.Instance.PlayerHealth;
        if (newhealth == null) return;

        if (health != null) health.OnDied-=PlayDead;
        health = newhealth;
        health.OnDied -= PlayDead;
        health.OnDied += PlayDead;  
    }
    private void OnDisable()
    {
        if(health!=null)
        health.OnDied -= PlayDead;
    }
    public bool TryTakeDamage(int amount)
    {
        if (amount <= 0 || health == null || StatsManager.Instance == null) return false;
        if (StatsManager.Instance.isDead||Time.time<InvincibleTimeUntil)
            return false;
        StatsManager.Instance.UpdateHealth(-amount);
        if(HealthAnim!=null)HealthAnim.Play("HealthChange");
        InvincibleTimeUntil = Math.Max(Time.time + InvincibleTime,InvincibleTimeUntil);
        return !health.IsDead;
    }
    public void PlayDead()
    {
        if (health==null) return;
        ResetSet();
        SetControl(false);
        if(rb!=null)rb.velocity = Vector2.zero;
    }
    public void PlayRevive()
    {
        if(health==null)return;
        health.Revive();
        if(rb!=null)rb.velocity=Vector2.zero;
        SetControl(true);
        ResetSet();
        InvincibleTimeUntil = Time.time + InvincibleTime;
    }
    private void SetControl(bool isEnable)
    {
        if(move!=null) move.enabled = isEnable;
        if(bow!=null) bow.enabled = isEnable;
        if(col!=null) col.enabled = isEnable;
        if(combat!=null) combat.enabled = isEnable;
    }
    private void ResetSet()
    {
        if (move != null) move.ResetState();
        if (combat != null) combat.ResetState();
        if (bow != null) bow.ResetState();
    }
}
