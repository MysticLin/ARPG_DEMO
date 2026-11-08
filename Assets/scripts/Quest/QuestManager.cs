using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<QuestSO, Dictionary<QuestObjective, int>> questProgress = new();
    private List<QuestSO> completedQuests = new();
    public static QuestManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    private void OnEnable()
    {
        QuestEvents.IsQuestComplete += IsQuestComplete;   
    }
    private void OnDisable()
    {
        QuestEvents.IsQuestComplete -= IsQuestComplete;
    }
    #region 任务接受区域
    public bool IsQuestAccepted(QuestSO questSO)
    {
        return questProgress.ContainsKey(questSO);
    }
    public List<QuestSO> GetActiveQuests()
    {
        return new List<QuestSO>(questProgress.Keys); 
    }
    public void AcceptQuest(QuestSO questSO)
    {
        questProgress[questSO] = new Dictionary<QuestObjective, int>();
        foreach(var objective in questSO.objectives)
        {
            UpdateObjectiveProgress(questSO, objective);
        }
    }
    #endregion
    #region 任务完成区域
    public bool IsQuestComplete(QuestSO questSO)
    {
        if (!questProgress.TryGetValue(questSO, out var progressDict))
            return false;
        foreach(var objective in questSO.objectives)
        {
            UpdateObjectiveProgress(questSO, objective);
        }
        foreach(var objective in questSO.objectives)
        {
            if (progressDict[objective] < objective.requiredAmount)
                return false;
        }
        return true;
    }
    public void CompleteQuest(QuestSO questSO)
    {
        questProgress.Remove(questSO);
        completedQuests.Add(questSO);
        foreach(var objective in questSO.objectives)
        {
            if (objective.targetItem != null && objective.requiredAmount > 0)
            {
                InventoryManager.Instance.RemoveItem(objective.targetItem, objective.requiredAmount);
            }
        }
        foreach(var reward in questSO.rewards)
        {
            InventoryManager.Instance.AddItem(reward.itemSO,reward.quantity);
        }
    }
    public bool GetCompleteQuest(QuestSO questSO)
    {
        return completedQuests.Contains(questSO);
    }
    #endregion
    public void UpdateObjectiveProgress(QuestSO questSO,QuestObjective objective)
    {
        if (!questProgress.ContainsKey(questSO))
            return;
        var progressDictionary = questProgress[questSO];
        int newAmount = 0;
        if (objective.targetItem != null)
            newAmount = InventoryManager.Instance.GetItemQuantity(objective.targetItem);
        else if (objective.targetLocation != null && GameManager.Instance.LocatioHistroyTracker.HasVisited(objective.targetLocation))
        newAmount = objective.requiredAmount;
        else if(objective.targetNpc!=null&&GameManager.Instance.DialogueHistoryTracker.HasSpokenWith(objective.targetNpc))
            newAmount=objective.requiredAmount;
        progressDictionary[objective] = newAmount;
    }
    public string GetProgressText(QuestSO questSO,QuestObjective objective)
    {
        int currrentAmount = GetCurrentAmount(questSO, objective);
        if (currrentAmount >= objective.requiredAmount)
            return "Complete";
        else if (objective.targetItem != null)
            return $"{currrentAmount}/{objective.requiredAmount}";
        else
            return "In Progress";
    }
    public int GetCurrentAmount(QuestSO questSO,QuestObjective objctive)
    {
        if (questProgress.TryGetValue(questSO, out var objectiveDictionary))
            if(objectiveDictionary.TryGetValue(objctive,out int amount))
                return amount;
        return 0;
    }
}
