using System.Collections.Generic;
using UnityEngine;

public class NPC_Talk : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public  Animator interactAnim;
    public List<DialogueSO> conversations;
    public DialogueSO currentConversation;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim=GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        QuestEvents.OnQuestAccepted += OnQuestAccepted_RemoveOfferings;
    }
    private void OnDestroy()
    {
        QuestEvents.OnQuestAccepted -= OnQuestAccepted_RemoveOfferings;
    }
    private void OnEnable()
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        anim.Play("Idle");
        interactAnim.Play("Open");
    }
    private void OnDisable()
    {
        rb.isKinematic=false;
        interactAnim.Play("Close");
    }
    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.DialogueManager == null) return;
        if (PauseService.Instance != null && PauseService.Instance.IsPaused) return;
        if (Input.GetButtonDown("Interact"))
        {
            if (GameManager.Instance.DialogueManager.isDialogueActive)
                GameManager.Instance.DialogueManager.AdvanceDialogue();
            else
            {
                if (GameManager.Instance.DialogueManager.CanStartDialogue())
                {
                    CheckForNewConversation();
                    if (currentConversation == null) return;
                    GameManager.Instance.DialogueManager.StartDialogue(currentConversation);
                }
            }
        }
    }
    private void CheckForNewConversation()
    {
        currentConversation = null;
        if (conversations == null) return;
        for(int i = 0; i < conversations.Count; i++)
        {
            var convo = conversations[i];
            if (convo == null && !convo.IsConditionMet()) continue;
            currentConversation = convo;
            if(convo.removeAfterPlay)
                conversations.RemoveAt(i);
            if (convo.removeTheseOnplay != null && convo.removeTheseOnplay.Count > 0)
            {
                foreach(var toRemove in convo.removeTheseOnplay)
                    conversations.Remove(toRemove);
            }
            break;
        }
    }
    private void OnQuestAccepted_RemoveOfferings(QuestSO acceptedQuest)
    {
        for(int i = conversations.Count - 1; i >= 0; i--)
        {
            var convo = conversations[i];
            if(convo==null)
                continue;
            if(convo.offerQuestOnEnd==acceptedQuest)
                conversations.RemoveAt(i);
        }
    }
}
