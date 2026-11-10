using System.Collections;
using UnityEngine;

public class UseItem : MonoBehaviour
{
    public bool CanUse(ItemSO itemSO)
    {
        if (itemSO == null || StatsManager.Instance == null) return false;
        bool hasHeal = itemSO.currentHealth > 0;
        bool hasOtherEff=itemSO.maxHealth>0||itemSO.damage > 0||itemSO.speed > 0;
        if (hasHeal && !hasOtherEff && StatsManager.Instance.CurrentHealth >= StatsManager.Instance.MaxHealth) return false;
        return true;
    }
   public bool ApplyItemEffects(ItemSO itemSO)
    {
        if(!CanUse(itemSO))return false;
        if (itemSO.currentHealth > 0)
            StatsManager.Instance.UpdateHealth(itemSO.currentHealth);
        if(itemSO.maxHealth>0)
            StatsManager.Instance.UpdateMaxHealth(itemSO.maxHealth+StatsManager.Instance.MaxHealth);
        if(itemSO.speed>0)
            StatsManager.Instance.UpdateSpeed(itemSO.speed);
        if (itemSO.duration > 0)
            StartCoroutine(EffectTimer(itemSO, itemSO.duration));
        return true;
    }
    private IEnumerator EffectTimer(ItemSO itemSO,float duration)
    {
        yield return new WaitForSeconds(duration);
        if (itemSO.maxHealth > 0)
            StatsManager.Instance.AddMaxHealth(-itemSO.maxHealth);
        if (itemSO.speed > 0)
            StatsManager.Instance.UpdateSpeed(-itemSO.speed);

    }
}
