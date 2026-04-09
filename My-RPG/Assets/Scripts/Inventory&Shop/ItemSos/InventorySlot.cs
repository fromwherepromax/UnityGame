
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour,IPointerClickHandler
{   
    public ItemSo itemSo;
    public int quantity;
    public Image itemImage;
    public TMP_Text quantityText;
    private InventoryManager inventoryManager;

    private void Start()
    {   
        inventoryManager = GetComponentInParent<InventoryManager>();
        UpdateUI();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (quantity > 0)
        {   
            if (eventData.button == PointerEventData.InputButton.Left)
            {   
                // if(activeShop != null)
                // {
                //     activeShop.SellItem(itemSo);
                //     quantity--;
                //     UpdateUI();
                // }
                // else{
                    inventoryManager.UseItem(this); 
                // }
            }
            // else if (eventData.button == PointerEventData.InputButton.Right)
            // {
            //     inventoryManager.DropItem(this);
            // }
        }
    }

    public void UpdateUI()
    {   
        if(quantity <= 0)
        {
            itemSo = null;
        }
        if(itemSo != null)
        {
           itemImage.sprite = itemSo.itemIcon;
           itemImage.gameObject.SetActive(true);
           quantityText.text =quantity.ToString();
        }
        else
        {
            itemImage.gameObject.SetActive(false);
            quantityText.text = "";
        }
    }
}
