using UnityEngine;

[CreateAssetMenu(fileName ="NewSKill",menuName="SkillTree/Skill")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public string SkillId;
    public int maxLevel;
    public Sprite skillIcon;
    public string Id => string.IsNullOrEmpty(SkillId) ? name : SkillId;
}
