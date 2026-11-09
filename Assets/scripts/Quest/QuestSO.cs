using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "QuestSO", menuName = "QuestSO")]
public class QuestSO : ScriptableObject
{
    public string questName;
    public int questLevel;
    [TextArea]public string questDirection;

    public List<QuestObjective> objectives;
    public List<QuestReward> rewards;
}
[System.Serializable]
public class QuestObjective
{
    public string description;
    [SerializeField] private Object target;
    public ItemSO targetItem=>target as ItemSO;
    public ActorSO targetNpc=>target as ActorSO;
    public LocationSO targetLocation => target as LocationSO;
    public int requiredAmount;

}
[System.Serializable]
public class QuestReward
{
    public ItemSO itemSO;
    public int quantity;
}
