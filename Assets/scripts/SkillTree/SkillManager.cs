using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public Player_Combat combat;
    private void OnEnable()
    {
        SKillSlot.OnAbilityPointSpent += HandleAbilityPointSpent;
    }
    private void OnDisable()
    {
        SKillSlot.OnAbilityPointSpent -= HandleAbilityPointSpent;
    }
    private void HandleAbilityPointSpent(SKillSlot Slot)
    {
        string skillName = Slot.skillSO.skillName;
        switch (skillName)
        {
            case "MaxHealthBoot":
                StatsManager.Instance.UpdateMaxHealth(1);
                break;
            case "Sword Slash":
                combat.enabled = true;
                break;
            default:
                break;
        }
    }
}
