using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestLogUI : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text questDescriptionText;
    [SerializeField] private QuestObjectiveSlot[] objectiveSlots;
    [SerializeField] private QuestRewardSlot[] rewardsSlots;

    private QuestSO questSO;
    public static bool IsOpen {  get; private set; }
    private bool pausedForQuestUI;
    [SerializeField] private QuestSO noAvailableQuestSO;
    [SerializeField] private QuestLogSlot[] questSlots;
    [SerializeField] private CanvasGroup questCanvas;
    [SerializeField] private CanvasGroup acceptCanvasGroup;
    [SerializeField] private CanvasGroup declineCanvasGroup;
    [SerializeField] private CanvasGroup completeCanvasGroup;
    private void OnEnable()
    {
        QuestEvents.OnQuestOfferRequested += ShowQuestOffer;
        QuestEvents.OnQuestTurnInRequested += ShowQuestTurnIn;
    }
    private void OnDisable()
    {
        IsOpen = false;
        QuestEvents.OnQuestOfferRequested -= ShowQuestOffer;
        QuestEvents.OnQuestTurnInRequested -= ShowQuestTurnIn;
        if (pausedForQuestUI)
        {
            PauseService.Instance?.Pop();
            pausedForQuestUI = false;
        }
    }
    #region 展示任务的方法
    public void ShowQuestOffer(QuestSO incomingQuestSO)
    {
        if (questManager.IsQuestAccepted(incomingQuestSO)||questManager.GetCompleteQuest(incomingQuestSO))
        {
            questSO = noAvailableQuestSO;
            SetCanvasState(acceptCanvasGroup, false);
            SetCanvasState(declineCanvasGroup, true);
            SetCanvasState(completeCanvasGroup, false);
        }
        else
        {
            questSO=incomingQuestSO;
            SetCanvasState(acceptCanvasGroup, true);
            SetCanvasState(declineCanvasGroup, true);
            SetCanvasState(completeCanvasGroup, false);
        }
        HandleQuestClicked(questSO);
        SetCanvasState(questCanvas, true);
    }

    public void ShowQuestTurnIn(QuestSO incomingQuestSO)
    {
        questSO=incomingQuestSO;
        HandleQuestClicked(questSO);
        SetCanvasState(completeCanvasGroup,true);
        SetCanvasState(acceptCanvasGroup,false);
        SetCanvasState (declineCanvasGroup,false);
        SetCanvasState(questCanvas,true);
    }
    #endregion
    #region 存放按钮点击的方法
    public void OnAcceptQuestClicked()
    {
        if (questSO == null) return;
        QuestEvents.OnQuestAccepted?.Invoke(questSO);
        questManager.AcceptQuest(questSO);
        RefreshQuestList();
        ShowQuestOffer(questSO);
    }
    public void OnDeclineQuestClicked()
    {
        SetCanvasState (questCanvas, false);
    }
    public void OnCompleteQuestClicked()
    {
        if (questSO == null) return;
        questManager.CompleteQuest(questSO);
        RefreshQuestList();
        ShowQuestOffer(questSO);
    }
    #endregion
    private void SetCanvasState(CanvasGroup group,bool activate)
    {
        if(group==null)return;
        group.alpha = activate ? 1 : 0;
        group.blocksRaycasts = activate;
        group.interactable = activate;
        if (group == questCanvas)
        {
            if (activate && !IsOpen)
            {
                PauseService.Instance?.Push();
                pausedForQuestUI = true;
            }
            else if (!activate && IsOpen)
            {
                PauseService.Instance?.Pop();
                pausedForQuestUI = false;
            }
            IsOpen = activate;
        }
    }
    public void RefreshQuestList()
    {
        List<QuestSO> activeQuests = questManager.GetActiveQuests();
        for (int i = 0; i < questSlots.Length; i++)
        {
            if (i < activeQuests.Count)
            {
                questSlots[i].SetQuest(activeQuests[i]);
            }
            else
            {
                questSlots[i].ClearSlot();
            }
        }
    }
   public void HandleQuestClicked(QuestSO questSO)
    {
        if (questSO == null)
        {
            questNameText.text = "";
            questDescriptionText.text = "";
            return;
        }
        this.questSO = questSO;
        questNameText.text = questSO.questName;
        questDescriptionText.text = questSO.questDirection;
        DisplayObjective();
        DisplayRewards();
    }

    private void DisplayObjective()
    {
        for(int i = 0; i < objectiveSlots.Length; i++)
        {
            if (i < questSO.objectives.Count)
            {
                var objective=questSO.objectives[i];
                questManager.UpdateObjectiveProgress(questSO, objective);
                int currentAmount = questManager.GetCurrentAmount(questSO, objective);
                string progress = questManager.GetProgressText(questSO, objective);
                bool isComplete=currentAmount>=objective.requiredAmount;
                objectiveSlots[i].gameObject.SetActive(true);
                objectiveSlots[i].RefreshObjectives(objective.description, progress, isComplete);
            }
            else
            {
                objectiveSlots[i].gameObject.SetActive(false);
            }
        }
    }
    private void DisplayRewards()
    {
        for(int i = 0; i < rewardsSlots.Length; i++)
        {
            if (i < questSO.rewards.Count)
            {
                var reward=questSO.rewards[i];
                rewardsSlots[i].DisplayReward(reward.itemSO.icon,reward.quantity);
                rewardsSlots[i].gameObject.SetActive(true);

            }
            else
            {
                rewardsSlots[i].gameObject.SetActive(false);
            }
        }
    }

}
