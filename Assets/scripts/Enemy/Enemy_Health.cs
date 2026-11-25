using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy_Health : MonoBehaviour
{
    [SerializeField] private EnemyDrop[] drops;
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private float deadAnimTime = 0.6f;
    [SerializeField] private float boneLifeTime = 1f;
    public int expReward = 3;
    public string enemyId;
    public static Action<int> OnMonsterDefeated;
    [FormerlySerializedAs("maxHealth")]
    [SerializeField] private int StartMaxHealth = 3;
    [SerializeField] private float DestoryTime = 1.5f;
    [SerializeField] private Animator anim;
    [SerializeField] private Enemy_Combat combat;
    [SerializeField] private Enemy_Move move;
    [SerializeField] private Collider2D col;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Enemy_Knockback knock;
    public Health health { get; private set; }
    public bool isDead=>health != null&&health.IsDead;
    private void Awake()
    {
        health = new Health(StartMaxHealth);
        health.OnDied += HandleDead;

        if (GetComponent<Enemy_StatusEffects>() == null)
            gameObject.AddComponent<Enemy_StatusEffects>();
    }
    private void Start()
    {
        if (string.IsNullOrEmpty(enemyId))
        {
            enemyId = $"{gameObject.scene.name}:{gameObject.name}:{transform.position.x:F2}:{transform.position.y:F2}";
        }

        ApplyWorldState(gameObject.scene.name);
    }
    private void OnDestroy()
    {
        if(health!=null)health.OnDied -= HandleDead;
    }
    public bool ChangeHealth(int amount)
    {
        if(health==null||isDead)return false;
        if (amount < 0)
        {
            if(anim!=null)anim.SetTrigger("isHurt");
            health.TakeDamage(-amount);
            DamageNumberManager.Spawn(-amount, transform.position + Vector3.up * 1f, Color.yellow);
            CameraShake.Instance?.Shake(0.15f);
            HitStop.instance?.Play(0.05f);
        }
        else if(amount>0)health.Heal(amount);
       return !health.IsDead;
    }
    public void ApplyWorldState(string sceneName)
    {
        if (WorldStateManager.Instance == null) return;
        if (string.IsNullOrEmpty(enemyId)) return;

        if (WorldStateManager.Instance.IsEnemyDead(sceneName, enemyId))
            gameObject.SetActive(false);
    }
    public void HandleDead()
    {
        if(rb!=null)rb.velocity=Vector2.zero;
        if(combat!=null)combat.enabled = false;
        if(move!=null)move.enabled = false;
        if (knock != null)
        {
            knock.Cancel();
            knock.enabled = false;
        }
        if(col!=null)col.enabled = false;
        OnMonsterDefeated?.Invoke(expReward);
        if (InventoryManager.Instance != null && drops != null)
        {
            foreach(var drop in drops)
            {
                if (drop.item == null) continue;
                if (UnityEngine.Random.value > drop.chance) continue;
                int amount = UnityEngine.Random.Range(drop.minAmount, drop.maxAmount + 1);
                Vector2 pos = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * 0.3f;
                InventoryManager.Instance.DropLootAt(drop.item, amount, pos);
            }
        }
        if (WorldStateManager.Instance != null &&
    !string.IsNullOrEmpty(enemyId))
        {
            WorldStateManager.Instance.MarkEnemyDead(
                gameObject.scene.name,
                enemyId);
        }
        if (anim != null)
        {
            anim.ResetTrigger("isHurt");
            anim.Play("Dead", 0, 0f);
        }
        StartCoroutine(DeathRoutine());
    }
    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deadAnimTime);
        foreach(var sr in GetComponentsInChildren<SpriteRenderer>())
            sr.enabled = false;
        if (deathEffectPrefab != null)
        {
            GameObject root = new GameObject("BoneFall");
            root.transform.position = transform.position;
            root.transform.rotation = Quaternion.identity;
            GameObject fx = Instantiate(deathEffectPrefab, root.transform);
            fx.transform.localPosition = Vector3.zero;
            fx.transform.localRotation = Quaternion.identity;
            Destroy(root, boneLifeTime);
        }
        yield return new WaitForSeconds(boneLifeTime);
        Destroy(gameObject);
    }
}
[Serializable]
public class EnemyDrop
{
    public ItemSO item;
    public int minAmount = 1;
    public int maxAmount = 1;
    [Range(0f, 1f)] public float chance = 0.5f;
}
