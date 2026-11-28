using UnityEngine;
using UnityEngine.EventSystems;

public class Player_Bow : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject arrowPrefab;
    public Player_Move playerMove;
    private Vector2 aimDirection=Vector2.right;
    public float shootCooldown = 0.5f;
    public float shootTimer;
    public Animator anim;
    private Vector2 shotDirection = Vector2.right;
    [SerializeField] private float maxShootTime = 2f;
    [SerializeField] private ArrowPool arrowPool;
    void Update()
    {
        if (PauseService.Instance != null && PauseService.Instance.IsPaused) return;
        if (DialogueManager.IsDialogueActive) return;
        if (StatsManager.Instance==null||StatsManager.Instance.isDead) return;
        shootTimer -=Time.deltaTime;
        if(playerMove.state==PlayerState.KnockBack)
        {
            CancelInvoke(nameof(EndShooting));
            if (anim != null) anim.SetBool("isShooting", false);
            return;
        }
        if (playerMove.state == PlayerState.Shooting) return;
        HandleAiming();
        if (Input.GetMouseButtonDown(0)&&!IsPointerOverUI()&& shootTimer <= 0)
        {
          shotDirection=aimDirection;
            playerMove.SetFacing(shotDirection.x);
            playerMove.SetShooting(true);
            if(anim!=null)anim.SetBool("isShooting",true);
            CancelInvoke(nameof(EndShooting));
            Invoke(nameof(EndShooting), maxShootTime);
        }
    }
    private void OnEnable()
    {
        anim.SetLayerWeight(0,0);
        anim.SetLayerWeight(1, 1);
    }
    private void OnDisable()
    {
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(1, 0);
    }
    public void Shoot()
    {
        if (StatsManager.Instance == null || StatsManager.Instance.isDead) return;
        if (playerMove == null || playerMove.state != PlayerState.Shooting) return;
        if (shootTimer > 0) return;
        Collider2D playerCollider=playerMove.GetComponent<Collider2D>();
        Vector2 dir=shotDirection.normalized;
        float offset = 0.6f;
        if (playerCollider != null)
        {
            offset = Mathf.Max(playerCollider.bounds.extents.x,playerCollider.bounds.extents.y)+0.1f;
        }
        Vector2 spawnPosition = (Vector2)launchPoint.position + dir * offset;
        Arrow arrow = arrowPool != null ? arrowPool.Get(spawnPosition) :
            Instantiate(arrowPrefab, spawnPosition, Quaternion.identity).GetComponent<Arrow>();
        arrow.Init(shotDirection);
        Collider2D arrowCollider = arrow.GetComponent<Collider2D>();
        if (playerCollider != null && arrowCollider != null)
            Physics2D.IgnoreCollision(playerCollider, arrowCollider, true);
        shootTimer = shootCooldown;
    }
    public void EndShooting()
    {
        CancelInvoke(nameof(EndShooting));
        if(anim!=null)
            anim.SetBool("isShooting",false);
        if(playerMove!=null)
            playerMove.SetShooting(false);
    }
    public void HandleAiming()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        if (horizontal != 0 || vertical != 0)
        {
            aimDirection=new Vector2(horizontal, vertical).normalized;
        }
        else
        {
            aimDirection = new Vector2(playerMove.facingDirection, 0);
        }
        anim.SetFloat("aimX", aimDirection.x);
        anim.SetFloat("aimY", aimDirection.y);
    }
    public void ResetState()
    {
        shootTimer=0;
        CancelInvoke(nameof(EndShooting));
        if (playerMove != null) playerMove.SetShooting(false);
        if (anim != null) anim.SetBool("isShooting", false);
    }
    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
