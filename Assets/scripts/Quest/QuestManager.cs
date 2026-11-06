using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<QuestSO, Dictionary<QuestObjective, int>> questProgress = new();
    public static QuestManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
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
    public void UpdateObjectiveProgress(QuestSO questSO,QuestObjective objective)
    {
        if (!questProgress.ContainsKey(questSO))
            questProgress[questSO] = new Dictionary<QuestObjective, int>();
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
