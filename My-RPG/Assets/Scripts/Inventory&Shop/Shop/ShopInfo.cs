// 脚本说明：ShopInfo 相关逻辑。
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopInfo : MonoBehaviour
{
    public CanvasGroup Infopanel;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;

    [Header("Stats Fields")]
    public TMP_Text[] statText;

    private RectTransform infoPanelRect;

    private void Awake()
    {
        infoPanelRect = Infopanel.GetComponent<RectTransform>();
    }
    public void ShowItemInfo(ItemSo itemSo)
    {
        if (itemSo == null)
        {
            HideItemInfo();
            return;
        }

        Infopanel.alpha = 1;
        Infopanel.interactable = true;
        Infopanel.blocksRaycasts = true;
        itemNameText.text = itemSo.itemName;
        itemDescriptionText.text = itemSo.itemDescription;

        for (int i = 0; i < statText.Length; i++)
        {
            statText[i].text = "";
            statText[i].gameObject.SetActive(false);
        }

        List<string> stats = new List<string>();
        if(itemSo.currentHealth > 0)
        {
            stats.Add($"Health: {itemSo.currentHealth}");
        }
        if(itemSo.maxHealth > 0)
        {
            stats.Add($"MaxHP: {itemSo.maxHealth}");
        }
        if(itemSo.damage > 0)
        {
            stats.Add($"Damage: {itemSo.damage}");
        }
        if(itemSo.speed > 0)
        {
            stats.Add($"Speed: {itemSo.speed}");
        }

        for(int i = 0; i < statText.Length; i++)
        {
            if(i < stats.Count)
            {
                statText[i].text = stats[i];
                statText[i].gameObject.SetActive(true);
            }
            else
            {
                statText[i].gameObject.SetActive(false);
            }
        }

    }
    public void HideItemInfo()
    {
        Infopanel.alpha = 0;
        itemNameText.text = "";
        itemDescriptionText.text = "";
        Infopanel.interactable = false;
        Infopanel.blocksRaycasts = false;
    }
    public void FollowMouse()
    {
        Vector3 mousePosition=Input.mousePosition;
        Vector3 offset = new Vector3(10, -10, 0); //调整偏移量以适应UI布局
        infoPanelRect.position = mousePosition + offset;
    }
}
