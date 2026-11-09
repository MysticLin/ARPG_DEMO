using System.Collections;
using UnityEngine;

public class Enemy_Knockback : MonoBehaviour
{
    private Rigidbody2D rb;
    private Enemy_Move enemy_move;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy_move = GetComponent<Enemy_Move>();
    }
    public void Knockback(Transform forceTransform,float knockbackForce,float stunTime,float knockbackTime)
    {
        enemy_move.ChangeState(EnemyState.Knockback);
        StartCoroutine(StunTimer(stunTime,knockbackTime));
        Vector2 direction=(transform.position-forceTransform.position).normalized;
        rb.velocity= direction*knockbackForce;
    }
    IEnumerator StunTimer(float stunTime,float knockbackTime)
    {
        yield return new WaitForSeconds(knockbackTime);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(stunTime);
        enemy_move.ChangeState(EnemyState.Idle);
    }
}
