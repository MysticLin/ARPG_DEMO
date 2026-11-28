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
    [Header("窗口")]
    [SerializeField] private CanvasGroup questCanvas;
    [SerializeField] private UIWindow questWindow;

    [SerializeField] private QuestSO noAvailableQuestSO;
    [SerializeField] private QuestLogSlot[] questSlots;

    [SerializeField] private CanvasGroup acceptCanvasGroup;
    [SerializeField] private CanvasGroup declineCanvasGroup;
    [SerializeField] private CanvasGroup completeCanvasGroup;
    private QuestSO questSO;
    public static bool IsOpen { get; private set; }
    private void Awake()
    {
        if (questWindow == null && questCanvas != null)
        {
            questWindow = questCanvas.GetComponent<UIWindow>();
            if(questWindow==null)
                questWindow= gameObject.gameObject.AddComponent<UIWindow>();
        }
        if (questWindow != null)
        {
            questWindow.OnShown += HandleWindowShown;
            questWindow.OnHidden += HandleWindowHidden;
        }
    }
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
        if (questWindow != null)
        {
            questWindow.OnShown -= HandleWindowShown;
            questWindow.OnHidden -= HandleWindowHidden;
            if (questWindow.IsOpen)
                UIWindowManager.Instance?.Close(questWindow);
        }
    }
    private void HandleWindowShown()
    {
        IsOpen = true;
        if(questManager!=null)
            RefreshQuestList();
    }
    private void HandleWindowHidden()
    {
        IsOpen=false;
    }
    #region 展示任务
    public void ShowQuestOffer(QuestSO incomingQuestSO)
    {
        if (questManager == null || incomingQuestSO == null) return;
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
        OpenQuestLog();
    }

    public void ShowQuestTurnIn(QuestSO incomingQuestSO)
    {
        if (questManager == null || incomingQuestSO == null) return;
        questSO=incomingQuestSO;
        HandleQuestClicked(questSO);
        SetCanvasState(completeCanvasGroup,true);
        SetCanvasState(acceptCanvasGroup,false);
        SetCanvasState (declineCanvasGroup,false);
        OpenQuestLog();
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
       CloseQuestLog();
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
    }
    public void RefreshQuestList()
    {
        if (questManager == null || questSlots == null) return;
        List<QuestSO> activeQuests = questManager.GetActiveQuests();
        for (int i = 0; i < questSlots.Length; i++)
        {
            if (i < activeQuests.Count)
                questSlots[i].SetQuest(activeQuests[i]);
            else
                questSlots[i].ClearSlot();
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
        bool canTurnIn = questManager.IsQuestComplete(questSO);
        SetCanvasState(completeCanvasGroup, canTurnIn);
        if (canTurnIn)
        {
            SetCanvasState(acceptCanvasGroup, false);
            SetCanvasState(declineCanvasGroup, false);
        }
    }

    private void DisplayObjective()
    {
        if (questSO == null || objectiveSlots == null) return;
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
        if (questSO == null || rewardsSlots == null) return;
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
    public void ToggleQuestLog()
    {
        if (questWindow == null) return;
        UIWindowManager.Instance?.Toggle(questWindow);
    }
    public void OpenQuestLog()
    {
        if (questWindow == null) return;
        UIWindowManager.Instance?.Open(questWindow);
    }
    public void CloseQuestLog()
    {
        if (questWindow == null) return;
        UIWindowManager.Instance?.Close(questWindow);
    }
}
