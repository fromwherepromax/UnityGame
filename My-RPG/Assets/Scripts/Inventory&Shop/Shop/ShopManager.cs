// 脚本说明：ShopManager 相关逻辑。
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{ 
    [SerializeField] private ShopSlot[] shopSlots; //商店界面上的物品槽数组
    [SerializeField] private InventoryManager inventoryManager; 

    public void PopulateShopItems(List<ShopItem> shopItems)  //根据商店物品列表填充商店界面
    {
        for (int i = 0; i < shopSlots.Length && i < shopItems.Count; i++)
        {
            ShopItem shopItem = shopItems[i];
            shopSlots[i].Initialize(shopItem.itemSo, shopItem.price);
            shopSlots[i].gameObject.SetActive(true);
        }
        for (int i = shopItems.Count; i < shopSlots.Length; i++)
        {
            shopSlots[i].gameObject.SetActive(false);
        }
    }
    public void TryBuyItem(ItemSo itemSo, int price)  //尝试购买物品
    {   
        if (inventoryManager.gold >= price &&itemSo != null && HasSpaceForItem(itemSo))
        {
            inventoryManager.gold -= price;
            inventoryManager.goldText.text = inventoryManager.gold.ToString();
            inventoryManager.AddItem(itemSo, 1);
        }
        else
        {
            Debug.Log("Not enough gold to buy " + itemSo.itemName);
        }
    }
    private bool HasSpaceForItem(ItemSo itemSo)  //检查背包是否有空间放置新物品（考虑堆叠）
    {
        foreach (var slot in inventoryManager.inventorySlots)
        {
            if (slot.itemSo == null || (slot.itemSo == itemSo && slot.quantity < itemSo.stackSize))
            {
                return true;
            }
        }
        return false;
    }

    public void SellItem(ItemSo itemSo){  //尝试出售物品
        if(itemSo==null){
            return;
        }
        foreach(var slot in shopSlots)
        {
            if(slot.itemSo == itemSo)
            {
               inventoryManager.gold += slot.price-1;
               inventoryManager.goldText.text = inventoryManager.gold.ToString();
               return;
            }
        }
    }
}
[System.Serializable]
public class ShopItem  //商店物品类，包含物品信息和价格
{
    public ItemSo itemSo;
    public int price;
}