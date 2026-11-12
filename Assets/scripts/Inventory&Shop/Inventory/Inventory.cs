
using System;

public readonly struct InventorySlotView
{
    public ItemSO itemSO { get; }
    public int Quantity { get; }
    public InventorySlotView(ItemSO itemSO, int quantity)
    {
        this.itemSO = itemSO;
        Quantity = quantity;
    }
    public bool IsEmpty => itemSO == null || Quantity <= 0;
}
public class Inventory
{
   private class Slot
    {
        public ItemSO itemSO;
        public int Quantity;
    }
    private readonly Slot[] slots;
    public event Action Changed;
    public  Inventory(int slotCount)
    {
        if (slotCount < 0) slotCount = 0;
        slots = new Slot[slotCount];
        for(int i = 0; i < slotCount; i++)
        {
            slots[i] = new Slot();
        }
    }
    public int SlotCount => slots.Length;
    public InventorySlotView GetSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return default;
        Slot s=slots[index];
        return new InventorySlotView(s.itemSO, s.Quantity);
    }
    public int Add(ItemSO itemSO,int Quantity)
    {
        if (itemSO == null || Quantity <= 0) return 0;
        int stackSize = Math.Max(1, itemSO.stackSize);//这个物品的最大堆叠上限
        int remaining = Quantity;//现在还剩多少数量
        for(int i = 0; i < slots.Length && remaining > 0; i++)
        {
            Slot s= slots[i];
            if (s.itemSO != itemSO || s.Quantity >= stackSize) continue;
            int add = Math.Min(stackSize - s.Quantity, remaining);
            s.Quantity += add;
            remaining -= add;
        }
        for(int i = 0; i < slots.Length&&remaining>0; i++)
        {
            Slot s= slots[i];
            if (s.itemSO != null && s.Quantity > 0) continue;
            int add = Math.Min(stackSize, remaining);
            s.itemSO= itemSO;
            s.Quantity = add;
            remaining-= add;
        }
        if (remaining != Quantity)
            Changed?.Invoke();
        return remaining;
    }
    public int Remove(ItemSO itemSO,int quantity)
    {
        if (itemSO == null || quantity <= 0) return 0;
        int removed = 0;
        for(int i=0; i<slots.Length&&removed < quantity; i++)
        {
            Slot s = slots[i];
            if (s.itemSO != itemSO || s.Quantity <= 0) continue;
            int take = Math.Min(s.Quantity, quantity - removed);
            s.Quantity -= take;
            removed += take;
            if (s.Quantity <= 0)
            {
                s.Quantity= 0;
                s.itemSO = null;
            }
        }
        if(removed>0)
            Changed?.Invoke();
        return removed; 
    }
    public int RemoveAt(int index,int quantity)
    {
        if (index < 0 || index >= slots.Length || quantity <= 0) return 0;
        Slot s = slots[index];
        if (s.itemSO == null || s.Quantity <= 0) return 0;
        int take = Math.Min(s.Quantity, quantity);
        s.Quantity -= take;
        if (s.Quantity <= 0)
        {
            s.Quantity= 0;
            s.itemSO = null;
        }
        if(take>0)
            Changed?.Invoke();
        return take;
    }
    public int Count(ItemSO itemSO)
    {
        if(itemSO==null) return 0;
        int total = 0;
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemSO == itemSO)
                total += slots[i].Quantity;
        }
        return total;
    }
    public bool Has(ItemSO itemSO,int quantity = 1)
    {
        if (itemSO == null || quantity <= 0) return false;
        return Count(itemSO) >= quantity;
    }
    public bool CanAdd(ItemSO itemSO,int quantity = 1)
    {
        if (itemSO == null || quantity <= 0) return false;
        int stackSize = Math.Max(1, itemSO.stackSize);
        int capacity = 0;
        for(int i = 0; i < slots.Length; i++)
        {
            Slot s=slots[i];
            if (s.itemSO == itemSO && s.Quantity < stackSize)
                capacity += stackSize - s.Quantity;
            else if (s.itemSO == null || s.Quantity <= 0)
                capacity += stackSize;
        }
        return capacity >= quantity;
    }
}
