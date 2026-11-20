using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 direction=Vector2.right;
    public float lifeSpawn = 2;
    public float speed;
    public ArrowPool Pool { get;set; }
    private Sprite normalSprite;
    public LayerMask enemyLayer;
    public LayerMask obstacleLayer;
    public SpriteRenderer sr;
    public Sprite buriedSprite;
    public int damage;
    public float knockbackForce;
    public float knockbackTime;
    public float stunTime;
    private bool hasHit;
    private void Awake()
    {
        if(sr!=null)
            normalSprite = sr.sprite;
    }
    public void Init(Vector2 dir)
    {
        transform.localScale = Vector3.one;
        CancelInvoke();
        hasHit = false;
        direction = dir;
        if (sr != null)
            sr.sprite = normalSprite;
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = dir * speed;
        }
        transform.SetParent(null);
        RotateArrow();
        Invoke(nameof(Despawn), lifeSpawn);
    }
    private void Despawn()
    {
        if (Pool != null)
            Pool.Release(this);
        else
            Destroy(gameObject);
    }
    private void RotateArrow()
    {
        float angle=Mathf.Atan2(direction.y,direction.x)*Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(hasHit)return;
        if ((enemyLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            hasHit = true;
            Enemy_Health enemy_Health = collision.gameObject.GetComponent<Enemy_Health>();
            if (enemy_Health != null && enemy_Health.ChangeHealth(-damage))
            {
               Enemy_Knockback enemy_KnockBack=collision.gameObject.GetComponent<Enemy_Knockback>();
                if (enemy_KnockBack != null) enemy_KnockBack.Knockback(transform, knockbackForce, stunTime, knockbackTime);
            }
            AttachToTarget(collision.gameObject.transform);
        }
        else if((obstacleLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            hasHit=true;
            AttachToTarget(collision.gameObject.transform);
        }
    }
    private void AttachToTarget(Transform target)
    {
        sr.sprite=buriedSprite;
        rb.velocity = Vector2.zero;
        rb.isKinematic=true;
        transform.SetParent(target);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle+180f);
    }

}
