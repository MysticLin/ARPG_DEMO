using UnityEngine;

public class ShopButtonToggle : MonoBehaviour
{
  public void OpenItemShop()
    {
        if (ShopKeeper.currentShopkeeper != null)
        {
            ShopKeeper.currentShopkeeper.OpenItemShop();
        }
    }
    public void OpenWeaponShop()
    {
        if (ShopKeeper.currentShopkeeper != null)
        {
            ShopKeeper.currentShopkeeper.OpenWeaponShop();
        }
    }
    public void OpenArmourShop()
    {
        if (ShopKeeper.currentShopkeeper != null)
        {
            ShopKeeper.currentShopkeeper.OpenArmourShop();
        }
    }
}
