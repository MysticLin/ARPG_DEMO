using UnityEngine;

[CreateAssetMenu(fileName = "NewSKill", menuName = "SkillTree/Skill")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public string SkillId;
    public int maxLevel;
    public Sprite skillIcon;
    public string Id => string.IsNullOrEmpty(SkillId) ? name : SkillId;
    [Header("效果")]
    public SkillEffectType effectType;
    public float valuePerLevel;
    public float duration;
}
public enum SkillEffectType
{
    UnlockMelee,
    AttackDamage,
    MaxHealth,
    Defense,
    DamageReduction,
    Burn,
    Slow,
    MoveSpeed,
    Dodge
}

