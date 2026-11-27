using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public Player_Combat combat;
    public static SkillManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (combat == null)
            combat = FindObjectOfType<Player_Combat>(true);
    }
    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void OnEnable()
    {
        SKillSlot.OnAbilityPointSpent += HandleAbilityPointSpent;
    }

    private void OnDisable()
    {
        SKillSlot.OnAbilityPointSpent -= HandleAbilityPointSpent;
    }

    private void Start()
    {
        ApplyRestoredSkills();
    }

    private void HandleAbilityPointSpent(SKillSlot slot)
    {
        if (slot == null || slot.skillSO == null) return;

        ApplyEffect(slot, true,1);
    }

    // 读档时调用：只重算技能提供的属性，不重放最大生命
    public void ApplyRestoredSkills()
    {
        if (combat == null)
            combat = FindObjectOfType<Player_Combat>(true);

        StatsManager stats = StatsManager.Instance;
        if (stats != null)
            stats.ResetSkillStats();

        SKillSlot[] slots = FindObjectsOfType<SKillSlot>(true);
        bool swordUnlocked = false;

        foreach (var slot in slots)
        {
            if (slot == null || slot.skillSO == null || !slot.isUnlocked)
                continue;

            if (slot.skillSO.effectType == SkillEffectType.UnlockMelee)
            {
                if (slot.skillSO.skillName == "Sword Slash"&&slot.currentLevel>0)
                    swordUnlocked = true;
                continue;
            }

            // 最大生命已经由 SaveManager.RestoreHealth 恢复，不能重复加
            if (slot.skillSO.effectType == SkillEffectType.MaxHealth)
                continue;

            ApplyEffect(slot, false,slot.currentLevel);
        }

        if (combat != null)
        {
            combat.unlocked = swordUnlocked;
            combat.enabled = swordUnlocked;
        }
    }

    // 命中时调用：火焰持续伤害 / 冰减速
    public void ApplyOnHit(Enemy_StatusEffects status)
    {
        if (status == null) return;

        SKillSlot[] slots = FindObjectsOfType<SKillSlot>(true);

        foreach (var slot in slots)
        {
            if (slot == null || slot.skillSO == null ||
                !slot.isUnlocked || slot.currentLevel <= 0)
                continue;

            SkillSO skill = slot.skillSO;
            float value = Mathf.Max(0f, skill.valuePerLevel);
            float duration = skill.duration > 0f ? skill.duration : 2f;

            switch (skill.effectType)
            {
                case SkillEffectType.Burn:
                    int burnDamage = Mathf.Max(
                        1,
                        Mathf.RoundToInt(slot.currentLevel * value));

                    status.ApplyBurn(burnDamage, duration);
                    break;

                case SkillEffectType.Slow:
                    float multiplier = Mathf.Clamp(
                        1f - slot.currentLevel * value,
                        0.2f,
                        1f);
                    status.ApplySlow(multiplier, duration);
                    break;
            }
        }
    }

    private void ApplyEffect(SKillSlot slot, bool applyMaxHealth,int levelCount)
    {
        if (slot == null || slot.skillSO == null) return;

        SkillSO skill = slot.skillSO;
        StatsManager stats = StatsManager.Instance;

        if (stats == null) return;
        int levels = Mathf.Max(0, levelCount);
        float totalvalue = Mathf.Max(0f, skill.valuePerLevel)*levels;
        int totalintValue = Mathf.RoundToInt(totalvalue);

        switch (skill.effectType)
        {
            case SkillEffectType.UnlockMelee:
                if (combat == null)
                    combat = FindObjectOfType<Player_Combat>(true);

                if (combat != null && skill.skillName == "Sword Slash")
                {
                    combat.unlocked = true;
                    combat.enabled = true;
                }
                break;

            case SkillEffectType.AttackDamage:
                stats.damage += totalintValue;
                break;

            case SkillEffectType.MaxHealth:
                if (applyMaxHealth)
                    stats.AddMaxHealth(totalintValue);
                break;

            case SkillEffectType.Defense:
                stats.defense += totalintValue;
                break;

            case SkillEffectType.DamageReduction:
                stats.damageReduction = Mathf.Clamp(
                    stats.damageReduction + totalintValue,
                    0f,
                    0.8f);
                break;

            case SkillEffectType.MoveSpeed:
                stats.speed += totalintValue;
                break;

            case SkillEffectType.Dodge:
                stats.dodgeChance = Mathf.Clamp(
                    stats.dodgeChance + totalintValue,
                    0f,
                    0.75f);
                break;
        }
    }
}
