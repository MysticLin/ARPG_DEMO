using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class SkillTreeManager : MonoBehaviour
{
    public SKillSlot[] skillSlots;
    public TMP_Text pointsText;
    public int availabliePoints;
    public SkillInfo skillInfo;
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
        if(skillInfo!=null)
            skillInfo.Hide();
    }
    private void Start()
    {
        RefreshSkillInfo();
       
        foreach(SKillSlot slot in skillSlots)
        {
            if(slot==null||slot.skillButton==null)continue;
            slot.skillInfo=skillInfo;
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
        if(pointsText!=null)
        pointsText.text = "Points:" + availabliePoints;
    }
    public List<SkillSaveEntry> ExportSkills()
    {
        List<SkillSaveEntry> result = new();
        foreach(SKillSlot slot in skillSlots)
        {
            if(slot==null||slot.skillSO==null)
                continue;
            result.Add(new SkillSaveEntry
            {
                skillId = slot.skillSO.Id,
                level = slot.currentLevel,
                unlocked = slot.isUnlocked
            });
        }
        return result;
    }
    public void RestoreProgression(int savedPoints,List<SkillSaveEntry> savedSkills)
    {
        availabliePoints = Mathf.Max(0, savedPoints);
        if (pointsText != null)
            pointsText.text = "Points:" + availabliePoints;
        if (savedSkills != null)
        {
            Dictionary<string, SkillSaveEntry> map = new();
            foreach(SkillSaveEntry entry in savedSkills)
            {
                if(entry==null||string.IsNullOrEmpty(entry.skillId))
                    continue;
                map[entry.skillId] = entry;
            }
            foreach(SKillSlot slot in skillSlots)
            {
                if(slot==null||slot.skillSO==null)
                    continue;
                if(map.TryGetValue(slot.skillSO.Id,out var entry))
                {
                    slot.RestoreState(entry.level, entry.unlocked);
                }
            }
        }
        SkillManager skillManager = FindObjectOfType<SkillManager>(true);
        if (skillManager != null)
            skillManager.ApplyRestoredSkills();
    }
    public void RefreshSkillInfo()
    {
        if (skillInfo == null)
            skillInfo = FindObjectOfType<SkillInfo>(true);
        if (skillSlots == null)
            return;
        foreach(SKillSlot slot in skillSlots)
        {
            if(slot!=null)
                slot.skillInfo=skillInfo;
        }
    }
}
