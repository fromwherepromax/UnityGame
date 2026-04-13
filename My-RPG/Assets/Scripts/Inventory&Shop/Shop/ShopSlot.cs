// 脚本说明：ShopSlot 相关逻辑。
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public ItemSo itemSo;
    public TMP_Text itemNameText;
    public TMP_Text PriceText;
    public Image itemImage;
    public int price;
    // [SerializeField] private ShopManager shopManager;
    // [SerializeField] private ShopInfo shopInfo;

     
    public void Initialize(ItemSo newitemSo, int Price)
    {
        itemSo = newitemSo;
        itemImage.sprite = itemSo.itemIcon;
        itemNameText.text = itemSo.itemName;
        price = Price;
        PriceText.text = price.ToString();
    }
    // public void OnBuyButtonClicked()
    // {
    //    shopManager.TryBuyItem(itemSo, price);
    // }

    // public void OnPointerEnter(PointerEventData eventData)
    // {   
    //     if(itemSo != null)
    //     shopInfo.ShowItemInfo(itemSo);
    // }

    // public void OnPointerExit(PointerEventData eventData)
    // {   
    //     if(itemSo != null)
    //     shopInfo.HideItemInfo();
    // }

    // public void OnPointerMove(PointerEventData eventData)
    // {   
    //     if(itemSo != null){
    //         shopInfo.FollowMouse();
    //     }
    // }
}
