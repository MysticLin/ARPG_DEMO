using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    public static ShopKeeper currentShopkeeper;
    public Animator anim;
    public UIWindow shopWindow;
    public CanvasGroup ShopCanvasGroup;
    public ShopManager shopManager;
    public static event Action<ShopManager, bool> OnShopStateChanged;
    [SerializeField] private List<ShopItems> shopItems;
    [SerializeField] private List<ShopItems> shopWeapons;
    [SerializeField] private List<ShopItems> shopArmour;
    [SerializeField] private Camera shopkeeperCam;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0, -1);

    private bool playerInRange;
    private bool isShopOpen;
    private void Start()
    {
        if (shopManager == null)
            shopManager = ShopManager.Instance;
        if (shopManager == null)
            return;
        if (ShopCanvasGroup == null)
        {
            ShopCanvasGroup = shopManager.GetComponentInParent<CanvasGroup>();
            if (ShopCanvasGroup == null)
                ShopCanvasGroup = shopManager.GetComponentInChildren<CanvasGroup>(true);
        }
        if (shopkeeperCam == null)
            shopkeeperCam = shopManager.GetComponentInChildren<Camera>(true);
        if (shopWindow == null && ShopCanvasGroup != null)
        {
            shopWindow = ShopCanvasGroup.GetComponent<UIWindow>();
            if (shopWindow == null)
                shopWindow = ShopCanvasGroup.GetComponentInParent<UIWindow>();
        }
        if (shopWindow != null)
            shopWindow.OnHidden += HandleShopWindowHidden;
    }
    private void OnDisable()
    {
        if (shopWindow != null)
            shopWindow.OnHidden -= HandleShopWindowHidden;
        if (isShopOpen)
        {
            if (UIWindowManager.Instance != null)
                UIWindowManager.Instance.Close(shopWindow);
            else
                HandleShopWindowHidden();
        }
    }

    void Update()
    {
        if (QuestLogUI.IsOpen) return;
        if (DialogueManager.IsDialogueActive) return;
        if (shopManager == null || ShopCanvasGroup == null) return;
        if (shopWindow == null) return;
        if (!playerInRange) return;
        if (!Input.GetButtonDown("Interact")) return;
        if (isShopOpen)
        {
            UIWindowManager.Instance?.Toggle(shopWindow);
            return;
        }
        OpenShop();
    }
    private void OpenShop()
    {
        if (shopWindow == null) return;
        if (isShopOpen) return;
        isShopOpen = true;
        currentShopkeeper = this;
        UIWindowManager.Instance?.Open(shopWindow);
        OnShopStateChanged?.Invoke(shopManager, true);
        if (shopkeeperCam != null)
        {
            shopkeeperCam.transform.position = transform.position + cameraOffset;
            shopkeeperCam.gameObject.SetActive(true);
        }
        OpenItemShop();
    }
    private void CloseShop()
    {
        if (!isShopOpen) return;
        UIWindowManager.Instance?.Close(shopWindow);
    }
    private void HandleShopWindowHidden()
    {
        isShopOpen=false;
        currentShopkeeper=null;
        OnShopStateChanged?.Invoke(shopManager, false);
        if (shopkeeperCam != null)
            shopkeeperCam.gameObject.SetActive(false);
    }
    public void OpenItemShop()
    {
        shopManager.PopulateShopItems(shopItems);
    }
    public void OpenWeaponShop()
    {
        shopManager.PopulateShopItems(shopWeapons);
    }
    public void OpenArmourShop()
    {
        shopManager.PopulateShopItems(shopArmour);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("playerInRange", true);
            playerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("playerInRange", false);
            playerInRange = false;
        }
    }
}
