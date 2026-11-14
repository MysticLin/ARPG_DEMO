using System;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public InventorySlot[] itemSlots;
    public UseItem useItem;
    public int gold;
    public TMP_Text goldText;
    public GameObject lootPrefab;
    public Transform player;
    public static event Action<int> OnExperienceGained;
    private Inventory inventory;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        int slotCount = itemSlots != null ? itemSlots.Length : 0;
        inventory = new Inventory(slotCount);
        inventory.Changed += RefreshSlots;
        if (itemSlots != null)
        {
            for(int i = 0; i < itemSlots.Length; i++)
            {
                if (itemSlots[i] != null)
                    itemSlots[i].Bind(i);
            }
        }
    }
    private void Start()
    {
       RefreshSlots();
        UpdateGoldText();
    }
    private void OnDestroy()
    {
        if(inventory!=null)
            inventory.Changed -= RefreshSlots;
        if(Instance==this)
            Instance = null;
    }
    public InventorySlotView GetSlot(int index)
    {
        return inventory != null ? inventory.GetSlot(index) : default;
    }
    public bool canAdd(ItemSO itemSO,int quantity = 1)
    {
        return inventory != null && inventory.CanAdd(itemSO, quantity);
    }
    public void AddItem(ItemSO itemSO,int quantity)
    {
        if (itemSO == null || quantity <= 0 || inventory == null) return;
        if (itemSO.isGold)
        {
            AddGold(quantity);
            return;
        }
        if (itemSO.isExp)
        {
            OnExperienceGained?.Invoke(quantity);
            return;
        }
        int left = inventory.Add(itemSO, quantity);
            if (left > 0)
            DropLoot(itemSO, left);
      
    }
    public void RemoveItem(ItemSO itemSO,int quantity)
    {
        if (inventory == null || itemSO == null || quantity <= 0) return;
        inventory.Remove(itemSO, quantity);
    }
    public int RemoveAt(int index,int quantity)
    {
        return inventory != null ? inventory.RemoveAt(index, quantity) : 0;
    }
    public void UseItem(int index)
    {
        if (inventory == null || useItem == null) return;
        InventorySlotView slot = inventory.GetSlot(index);
        if (slot.IsEmpty) return;
        if (!useItem.ApplyItemEffects(slot.itemSO)) return;
        inventory.RemoveAt(index, 1);
    }
    public void DropItem(int index)
    {
        if (ShopKeeper.currentShopkeeper != null) return;
        if (inventory == null) return;
        InventorySlotView slot = inventory.GetSlot(index);
        if (slot.IsEmpty) return;
        if (inventory.RemoveAt(index, 1) == 0) return;
        DropLoot(slot.itemSO, 1);
    }
    public bool HasItem(ItemSO itemSO)
    {
        return inventory != null && inventory.Has(itemSO);
    }
    public int GetItemQuantity(ItemSO itemSO)
    {
        return inventory != null ? inventory.Count(itemSO) : 0;
    }
    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        gold += amount;
        UpdateGoldText();
    }
    public  bool TrySpendGold(int amount)
    {
        if (amount < 0 || gold < amount) return false;
        gold -= amount;
        UpdateGoldText();
        return true;
    }
    public int TryAddItem(ItemSO item,int quantity)
    {
        return inventory!=null?inventory.Add(item,quantity):quantity;
    }
    public void UpdateGoldText()
    {
        if (goldText != null)
            goldText.text = gold.ToString();
    }
    public void RefreshSlots()
    {
        if (inventory == null || itemSlots == null) return;
        for(int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i]==null)continue;
            InventorySlotView slot=inventory.GetSlot(i);
            itemSlots[i].Set(slot.itemSO, slot.Quantity);
        }
    }
   
    private void DropLoot(ItemSO itemSO, int quantity)
    {
        if (itemSO == null || quantity <= 0) return;
        if (lootPrefab == null || player == null) return;
        Loot loot = Instantiate(lootPrefab, player.position, Quaternion.identity).GetComponent<Loot>();
        if(loot!=null)
        loot.Initialize(itemSO, quantity);
    }

}
