using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Timeline;

public class Enemy_Move : MonoBehaviour
{
    [Header("移动")]
    public float speed;
    [Header("检测")]
    public float playerDetectRange = 5;
    public Transform detectionPoint;
    public LayerMask playerMark;
    [SerializeField] private LayerMask obstacleLayer;
    [Header("攻击")]
    public float attackRange = 2;
    public float attackCooldown = 2;
    private float attackCooldownTimer;
    private float attackStateEndTime;
    [Header("巡逻/脱战")]
    [SerializeField] private float patrolRadius = 3f;
    [SerializeField] private float patrolWaitTime = 1.5f;
    [SerializeField] private float leashRange = 8f;
    [SerializeField] private float loseTargetTime = 2f;
    [SerializeField] private float idleTime = 1.5f;
    [Header("层级")]
    [SerializeField] private int elevationLevel = 0;
    private readonly Collider2D[] hits = new Collider2D[4];

    private EnemyState enemyState;
    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;
   
    private Vector2 homePosition;
    private Vector2 patrolTarget;

    private float lostTimer;
    private float idleTimer;
    private float patrolWaitTimer;
    private int facingDirection = -1;
    private void Start()
    {
        homePosition=transform.position;
        patrolTarget = homePosition;
        rb = GetComponent<Rigidbody2D>();
        anim= GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
    }
    private void Update()
    {
        if (enemyState == EnemyState.Knockback)
            return;
        if(attackCooldown>0f)
            attackCooldownTimer-=Time.deltaTime;
        CheckForPlayer();
        switch (enemyState)
        {
            case EnemyState.Idle:
                Idle();
                break;

            case EnemyState.Patrolling:
                Patrol();
                break;

            case EnemyState.Chasing:
                Chase();
                break;

            case EnemyState.Attacking:
                if (rb != null)
                    rb.velocity = Vector2.zero;
                if (Time.unscaledTime >= attackStateEndTime)
                    ChangeState(EnemyState.Idle);
                break;

            case EnemyState.Returning:
                ReturnHome();
                break;
        }
    }
    private void Idle()
    {
        if(rb!=null)
            rb.velocity = Vector2.zero;
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleTime)
        {
            PickPatrolTarget();
            ChangeState(EnemyState.Patrolling);
        }
    }
    private void Patrol()
    {
        if (Vector2.Distance(transform.position, patrolTarget) <= 0.2f)
        {
            if (rb != null)
                rb.velocity = Vector2.zero;
            patrolWaitTimer += Time.deltaTime;
            if (patrolWaitTimer >= patrolWaitTime)
            {
                patrolWaitTimer = 0f;
                PickPatrolTarget();
            }
            return;
        }
        MoveTo(patrolTarget);
    }
    private void Chase()
    {
        if (player == null)
        {
            ChangeState(EnemyState.Returning);
            return;
        }
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            if(rb!=null)
                rb.velocity = Vector2.zero;
            if (attackCooldownTimer <= 0f)
            {
                attackCooldownTimer = attackCooldown;
                ChangeState(EnemyState.Attacking);
            }
            else
                ChangeState(EnemyState.Idle);
            return;
        }
        MoveTo(player.position);
    }
    private void ReturnHome()
    {
        if (Vector2.Distance(transform.position, homePosition) <= 0.3f)
        {
            if(rb!=null)
               rb.velocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
            return;
        }
        MoveTo(homePosition);
    }
    private void MoveTo(Vector2 target)
    {
        Vector2 direction = target - (Vector2)transform.position;
        if (direction.sqrMagnitude < 0.001f)
        {
            if (rb != null)
                rb.velocity = Vector2.zero;
            return;
        }
        direction.Normalize();
        if (Mathf.Abs(direction.x) > 0.01f)
            SetFacing(direction.x);
        if (rb != null)
            rb.velocity = direction * speed;
    }
    private void SetFacing(float x)
    {
        int direction=x>0f? 1 : -1;
        if (direction == facingDirection)
            return;
        facingDirection= direction;
        Vector3 scale=transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }
    private void PickPatrolTarget()
    {
        patrolTarget = homePosition + Random.insideUnitCircle * patrolRadius;
        patrolWaitTimer = 0f;
    }
    private void CheckForPlayer()
    {
        if (detectionPoint == null)
            return;
        int count = Physics2D.OverlapCircleNonAlloc(
            detectionPoint.position, playerDetectRange, hits, playerMark);
        if (count == 0)
        {
            OnPlayerLost();
            return;
        }
        Transform candidate=hits[0].transform;
        Player_Move playerMove=candidate.GetComponentInParent<Player_Move>();
        if (playerMove == null || playerMove.CurrentElevation != elevationLevel)
        {
            OnPlayerLost();
            return;
        }
        if (!HasLineOfSight(candidate))
        {
            OnPlayerLost();
            return;
        }
        player=candidate;
        lostTimer = 0;
        if (enemyState == EnemyState.Attacking)
            return;
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            if (attackCooldownTimer <= 0f)
            {
                attackCooldownTimer = attackCooldown;
                ChangeState(EnemyState.Attacking);
            }
            else
            {
                rb.velocity = Vector2.zero;
                ChangeState(EnemyState.Idle);
            }
            return;
        }
        ChangeState(EnemyState.Chasing);
    }
    private void OnPlayerLost()
    {
        if (enemyState == EnemyState.Attacking)
            return;
        lostTimer += Time.deltaTime;
        if (lostTimer < loseTargetTime)
            return;
        if (Vector2.Distance(transform.position, homePosition) > 0.5f)
            ChangeState(EnemyState.Returning);
        else
            ChangeState(EnemyState.Idle);
    }

    private bool HasLineOfSight(Transform target)
    {
        if (target == null || detectionPoint == null)
            return false;
        if(obstacleLayer.value==0)
            return true;
        Vector2 origin = detectionPoint.position;
        Vector2 targetPos=target.position;
        RaycastHit2D hit = Physics2D.Linecast(origin, targetPos, obstacleLayer);
            return hit.collider == null;
    }
    public void ResetStare()
    {
       StopAllCoroutines();
        if(rb!=null)
            rb.velocity=Vector2.zero;
        ChangeState(EnemyState.Idle);
    }
    public void ChangeState(EnemyState newstate)
    {
        if (enemyState == newstate)
            return;
        if (anim != null)
        {
            anim.SetBool("isIdle", false);
            anim.SetBool("isChasing", false);
            anim.SetBool("isAttacking", false);
        }
        enemyState= newstate;
        if (newstate == EnemyState.Idle)
        {
            idleTimer = 0;
            lostTimer = 0;
            patrolWaitTimer = 0;
        }
        if (anim != null)
        {
            if (newstate == EnemyState.Idle)
                anim.SetBool("isIdle", true);
            else if (newstate == EnemyState.Chasing || newstate == EnemyState.Patrolling
                || newstate == EnemyState.Returning)
                anim.SetBool("isChasing", true);
            else if (newstate == EnemyState.Attacking)
            {
                anim.SetBool("isAttacking", true);
                attackStateEndTime = Time.unscaledTime + 0.8f;
            }
               
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (detectionPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);
            Gizmos.color= Color.yellow;
            Gizmos.DrawWireSphere(detectionPoint.position, attackRange);
        }
        Gizmos.color = Color.green;
        Vector2 center = Application.isPlaying ? homePosition : transform.position;
        Gizmos.DrawWireSphere(center, patrolRadius);
       
    }
}
public enum EnemyState{
    Idle,
    Chasing,
    Attacking,
    Knockback,
    Returning,
    Patrolling,
}
