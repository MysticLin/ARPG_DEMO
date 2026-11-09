using UnityEngine;

[CreateAssetMenu(fileName ="NewSKill",menuName="SkillTree/Skill")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public int maxLevel;
    public Sprite skillIcon;
}
