using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    public PlayerState state {  get; private set; }=PlayerState.Normal;
    [SerializeField] private SpriteRenderer[] visuals;
    public int facingDirection = 1;
    public Rigidbody2D rb;
    public Animator anim;
    public Player_Combat player_Combat;
    private readonly List<int> elevationStack = new();
    public int CurrentElevation { get; private set; }
    public void SetElevation(int level)
    {
        CurrentElevation = level;
    }
    /* public void PushElevation(int level)
     {
         elevationStack.Add(level);
     }
     public void PopElevation()
     {
         if(elevationStack.Count>0)
             elevationStack.RemoveAt(elevationStack.Count-1);
     }
    */
    public void Awake()
    {
        if(visuals==null||visuals.Length==0)
            visuals=GetComponentsInChildren<SpriteRenderer>(true);
        if (visuals != null && visuals.Length > 0 && visuals[0]!=null)
            facingDirection = visuals[0].flipX ? -1 : 1;
    }
    private void Update()
    {
        if(PauseService.Instance!=null&&PauseService.Instance.IsPaused)return;
        if (DialogueManager.IsDialogueActive) return;
        if (state != PlayerState.Normal) return;
        if (Input.GetButtonDown("Slash")&&player_Combat!=null&&player_Combat.enabled)
        {
            player_Combat.Attack();
        }   
    }
    void FixedUpdate()
    {
        if (PauseService.Instance!=null&&PauseService.Instance.IsPaused)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        if (DialogueManager.IsDialogueActive)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        if (state == PlayerState.Shooting)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        if (state==PlayerState.KnockBack)
            return;
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
        if (Mathf.Abs(horizontal) > 0.01f)
        {
            int dir = horizontal > 0 ? 1 : -1;
            if (dir != facingDirection)
                ApplyFacing(dir);
        }
            anim.SetFloat("horizontal", Mathf.Abs(horizontal));
            anim.SetFloat("vertical", Mathf.Abs(vertical));

            rb.velocity = new Vector2(horizontal, vertical) * StatsManager.Instance.speed;
    }
    public void SetShooting(bool shooting)
    {
        if (state == PlayerState.KnockBack) return;
        state=shooting?PlayerState.Shooting:PlayerState.Normal;
        if(shooting&&rb!=null)
            rb.velocity = Vector2.zero;
    }
    public void Knockback(Transform enemy,float force,float stunTime)
    {
        Vector2 direction=(transform.position-enemy.position).normalized;
        rb.velocity = direction*force;
        ApplyKnockback(direction, force, stunTime);
    }
    public void ApplyKnockback(Vector2 direction,float force,float stunTime)
    {
        StopAllCoroutines();
        state=PlayerState.KnockBack;
        rb.velocity=direction.normalized*force;
        StartCoroutine(KnockbackCounter(stunTime));
    }
    public void SetFacing(float x)
    {
        if(Mathf.Abs(x)<0.01f)return;
        ApplyFacing(x > 0 ? 1 : -1);
    }
    public void ResetState()
    {
        StopAllCoroutines();
        state=PlayerState.Normal;
        if(rb!=null)
            rb.velocity = Vector2.zero;
    }
    private IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.velocity= Vector2.zero;
        if (state == PlayerState.KnockBack)
            state = PlayerState.Normal;
    }
    private void ApplyFacing(int dir)
    {
        if (dir == facingDirection) return;
        facingDirection=dir;
        if (visuals == null) return;
        foreach(var sr in visuals)
        {
            if (sr != null)
                sr.flipX = dir < 0;
        }
    }
}
public enum PlayerState
{
    Normal,
    Shooting,
    KnockBack
}
