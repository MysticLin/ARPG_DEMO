using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class SKillSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerMoveHandler
{
    public List<SKillSlot> prerequisiteSkillSlots;
    public SkillSO skillSO;
    public int currentLevel;
    public bool isUnlocked;
    public Image skillIcon;
    public Button skillButton;
    public TMP_Text skillLevelText;
    public SkillInfo skillInfo;
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
    public void RefreshUI()
    {
        if (skillSO == null) return;
        UpdateUI();
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
    public void RestoreState(int savedLevel,bool savedUnlocked)
    {
        if (skillSO == null) return;
        currentLevel = Mathf.Clamp(savedLevel, 0, skillSO.maxLevel);
        isUnlocked=savedUnlocked;
        UpdateUI();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skillInfo != null)
            skillInfo.Show(this);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (skillInfo != null)
            skillInfo.FollowMouse();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       if(skillInfo!=null)
            skillInfo.Hide();
    }
}
