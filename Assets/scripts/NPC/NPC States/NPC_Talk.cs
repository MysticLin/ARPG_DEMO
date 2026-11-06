using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

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
        if (Input.GetButtonDown("Interact"))
        {
            if (GameManager.Instance.DialogueManager.isDialogueActive)
                GameManager.Instance.DialogueManager.AdvanceDialogue();
            else
            {
                if (GameManager.Instance.DialogueManager.CanStartDialogue())
                {
                    CheckForNewConversation();
                    GameManager.Instance.DialogueManager.StartDialogue(currentConversation);
                }
            }
        }
    }
    private void CheckForNewConversation()
    {
        for(int i = 0; i < conversations.Count; i++)
        {
            var convo = conversations[i];
            if (convo != null && convo.IsConditionMet())
            {
                currentConversation=convo;
                if(convo.removeAfterPlay)
                conversations.RemoveAt(i);
                if (convo.removeTheseOnplay != null && convo.removeTheseOnplay.Count > 0)
                {
                    foreach(var toRemove in convo.removeTheseOnplay)
                    {
                        conversations.Remove(toRemove);
                    }
                }
                break;
            }
        }
    }
}
