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
    [Header("限制任务重复提交(可选)")]
    public QuestSO forbiddenQuest;

    [Header("对话要求（可选择）")]
    public ActorSO[] requiredNPCs;
    public LocationSO[] requiredLocations;
    public ItemSO[] requiredItems;
    [Header("控制标志")]
    public bool removeAfterPlay;//播放后移除
    public List<DialogueSO> removeTheseOnplay;//播放后移除这些

    public bool IsConditionMet()
    {
        if (GameManager.Instance == null) return false;
        if (requiredNPCs!= null&&requiredNPCs.Length > 0 )
        {
            if(GameManager.Instance.DialogueHistoryTracker==null)return false;
            foreach(var npc in requiredNPCs)
            {
                if (npc == null) continue;
                if(!GameManager.Instance.DialogueHistoryTracker.HasSpokenWith(npc))
                    return false;
            }
        }
        if (requiredLocations!=null&&requiredLocations.Length > 0)
        {
            if (GameManager.Instance.LocatioHistroyTracker == null) return false;
            foreach (var location in requiredLocations)
            {
                if (location == null) continue;
                if(!GameManager.Instance.LocatioHistroyTracker.HasVisited(location))
                    return false;
            }
        }
        if (requiredItems != null&&requiredItems.Length > 0)
        {
            if (InventoryManager.Instance == null) return false;
            foreach(var item in requiredItems)
            {
                if (item == null) continue;
                if(!InventoryManager.Instance.HasItem(item))
                    return false;
            }
        }
        if (requiredCompletedQuests != null && requiredCompletedQuests.Length > 0)
        {
            if (QuestManager.Instance == null) return false;
            foreach (var quest in requiredCompletedQuests)
            {
                if(quest==null)continue;
                if (!QuestManager.Instance.GetCompleteQuest(quest))
                    return false;
            }
        }
        if (forbiddenQuest != null && QuestManager.Instance != null && (QuestManager.Instance.IsQuestAccepted(forbiddenQuest) ||
            QuestManager.Instance.GetCompleteQuest(forbiddenQuest)))
            return false;
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
}
