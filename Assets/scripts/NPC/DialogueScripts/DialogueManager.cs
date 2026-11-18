using System.Data.SqlTypes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    [Header("UI引用")]
    public CanvasGroup canvasGroup;
    public Image portrait;
    public TMP_Text actorName;
    public TMP_Text dialogueText;
    public Button[] choiceButtons;

    public bool isDialogueActive;
    private bool waitingForEnd;
    private bool showingChoices;
    private float endReadyTime;
    public static bool IsDialogueActive { get;private set; }
    private  DialogueSO currentDialogue;
    private int dialogueIndex;
    private float lastDialogueEndTime;
    private float dialogueCooldown = 0.1f;
    private void Awake()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        foreach (var button in choiceButtons)
        button.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (!IsDialogueActive) return;
        if (waitingForEnd)
        {
            if (Time.unscaledTime < endReadyTime) return;
            if (Input.anyKeyDown) EndDialogue();
            return;
        }
       if(Input.GetButtonDown("Interact"))
            AdvanceDialogue();
    }
    public bool CanStartDialogue()
    {
        return Time.unscaledTime - lastDialogueEndTime > dialogueCooldown;
    }
    public void StartDialogue(DialogueSO dialogueSO)
    {
        showingChoices = false;
        waitingForEnd = false;
        endReadyTime = 0f;
        currentDialogue = dialogueSO;
        dialogueIndex = 0;
        isDialogueActive = true;
        IsDialogueActive = true;
        ShowDialogue();
    }
    public void AdvanceDialogue()
    {
        if (currentDialogue == null || waitingForEnd||showingChoices) return;
        if (dialogueIndex < currentDialogue.lines.Length)
            ShowDialogue();
        else
            ShowChoice();
    }
    private void ShowDialogue()
    {
        DialogueLine line =currentDialogue.lines[dialogueIndex];
        GameManager.Instance.DialogueHistoryTracker.RecordNPC(line.speaker);
        portrait.sprite = line.speaker.portrait;
        actorName.text = line.speaker.actorName;
        dialogueText.text = line.text;
        dialogueIndex++;
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }
    private void ShowChoice()
    {
        ClearChoices();
        showingChoices = true;
        if (currentDialogue.options.Length > 0&&currentDialogue.options!=null)
        {
            for(int i = 0; i < currentDialogue.options.Length; i++)
            {
                var option= currentDialogue.options[i];
                choiceButtons[i].GetComponentInChildren<TMP_Text>().text =option.optionText;
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].onClick.AddListener(() =>ChooseOption(option.nextDialogue));
            }
            EventSystem.current.SetSelectedGameObject(choiceButtons[0].gameObject);
            return;
        }
            if (currentDialogue.turnInQuestOnEnd != null &&QuestManager.Instance.IsQuestComplete(currentDialogue.turnInQuestOnEnd))
            {
                QuestEvents.OnQuestTurnInRequested?.Invoke(currentDialogue.turnInQuestOnEnd);
                EndDialogue();
            return;
            }
            if (currentDialogue.offerQuestOnEnd != null)
            {
                EndDialogue();
                QuestEvents.OnQuestOfferRequested?.Invoke(currentDialogue.offerQuestOnEnd);
            return;
            }
        waitingForEnd = true;
        endReadyTime = Time.unscaledTime + 0.1f;
            if(dialogueText!=null&&currentDialogue.lines.Length>0)
            {
            dialogueText.text = currentDialogue.lines[currentDialogue.lines.Length - 1].text + "\n\n<Press any key to continue>";
            }
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }
    private void ChooseOption(DialogueSO dialogueSO)
    {
        if (dialogueSO == null)
            EndDialogue();
        else
        {
            ClearChoices();
            StartDialogue(dialogueSO);
        }
    }
    private void EndDialogue()
    {
        showingChoices = false;
        waitingForEnd = false;
        endReadyTime = 0f;
        dialogueIndex = 0;
        isDialogueActive=false;
        IsDialogueActive =false;
        ClearChoices();

        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        lastDialogueEndTime=Time.unscaledTime;
    }
    private void ClearChoices()
    {
        foreach(var button in choiceButtons)
        {
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }
    }
}
