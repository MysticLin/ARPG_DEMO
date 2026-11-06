using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    public SKillSlot[] skillSlots;
    public TMP_Text pointsText;
    public int availabliePoints;
    private void OnEnable()
    {
        SKillSlot.OnAbilityPointSpent += HandleAbilityPointsSpent;
        SKillSlot.OnSkillMaxed += HandleSkillMaxed;
        ExpManager.OnLevelUp += UpdateAbilityPoints;
    }
    private void OnDisable()
    {
        SKillSlot.OnAbilityPointSpent -= HandleAbilityPointsSpent;
        SKillSlot.OnSkillMaxed -= HandleSkillMaxed;
        ExpManager.OnLevelUp -= UpdateAbilityPoints;
    }
    private void Start()
    {
        foreach(SKillSlot slot in skillSlots)
        {
            slot.skillButton.onClick.AddListener(()=>CheckAvailablePoints(slot));
        }
        UpdateAbilityPoints(0);
    }
    private void CheckAvailablePoints(SKillSlot slot)
    {
        if (availabliePoints > 0)
        {
            slot.TryUpgradeSkill();
        }
    }
    private void HandleAbilityPointsSpent(SKillSlot skillSlot)
    {
        if (availabliePoints > 0)
        {
            UpdateAbilityPoints(-1);
        }
    }
    private void HandleSkillMaxed(SKillSlot skillSlot)
    {
        foreach(SKillSlot slot in skillSlots)
        {
            if (!slot.isUnlocked && slot.CanUnlockSkill())
            {
                slot.Unlock();
            }
        }
    }
    public void UpdateAbilityPoints(int amount)
    {
        availabliePoints += amount;
        pointsText.text = "Points:" + availabliePoints;
    }
}
