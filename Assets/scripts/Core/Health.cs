using System;

public class Health
{
    public event Action<int,int> OnChangeHealth;
    public event Action OnDied;
    public int MaxHealth {  get; private set; }
    public int CurrentHealth {  get; private set; }
    public bool IsDead {  get; private set; }
    public Health(int maxHp)
    {
        MaxHealth=Math.Max(maxHp,1);
        CurrentHealth=MaxHealth;
        IsDead = false;
    }
    #region 造成伤害
    public int TakeDamage(int damage)
    {
        if (IsDead||damage<=0) return 0;
        int oldHp=CurrentHealth;
        CurrentHealth -= damage;
        CurrentHealth=Math.Clamp(CurrentHealth,0,MaxHealth);//限制在0~MaxHealth这个区间
        int actualDamage=oldHp-CurrentHealth;
        OnChangeHealth?.Invoke(CurrentHealth,MaxHealth);
        if (CurrentHealth <= 0)Die();
        return actualDamage;
    }
    #endregion
    #region 恢复血量
    public int Heal(int amount)
    {
        if(IsDead||amount<=0)return 0;
        int oldHp=CurrentHealth;
        CurrentHealth += amount;
        CurrentHealth = Math.Clamp(CurrentHealth, 0, MaxHealth);
        int actualHeal=CurrentHealth-oldHp;
        if (actualHeal == 0) return 0;
        OnChangeHealth?.Invoke(CurrentHealth,MaxHealth);
        return actualHeal;
    }
    #endregion
    #region 修改最大血量(不补差值)
    public void SetMaxKeepCurrent(int newMax)
    {
        newMax=Math.Max(newMax,1);
        if (newMax == MaxHealth) return;
        MaxHealth = newMax;
        CurrentHealth = Math.Clamp(CurrentHealth, 0, MaxHealth);
        OnChangeHealth?.Invoke(CurrentHealth,MaxHealth);
    }
    #endregion
    #region 修改最大血量(补差值)
    public void SetMaxAndDiff(int newMax)
    {
        newMax = Math.Max(newMax, 1);
        if (newMax == MaxHealth) return;
        int oldMax = MaxHealth;
        MaxHealth = newMax;
        int diff=MaxHealth-oldMax;
        if (diff > 0&&!IsDead) CurrentHealth += diff;
        CurrentHealth=Math.Clamp(CurrentHealth, 0, MaxHealth);
        OnChangeHealth?.Invoke(CurrentHealth,MaxHealth);
    }
    #endregion
    #region 复活:解除死亡，恢复满血
    public void Revive()
    {
        if (!IsDead) return;
        IsDead = false;
        CurrentHealth = MaxHealth;
        OnChangeHealth?.Invoke(CurrentHealth,MaxHealth);
    }
    #endregion
    //死亡
    public void Die()
    {
        if (IsDead) return;
        IsDead=true;
        OnDied?.Invoke();
    }
    //强制死亡
    public void KillSelf()
    {
        if(!IsDead) return;
        if (CurrentHealth != 0)
        {
            CurrentHealth = 0;
            OnChangeHealth?.Invoke(CurrentHealth,MaxHealth);
        }
        Die();
    }
}
