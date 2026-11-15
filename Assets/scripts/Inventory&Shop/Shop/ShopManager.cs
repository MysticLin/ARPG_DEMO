using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance {  get; private set; }
    [SerializeField] private ShopSlot[] shopSlots;
    [SerializeField] private InventoryManager inventoryManager;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        if(inventoryManager==null)
            inventoryManager=InventoryManager.Instance;
    }
    private void OnDestroy()
    {
        if(Instance==this)Instance=null;
    }
    public void PopulateShopItems(List<ShopItems> shopItems)
    {
        if (shopSlots == null || shopItems == null) return;
        for(int i = 0; i < shopItems.Count && i < shopSlots.Length; i++)
        {
            ShopItems shopItem=shopItems[i];
            shopSlots[i].Initialize(shopItem.itemSO, shopItem.price);
            shopSlots[i].gameObject.SetActive(true);
        }
        for(int i = shopItems.Count; i < shopSlots.Length; i++)
        {
            shopSlots[i].gameObject.SetActive(false);
        }
    }
    public void TryBuyItem(ItemSO itemSO,int price)
    {
        if (itemSO == null || inventoryManager == null) return;
        if (!inventoryManager.canAdd(itemSO, 1)) return;
        if (!inventoryManager.TrySpendGold(price)) return;
        inventoryManager.AddItem(itemSO, 1);
    }
    public bool SellItem(int inventoryIndex) 
    {
        if (inventoryManager == null || shopSlots == null) return false;
        InventorySlotView slot = inventoryManager.GetSlot(inventoryIndex);
        if (slot.IsEmpty) return false;
        foreach(ShopSlot shopSlot in shopSlots)
        {
            if (shopSlot == null || shopSlot.itemSO != slot.itemSO) continue;
            int price = shopSlot.price / 2;
            if (inventoryManager.RemoveAt(inventoryIndex, 1) == 0) return false;
            inventoryManager.AddGold(price);
            return true;
        }
        return false;
    }
}
[System.Serializable]
public class ShopItems
{
    public ItemSO itemSO;
    public int price;

}
