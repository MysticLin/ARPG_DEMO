using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour,IPointerClickHandler
{
    private ItemSO itemSO { get; set; }
    private int quantity { get; set; }
    private int index{ get; set; }
    public Image itemImage;
    public TMP_Text quantityText;
    private InventoryManager inventoryManager;
    private static ShopManager activeShop;
    private void Start()
    {
        inventoryManager = GetComponentInParent<InventoryManager>();
    }
    private void OnEnable()
    {
        ShopKeeper.OnShopStateChanged += HandleShopStateChanged;
    }
    private void OnDisable()
    {
        ShopKeeper.OnShopStateChanged -= HandleShopStateChanged;
    }
    private void HandleShopStateChanged(ShopManager shopManager,bool isOpen)
    {
        activeShop=isOpen?shopManager:null;
    }
    public void Bind(int index)
    {
        this.index = index;
    }
    public void Set(ItemSO itemSO,int quantity)
    {
       this.itemSO = itemSO;
        this.quantity = quantity;
        UpdateUI();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (quantity <= 0 || inventoryManager == null) return;
        if (activeShop != null)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                    activeShop.SellItem(index);
                return;
            }
        }
        if(eventData.button==PointerEventData.InputButton.Left)
            inventoryManager.UseItem(index);
        else if(eventData.button==PointerEventData.InputButton.Right)
            inventoryManager.DropItem(index);
    }
    public void UpdateUI()
    {
        if (quantity <= 0)
        {
            itemSO = null;
            quantity = 0;
        }
        if(itemSO != null)
        {
            if (itemImage!=null)
            {
                itemImage.sprite = itemSO.icon;
                itemImage.gameObject.SetActive(true);
            }
            if (quantityText != null)
                quantityText.text = quantity.ToString();
        }
        else
        {
            if (itemImage != null)
                itemImage.gameObject.SetActive(false);
            if (quantityText != null)
                quantityText.text = "";
        }
    }
}
