
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
    private ShopManager activeShop;

    private void Start()
    {   
        inventoryManager = GetComponentInParent<InventoryManager>();
        UpdateUI();
    }
    // private void OnEnable()
    // {
    //     ShopKeeper.OnShopStateChanged += HandleShopStateChanged;
    // }
    // private void OnDisable()
    // {
    //     ShopKeeper.OnShopStateChanged -= HandleShopStateChanged;
    // }
    private void HandleShopStateChanged(ShopManager shopManager, bool isOpen)
    {
        if (isOpen)
        {
            activeShop = shopManager;
        }
        else if (activeShop == shopManager)
        {
            activeShop = null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (quantity > 0)
        {   
            if (eventData.button == PointerEventData.InputButton.Left)
            {   
                if(activeShop != null)
                {
                    activeShop.SellItem(itemSo);
                    quantity--;
                    UpdateUI();
                }
                else{
                    inventoryManager.UseItem(this); 
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                inventoryManager.DropItem(this);
            }
        }
    }

    public void UpdateUI()  //更新背包格子UI显示
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
