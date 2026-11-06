using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SKillSlot : MonoBehaviour
{
    public List<SKillSlot> prerequisiteSkillSlots;
    public SkillSO skillSO;
    public int currentLevel;
    public bool isUnlocked;
    public Image skillIcon;
    public Button skillButton;
    public TMP_Text skillLevelText;

    public static event Action<SKillSlot> OnAbilityPointSpent;
    public static event Action<SKillSlot> OnSkillMaxed;

    private void OnValidate()
    {
        if (skillSO != null&&skillLevelText!=null)
        {
            UpdateUI();
        }
    }
    public void TryUpgradeSkill()
    {
        if (isUnlocked && currentLevel < skillSO.maxLevel)
        {
            currentLevel++;
            OnAbilityPointSpent?.Invoke(this);
            if (currentLevel >= skillSO.maxLevel)
            {
                OnSkillMaxed?.Invoke(this);
            }
            UpdateUI();
        }
    }
    public bool CanUnlockSkill()
    {
        foreach(SKillSlot slot in prerequisiteSkillSlots)
        {
            if (!slot.isUnlocked||slot.currentLevel<slot.skillSO.maxLevel)
            {
                return false;
            }
        }
        return true;
    }
    public void Unlock()
    {
        isUnlocked=true;
        UpdateUI();
    }
    private void UpdateUI()
    {
        skillIcon.sprite = skillSO.skillIcon;
        if (isUnlocked)
        {
            skillButton.interactable = true;
            skillLevelText.text=currentLevel.ToString()+"/"+skillSO.maxLevel.ToString();
            skillIcon.color = Color.white;
        }
        else
        {
            skillButton.interactable = false;
            skillLevelText.text = "Locked";
            skillIcon.color=Color.black;
        }
    }
}
