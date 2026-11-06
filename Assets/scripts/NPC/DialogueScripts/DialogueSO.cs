using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
[CreateAssetMenu(fileName ="DialogueSO",menuName ="Dialogue/DialogueNode")]
public class DialogueSO : ScriptableObject
{
    public DialogueLine[] lines;
    public DialogueOption[] options;
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
