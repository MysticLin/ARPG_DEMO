using System;
using System.Collections;
using UnityEngine;

public class Player_Health : MonoBehaviour
{
    public Animator HealthAnim;  
    public Health health;
    public float InvincibleTime=1;
    private float InvincibleTimeUntil;
    [SerializeField] private CanvasGroup DeadCanvas;
    [SerializeField] private Player_Move move;
    [SerializeField] private Player_Combat combat;
    [SerializeField] private Player_Bow bow;
    [SerializeField] private Collider2D col;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer[] playerRenderers;
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private Animator anim;
    [SerializeField] private float deadAnimTime = 0.6f;
    [SerializeField] private float boneLifeTime = 1f;
    private Coroutine flashRoutine;
    private void OnEnable() => Bind();
    private void Start()=>Bind();
    private void Awake()
    {
        if (playerRenderers == null || playerRenderers.Length == 0)
            playerRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }
    private void Bind()
    {
        if (StatsManager.Instance == null) return;
        Health newhealth = StatsManager.Instance.PlayerHealth;
        if (newhealth == null) return;

        if (health != null) health.OnDied-=PlayDead;
        health = newhealth;
        health.OnDied -= PlayDead;
        health.OnDied += PlayDead;  
    }
    private void OnDisable()
    {
        if(health!=null)
        health.OnDied -= PlayDead;
    }
    public bool TryTakeDamage(int amount)
    {
        if (amount <= 0 || health == null || StatsManager.Instance == null) return false;
        if (StatsManager.Instance.isDead||Time.time<InvincibleTimeUntil)
            return false;
        StatsManager.Instance.UpdateHealth(-amount);
        HitStop.instance?.Play(0.05f);
        if(flashRoutine!=null)
            StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashDamage());
        if(HealthAnim!=null)HealthAnim.Play("HealthChange");
        InvincibleTimeUntil = Math.Max(Time.time + InvincibleTime,InvincibleTimeUntil);
        return !health.IsDead;
    }
    public void PlayDead()
    {
        if (health==null) return;
        ResetSet();
        SetControl(false);
        if(rb!=null)rb.velocity = Vector2.zero;
        if (anim != null)
            anim.Play("Dead", 0, 0f);
        StartCoroutine(DeathRoutine());
    }
    public void PlayRevive()
    {
        if(health==null||!health.IsDead)return;
        PauseService.Instance?.Pop();
        health.Revive();
        if(rb!=null)rb.velocity=Vector2.zero;
        if (anim != null) anim.Play("Idle", 0, 0f);
        Vector3 scale = transform.localScale;
        scale.x=Mathf.Abs(scale.x);
        if (scale.x < 0.01f) scale.x = 1f;
        transform.localScale = scale;
        SetControl(true);
        ResetSet();
        InvincibleTimeUntil = Time.time + InvincibleTime;
    }
    private void SetControl(bool isEnable)
    {
        if(move!=null) move.enabled = isEnable;
        if(col!=null) col.enabled = isEnable;
    }
    public void ResetSet()
    {
        if (DeadCanvas != null)
        {
            DeadCanvas.alpha = 0;
            DeadCanvas.blocksRaycasts = false;
            DeadCanvas.interactable =false;
        }
        if (move != null) move.ResetState();
        if (combat != null) combat.ResetState();
        if (bow != null) bow.ResetState();
    }
    private IEnumerator FlashDamage()
    {
        Color[] originals = new Color[playerRenderers.Length];
        for(int i = 0; i < playerRenderers.Length; i++)
        {
            if (playerRenderers[i] == null) continue;
            originals[i]=playerRenderers[i].color;
            playerRenderers[i].color = Color.red;
        }
        yield return new WaitForSecondsRealtime(0.08f);
        for(int i = 0; i < playerRenderers.Length; i++)
        {
            if (playerRenderers[i]!=null)
                playerRenderers[i].color=originals[i];
        }
        flashRoutine = null;
    }
    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deadAnimTime);
        foreach(var sr in playerRenderers)
        {
            if (sr != null)
                sr.enabled = false;
        }
        if (deathEffectPrefab != null)
        {
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
        }
        yield return new WaitForSeconds(boneLifeTime);
        PauseService.Instance?.Push();
        if (DeadCanvas != null)
        {
            DeadCanvas.alpha = 1;
            DeadCanvas.blocksRaycasts = true;
            DeadCanvas.interactable = true;
        }
    }
}
