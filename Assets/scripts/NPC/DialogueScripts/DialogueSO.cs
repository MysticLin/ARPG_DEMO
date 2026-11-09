using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="DialogueSO",menuName ="Dialogue/DialogueNode")]
public class DialogueSO : ScriptableObject
{
    public DialogueLine[] lines;
    public DialogueOption[] options;
    [Header("任务 提供（可选择）")]
    public QuestSO offerQuestOnEnd;
    [Header("已完成任务要求（可选择）")]
    public QuestSO[] requiredCompletedQuests;
    [Header("任务提交（可选择）")]
    public QuestSO turnInQuestOnEnd;

    [Header("对话要求（可选择）")]
    public ActorSO[] requiredNPCs;
    public LocationSO[] requiredLocations;
    public ItemSO[] requiredItems;
    [Header("控制标志")]
    public bool removeAfterPlay;//播放后移除
    public List<DialogueSO> removeTheseOnplay;//播放后移除这些

    public bool IsConditionMet()
    {
        if (requiredNPCs.Length > 0)
        {
            foreach(var npc in requiredNPCs)
            {
                if(!GameManager.Instance.DialogueHistoryTracker.HasSpokenWith(npc))
                    return false;
            }
        }
        if (requiredLocations.Length > 0)
        {
            foreach (var location in requiredLocations)
            {
                if(!GameManager.Instance.LocatioHistroyTracker.HasVisited(location))
                    return false;
            }
        }
        if (requiredItems.Length > 0)
        {
            foreach(var item in requiredItems)
            {
                if(!InventoryManager.Instance.HasItem(item))
                    return false;
            }
        }
        if (requiredCompletedQuests != null && requiredCompletedQuests.Length > 0)
        {
            foreach (var quest in requiredCompletedQuests)
            {
                if (!QuestManager.Instance.IsQuestComplete(quest))
                {
                    return false;
                }
            }
        }
        return true;
    }
}
[System.Serializable]
public class DialogueLine
{
    public ActorSO speaker;
    [TextArea(3,5)]public string text;
}
[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public DialogueSO nextDialogue;
    public QuestSO offerQuest;
}
