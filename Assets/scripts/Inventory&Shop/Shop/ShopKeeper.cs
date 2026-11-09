using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    public static ShopKeeper currentShopkeeper;
    public Animator anim;
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


    void Update()
    {
        if (playerInRange)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (!isShopOpen)
                {
                    Time.timeScale = 0;
                    currentShopkeeper = this;
                    isShopOpen = true;
                    ShopCanvasGroup.alpha = 1;
                    OnShopStateChanged?.Invoke(shopManager, true);
                    ShopCanvasGroup.blocksRaycasts = true;
                    ShopCanvasGroup.interactable = true;

                    shopkeeperCam.transform.position = transform.position + cameraOffset;
                    shopkeeperCam.gameObject.SetActive(true);
                    OpenItemShop();
                }
                else
                {
                    Time.timeScale = 1;
                    isShopOpen = false;
                    currentShopkeeper = null;
                    ShopCanvasGroup.alpha = 0;
                    OnShopStateChanged?.Invoke(shopManager,false);
                    ShopCanvasGroup.blocksRaycasts = false;
                    shopkeeperCam.gameObject.SetActive(false);
                    ShopCanvasGroup.interactable = false;
                }
               
            }
        }
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
