using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Schema;
using UnityEditor;
using UnityEngine;

public class NPC_Wander : MonoBehaviour
{
    [Header("漫游区域")]
    public float wanderWidth = 5;
    public float wanderHeight = 5;
    public Vector2 startingPosition;
    public float pauseDuration = 1;

    public float speed;
    public Vector2 target;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isPaused;

    private void Awake()
    {
        anim=GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        StartCoroutine(PauseAndPickNewDestination());
    }
    private void Update()
    {
        if(isPaused)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        if (Vector2.Distance(transform.position, target) < 0.1f)
            StartCoroutine(PauseAndPickNewDestination());
        Move();
    }
    private void Move()
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        if (direction.x > 0 && transform.localScale.x < 0 || direction.x < 0 && transform.localScale.x > 0)
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        rb.velocity = direction * speed;
    }

    IEnumerator PauseAndPickNewDestination()
    {
        isPaused = true;
        anim.Play("Idle");
        yield return new WaitForSeconds(pauseDuration);
        target = GetRandomTarget();
        isPaused = false;
        anim.Play("Move");
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!enabled) return;
        StartCoroutine (PauseAndPickNewDestination());
    }
    private Vector2 GetRandomTarget()
    {
        float halfWidth = wanderWidth / 2;
        float halfHeigh=wanderHeight / 2;
        int edge = Random.Range(0, 4);
        return edge switch
        {
            0 => new Vector2(startingPosition.x - halfWidth, Random.Range(startingPosition.y - halfHeigh, startingPosition.y + halfHeigh)),//范围左边
            1 => new Vector2(startingPosition.x + halfWidth, Random.Range(startingPosition.y - halfHeigh, startingPosition.y + halfHeigh)),//范围右边
            2 => new Vector2(Random.Range(startingPosition.x-halfWidth,startingPosition.x+halfWidth),startingPosition.y-halfHeigh),//范围底部
            _ => new Vector2(Random.Range(startingPosition.x - halfWidth, startingPosition.x + halfWidth), startingPosition.y + halfHeigh),//范围顶部
        };
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(startingPosition, new Vector3(wanderWidth, wanderHeight, 0)); ;
    }
}
