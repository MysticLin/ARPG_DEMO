using UnityEngine;

public class Loot : MonoBehaviour
{
    public ItemSO itemSO;
    public SpriteRenderer sr;
    public Animator anim;
    public bool canBePickedUp=true;
    public int quantity;
    public LootPool Pool { get;set; }
    [SerializeField]private float pickupDelay = 0.8f;
    private bool picked;
    private void OnValidate()
    {
        if (itemSO != null)
        UpdateAppearance();
    }
    public void Initialize(ItemSO itemSO,int quantity)
    {
        CancelInvoke();
        picked = false;
        this.itemSO = itemSO;
        this.quantity = quantity;
        canBePickedUp = false;
        if (anim != null)
            anim.Play("Idle", 0, 0f);
        UpdateAppearance();
        Invoke(nameof(EnablePickup), pickupDelay);
    }
    private void EnablePickup()
    {
        if (picked) return;
        canBePickedUp =true;
    }
    private void UpdateAppearance()
    {
        if (sr == null || itemSO == null) return;
        sr.sprite = itemSO.icon;
        this.name = itemSO.itemName;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (picked || !canBePickedUp) return;
        if (!collision.CompareTag("Player")) return;
        if (itemSO == null || quantity <= 0) return;
        if (InventoryManager.Instance == null) return;
        int left = InventoryManager.Instance.TryAddItem(itemSO, quantity);
        if (left > 0)
        {
            quantity = left;
            canBePickedUp = false;
            Invoke(nameof(EnablePickup), 1f);
            return;
        }
        picked = true;
        if(anim!=null)
        anim.Play("LootPickUp");
        Invoke(nameof(Despawn), 0.5f);
    }
    private void Despawn()
    {
        if (Pool != null)
            Pool.Release(this);
        else
            Destroy(gameObject);
    }
}
