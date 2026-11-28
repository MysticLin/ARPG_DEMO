using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    [Header("快捷栏")]
    public InventorySlot[] itemSlots;
    [Header("背包")]
    public InventorySlot[] backpackSlots;
    public UIWindow backpackWindow;
    [Header("其他")]
    public CanvasGroup backpackCanvas;
    private bool backpackOpen;
    public UseItem useItem;
    public int gold;
    public TMP_Text goldText;
    public GameObject lootPrefab;
    public Transform player;
    public static event Action<int> OnExperienceGained;
    [SerializeField] private LootPool lootPool;
    private Inventory inventory;
    public bool BackpackOpen => backpackOpen;
    public int QuickbarSlotCount=>itemSlots!=null?itemSlots.Length : 0;
    public int BackpackSlotCount => backpackSlots != null ? backpackSlots.Length : 0;
    public int SlotCount=>inventory!=null?inventory.SlotCount: 0;   
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        int quickbarCount = QuickbarSlotCount;
        int backpackCount = BackpackSlotCount;
        int totalCount=quickbarCount+backpackCount;
        inventory = new Inventory(totalCount);
        inventory.Changed += RefreshSlots;
        BindSlots(itemSlots, 0);
        BindSlots(backpackSlots, quickbarCount);
        if (backpackWindow == null && backpackCanvas != null)
        {
            backpackWindow = backpackCanvas.GetComponent<UIWindow>();
            if (backpackWindow == null)
                backpackWindow = backpackCanvas.GetComponentInParent<UIWindow>();
        }
        if (backpackWindow != null)
            backpackWindow.OnHidden += HandleBackpackHidden;
        HideBackpackImmediate();
    }
    private void Start()
    {
       RefreshSlots();
        UpdateGoldText();
    }
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.B))
            return;
        if (backpackWindow == null) return;
        if (backpackOpen)
        {
            UIWindowManager.Instance?.Toggle(backpackWindow);
            return;
        }
        if (!CanOpenBackpack())
            return;
        OpenBackpack();
    }
    private void OnDestroy()
    {
       if(backpackWindow!=null)
            backpackWindow.OnHidden -= HandleBackpackHidden;
        if (inventory!=null)
            inventory.Changed -= RefreshSlots;
        if(Instance==this)
            Instance = null;
    }
    private void BindSlots(InventorySlot[] slots,int offset)
    {
        if (slots == null) return;
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i]!=null)
                slots[i].Bind(offset+i);
        }
    }
    private bool CanOpenBackpack()
    {
       if(StatsManager.Instance!=null&&StatsManager.Instance.isDead)
            return false;
       if(DialogueManager.IsDialogueActive)
            return false;
       if(QuestLogUI.IsOpen)
            return false;
       return true;
    }
    public void OpenBackpack()
    {
        if(backpackWindow==null||backpackOpen)return;
        backpackOpen = true;
        EnsureBackpackSorting();
        UIWindowManager.Instance?.Open(backpackWindow);
    }

    private void EnsureBackpackSorting()
    {
        if (backpackCanvas == null) return;
        Canvas canvas = backpackCanvas.GetComponent<Canvas>();
        if (canvas == null)
            canvas = backpackCanvas.gameObject.AddComponent<Canvas>();
        if (backpackCanvas.GetComponent<GraphicRaycaster>() == null)
            backpackCanvas.gameObject.AddComponent<GraphicRaycaster>();
        backpackCanvas.transform.SetAsLastSibling();
    }
    public void CloseBackpack()
    {
        if (!backpackOpen || backpackWindow == null) return;
        UIWindowManager.Instance?.Close(backpackWindow);
    }
    private void HandleBackpackHidden()
    {
        backpackOpen = false;
    }
    private void HideBackpackImmediate()
    {
        backpackOpen = false;
        if (backpackWindow != null)
        {
            backpackWindow.Hide();
            return;
        }
        if (backpackCanvas == null) return;
        backpackCanvas.alpha=0f;
        backpackCanvas.blocksRaycasts = false;
        backpackCanvas.interactable=false;
    }
    public InventorySlotView GetSlot(int index)
    {
        return inventory != null ? inventory.GetSlot(index) : default;
    }
    public void SetSlot(int index, ItemSO itemSO, int quantity)
    {
        if (inventory != null)
            inventory.SetSlot(index, itemSO, quantity);
    }
    public bool MoveItem(int fromIndex,int toIndex)
    {
        return inventory!=null&&inventory.MoveSlot(fromIndex, toIndex);
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
    public void SetGold(int value)
    {
        gold=Math.Max(0, value);
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
        if (item == null || quantity <= 0) return 0;
        if (item.isGold)
        {
            AddGold(quantity);
            return 0;
        }
        if (item.isExp)
        {
            OnExperienceGained?.Invoke(quantity);
            return 0;
        }
        return inventory!=null?inventory.Add(item,quantity):quantity;
    }
    public void UpdateGoldText()
    {
        if (goldText != null)
            goldText.text = gold.ToString();
    }
    public void RefreshSlots()
    {
        RefreshGroup(itemSlots, 0);
        RefreshGroup(backpackSlots, QuickbarSlotCount);
    }
    private void RefreshGroup(InventorySlot[] slots,int offset)
    {
        if (inventory == null || slots == null) return;
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i]==null)continue;
            InventorySlotView slot = inventory.GetSlot(offset + i);
            slots[i].Set(slot.itemSO, slot.Quantity);
        }
    }
    private void DropLoot(ItemSO itemSO, int quantity)
    {
        if (itemSO == null || quantity <= 0) return;
        if (player == null) return;
        DropLootAt(itemSO, quantity, player.position);
    }
    public void DropLootAt(ItemSO itemSO,int quantity,Vector2 position)
    {
        if(itemSO == null || quantity <= 0)return;
        if (lootPool != null)
        {
            lootPool.Get(itemSO, quantity, position);
            return;
        }
        if (lootPrefab == null) return;
        Loot loot = Instantiate(lootPrefab, position, Quaternion.identity).GetComponent<Loot>();
        if(loot!=null)
            loot.Initialize(itemSO, quantity);
    }
    public void ClearInventory()
    {
        if (inventory == null) return;
        inventory.Clear();
        RefreshSlots();
    }
}
