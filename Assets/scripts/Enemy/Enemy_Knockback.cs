using System.Collections;
using UnityEngine;

public class Enemy_Knockback : MonoBehaviour
{
    private Rigidbody2D rb;
    private Enemy_Move enemy_move;
    private bool cancel;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy_move = GetComponent<Enemy_Move>();
    }
    private void OnEnable()
    {
        cancel = false;
    }
    public void Knockback(Transform forceTransform,float knockbackForce,float stunTime,float knockbackTime)
    {
        if(cancel||!enabled)return;
        enemy_move.ChangeState(EnemyState.Knockback);
        StartCoroutine(StunTimer(stunTime,knockbackTime));
        Vector2 direction=(transform.position-forceTransform.position).normalized;
        rb.velocity= direction*knockbackForce;
    }
    public void Cancel()
    {
        cancel = true;
        StopAllCoroutines();
        if(rb!=null)rb.velocity= Vector2.zero;
        if(enemy_move!=null)enemy_move.ResetStare();
    }
    IEnumerator StunTimer(float stunTime,float knockbackTime)
    {
        yield return new WaitForSeconds(knockbackTime);
        if (cancel) yield break;
        if(rb!=null)rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(stunTime);
        if(enemy_move!=null)enemy_move.ChangeState(EnemyState.Idle);
    }
}
