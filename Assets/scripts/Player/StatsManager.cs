using UnityEngine;
using TMPro;
using System;
using System.Runtime.InteropServices;

public class StatsManager : MonoBehaviour
{
    //单例模式，数值管理器实例。
    public static StatsManager Instance;
    public StatsUI statsUI;

    [Header("武器属性")]
    public int damage;
    public float knockbackForce;
    public float weaponRange;
    public float knockbackTime;
    public float stunTime;
    [Header("移动属性")]
    public float speed;

    [Header("生命属性")]
    public int startHealth = 100;
    [Header("防御属性")]
    public int defense;
    public float damageReduction;
    public float dodgeChance;
    private int baseDamage;
    private float baseSpeed;
    private int baseDefense;
    private float baseDamageReduction;
    private float baseDodgeChance;
    private Health health { get;set; }
    public Health PlayerHealth => health;
    public int CurrentHealth => health != null ? health.CurrentHealth : 0;
    public int MaxHealth =>health != null ? health.MaxHealth : 0;
    public bool isDead=>health!=null&&health.IsDead;
    public event Action<int, int> OnChangeHealth;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (startHealth <= 0)
            startHealth = 100;
        health =new Health(startHealth);

        baseDamage = damage;
        baseSpeed = speed;
        baseDefense = defense;
        baseDamageReduction = damageReduction;
        baseDodgeChance = dodgeChance;
    }

    public void ResetSkillStats()
    {
        damage = baseDamage;
        speed = baseSpeed;
        defense = baseDefense;
        damageReduction = baseDamageReduction;
        dodgeChance = baseDodgeChance;
    }
    private void OnEnable()
    {
        if (health != null) health.OnChangeHealth += OnHealthChangedCallback;
    }
    private void OnDisable()
    {
        if (health != null)
        health.OnChangeHealth -= OnHealthChangedCallback;    
    }
    private void OnHealthChangedCallback(int cur,int max)
    {
        if(health!=null)
        OnChangeHealth?.Invoke(cur, max);
    }
    public void UpdateHealth(int amount)
    {
        if(amount>0)health.Heal(amount);
        else if(amount<0)health.TakeDamage(-amount);
    }
    public void UpdateMaxHealth(int newMax)
    {
        health.SetMaxAndDiff(newMax);
    }
    public void SetMaxKeepCurrent(int newMax)
    {
        health.SetMaxKeepCurrent(newMax);
    }
    public void AddMaxHealth(int amount)
    {
        if(health==null)return;
        health.SetMaxAndDiff(amount+health.MaxHealth);
    }
    public void RevivePlayer()
    {
        health.Revive();
    }
    public void RestoreHealth(int current,int max)
    {
        if (health == null)
            health = new Health(max);
        health.Restore(current, max);
    }
    public void KillSelf()
    {
        health.KillSelf();
    }
    public void UpdateSpeed(int amount)
    {
        speed += amount;
        if (statsUI != null)
            statsUI.UpdateAllStats();
    }
}
