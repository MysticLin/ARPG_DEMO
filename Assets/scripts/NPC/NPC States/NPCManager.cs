using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public NPCState currentState;
    public NPCState defaultState;
    public NPC_Patro patrol;
    public NPC_Wander wander;
    public NPC_Talk talk;

    private Rigidbody2D rb;
    public Animator anim;
    void Start()
    {
        defaultState = currentState;
        rb = GetComponent<Rigidbody2D>();
        SwitchState(currentState);
      
        
    }
    public void SwitchState(NPCState newState)
    {
        if (newState == NPCState.Idle)
        {
            rb.velocity = Vector2.zero;
            anim.Play("Idle");
        }
        currentState = newState;
        patrol.enabled=newState==NPCState.Patrol;
        wander.enabled=newState==NPCState.Wander;
        talk.enabled=newState==NPCState.Talk;
      
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            SwitchState(NPCState.Talk);
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            SwitchState(defaultState);
    }
}
public enum NPCState{
    Default,
    Idle,
    Patrol,
    Wander,
    Talk,
}
