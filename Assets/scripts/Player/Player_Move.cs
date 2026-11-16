using System.Collections;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    public PlayerState state {  get; private set; }=PlayerState.Normal;
    public int facingDirection = 1;
    public Rigidbody2D rb;
    public Animator anim;
    public Player_Combat player_Combat;
    private void Update()
    {
        if (DialogueManager.IsDialogueActive) return;
        if (state != PlayerState.Normal) return;
        if (Input.GetButtonDown("Slash")&&player_Combat!=null&&player_Combat.enabled)
        {
            player_Combat.Attack();
        }   
    }
    void FixedUpdate()
    {
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
            if (horizontal > 0 && transform.localScale.x < 0 || horizontal < 0 && transform.localScale.x > 0)
            {
                Flip();
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
        int dir=x>0 ? 1 : -1;
        facingDirection = dir;
        Vector3 scale=transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dir;
        transform.localScale = scale;
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
    private void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
}
public enum PlayerState
{
    Normal,
    Shooting,
    KnockBack
}
