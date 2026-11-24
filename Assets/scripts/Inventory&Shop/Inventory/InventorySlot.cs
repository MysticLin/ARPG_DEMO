using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class InventorySlot : MonoBehaviour,IPointerClickHandler,IBeginDragHandler,
    IDragHandler,IEndDragHandler
{
    private ItemSO itemSO { get; set; }
    private int quantity { get; set; }
    private int index{ get; set; }
    public UnityEngine.UI.Image itemImage;
    public TMP_Text quantityText;
    private static ShopManager activeShop;
    private InventoryManager inventoryManager;
    private GameObject dragGhost;
    private Canvas dragCanvas;
    private bool wasDragged;
    private int slotIndex => index;
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
        if (wasDragged)
        {
            wasDragged = false;
            return;
        }
        if (quantity <= 0 || inventoryManager == null) return;
        if (activeShop != null)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                activeShop.SellItem(index);
                return;
            }
        }
        if (eventData.button == PointerEventData.InputButton.Left)
            inventoryManager.UseItem(index);
        else if(eventData.button==PointerEventData.InputButton.Right)
            inventoryManager.DropItem(index);
    }
    //拖拽
    public void OnBeginDrag(PointerEventData eventData)
    {
        wasDragged = false;
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        if(quantity<=0||itemSO==null||itemImage==null)
            return;
        if (inventoryManager == null)
            return;
        if (activeShop != null)
            return;
        dragCanvas=GetComponentInParent<Canvas>();
        if (dragCanvas == null) return;
        itemImage.enabled = false;
        dragGhost = new GameObject("DragIcon", typeof(RectTransform),
            typeof(CanvasGroup), typeof(Image));
        dragGhost.transform.SetParent(dragCanvas.transform, false);
        dragGhost.transform.SetAsLastSibling();
        RectTransform ghostRcet=dragGhost.GetComponent<RectTransform>();
        ghostRcet.sizeDelta = itemImage.rectTransform.rect.size;
        Image ghostImage = dragGhost.GetComponent<Image>();
        ghostImage.sprite = itemSO.icon;
        ghostImage.raycastTarget = false;
        CanvasGroup group = dragGhost.GetComponent<CanvasGroup>();
        group.blocksRaycasts = false;
        UpdateGhostPosition(eventData);
    }
   public void OnDrag(PointerEventData eventData)
    {
        if (dragGhost == null) return;
        UpdateGhostPosition(eventData);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        wasDragged = true;
        if (itemImage != null)
            itemImage.enabled = true;
        InventorySlot target = null;
        if(eventData.pointerCurrentRaycast.gameObject!=null)
            target=eventData.pointerCurrentRaycast.gameObject
                .GetComponentInParent<InventorySlot>();
        if (target != null && target != this && inventoryManager != null)
            inventoryManager.MoveItem(index, target.index);
        DestroyGhost();
    }
    private void UpdateGhostPosition(PointerEventData eventData)
    {
        if (dragGhost == null || dragCanvas == null) return;
        RectTransform canvasRect = dragCanvas.transform as RectTransform;
        if (canvasRect == null) return;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, eventData.position, eventData.pressEventCamera,
            out Vector2 localPoint))
            dragGhost.GetComponent<RectTransform>().localPosition = localPoint;
    }
    private void DestroyGhost()
    {
        if (dragGhost != null)
        {
            Destroy(dragGhost);
            dragGhost = null;
        }
        if (itemImage != null)
            itemImage.enabled = true;
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
