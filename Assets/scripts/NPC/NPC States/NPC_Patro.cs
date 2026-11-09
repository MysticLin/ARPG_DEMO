using System.Collections;
using UnityEngine;

public class NPC_Patro : MonoBehaviour
{
    public float speed = 2;
    public Vector2[] partorlPoints;
    public float pauseDuration = 1.5f;
    private bool isPasued;
    private Vector2 target;
    private int currentPatrolIndex;
    private Rigidbody2D rb;
    private Animator anim;
   
    void Start()
    {
        rb= GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        StartCoroutine(SetPatorlPoint());
    }

   
    void Update()
    {
        if (isPasued)
        {
            rb.velocity= Vector2.zero;
            return;
        }
        Vector2 direction=(target-(Vector2)transform.position).normalized;
        if (direction.x < 0 && transform.localScale.x > 0 || direction.x > 0 && transform.localScale.x < 0)
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        rb.velocity=direction*speed;
        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            StartCoroutine(SetPatorlPoint());
        }
    }
   IEnumerator SetPatorlPoint()
    {
        isPasued= true;
        anim.Play("Idle");
        yield return new WaitForSeconds(pauseDuration);
        currentPatrolIndex=(currentPatrolIndex+1)%partorlPoints.Length;
        target=partorlPoints[currentPatrolIndex];
        isPasued = false;
        anim.Play("Move");
    }
}
