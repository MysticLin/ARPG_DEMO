using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfo : MonoBehaviour
{
    public CanvasGroup infoPanel;
    public TMP_Text skillNameText;
    public TMP_Text descriptionText;
    public TMP_Text[] stateTexts;

    public Vector2 mouseOffset = new Vector2(12f, -12f);
    private RectTransform InfoPanelRect;
    private void Awake()
    {
        InfoPanelRect=GetComponent<RectTransform>();
        //提示框不能挡住技能按钮
        foreach (Graphic graphic in GetComponentsInChildren<Graphic>(true))
            graphic.raycastTarget = false;
        if (infoPanel != null)
        {
            infoPanel.alpha = 0f;
            infoPanel.blocksRaycasts = false;
            infoPanel.interactable = false;
        }
    }
    public void Show(SKillSlot slot)
    {
        if (slot == null || slot.skillSO == null || infoPanel == null)
            return;
        SkillSO skill = slot.skillSO;
        if (skillNameText != null)
            skillNameText.text = skill.skillName;
        if (descriptionText != null)
            descriptionText.text = BuildDescription(skill, slot);
        if (stateTexts != null)
        {
            SetStateText(0, "等级: " + (slot.isUnlocked ? slot.currentLevel + "/" + skill.maxLevel : "未解锁"));
            SetStateText(1, "当前: " + BuildEffect(skill, slot.currentLevel));
            SetStateText(2, "下一级 " + BuildEffect(skill, Mathf.Min(slot.currentLevel + 1, skill.maxLevel)));
            for(int i = 3; i < stateTexts.Length; i++)
            {
                if (stateTexts[i]!=null)
                    stateTexts[i].gameObject.SetActive(false);
            }
        }
        infoPanel.alpha = 1f;
        infoPanel.blocksRaycasts = false;
        transform.SetAsLastSibling();
        FollowMouse();
    }
    public void Hide()
    {
        if (infoPanel != null)
            infoPanel.alpha = 0f;
    }
    public void FollowMouse()
    {
        if (InfoPanelRect == null) return;
        Vector3 position = Input.mousePosition + (Vector3)mouseOffset;
        float width=InfoPanelRect.rect.width;
        float height=InfoPanelRect.rect.height;
        position.x = Mathf.Clamp(position.x, 0f, Mathf.Max(0f, Screen.width - width));
        position.y = Mathf.Clamp(position.y, height, Screen.height);
        InfoPanelRect.position=position;
    }
    private void SetStateText(int index,string text)
    {
        if (stateTexts == null || index < 0 || index >= stateTexts.Length || stateTexts[index] == null)
            return;
        stateTexts[index].gameObject.SetActive(true);
        stateTexts[index].text = text;
    }
    private string BuildDescription(SkillSO skill,SKillSlot slot)
    {
        if (skill.effectType == SkillEffectType.UnlockMelee)
            return "解锁近战攻击，之后才能使用近战武器。";

        if (!slot.isUnlocked)
            return "该技能尚未解锁，需要先解锁前置技能。";

        return "每点提升一级，效果可叠加。";
    }
    private string BuildEffect(SkillSO skill,int level)
    {
        if (level <= 0)
            return "无";
        float value = skill.valuePerLevel * level;
        switch (skill.effectType)
        {
            case SkillEffectType.UnlockMelee:
                return "解锁近战攻击";
            case SkillEffectType.AttackDamage:
                return "攻击力 +" + Mathf.RoundToInt(value);
            case SkillEffectType.MaxHealth:
                return "最大生命 +" + Mathf.RoundToInt(value);
            case SkillEffectType.Defense:
                return "防御 +" + Mathf.RoundToInt(value);
            case SkillEffectType.DamageReduction:
                return "免伤 +" + Mathf.RoundToInt(value * 100f) + "%";
            case SkillEffectType.MoveSpeed:
                return "移动速度 +" + value.ToString("0.##");
            case SkillEffectType.Dodge:
                return "闪避 +" + Mathf.RoundToInt(value * 100f) + "%";
            case SkillEffectType.Burn:
                return "命中附加燃烧：每秒 "
                    + Mathf.RoundToInt(value)
                    + " 点伤害，持续 "
                    + skill.duration.ToString("0.#")
                    + " 秒";
            case SkillEffectType.Slow:
                return "命中减速 "
                    + Mathf.RoundToInt(value * 100f)
                    + "%，持续 "
                    + skill.duration.ToString("0.#")
                    + " 秒";
            default:
                return "无";
        }
    }
    private void OnDisable()
    {
        Hide();
    }
}
