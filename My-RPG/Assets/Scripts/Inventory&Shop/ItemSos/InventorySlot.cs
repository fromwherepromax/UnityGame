using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public ItemSo itemSo;
    public int quantity;
    public Image itemImage;
    public TMP_Text quantityText;

    // 槽位点击后需要把自己传给右侧详情面板
    private InventoryDetailsUI detailsUI;

    private void Start()
    {
        InventoryManager inventoryManager = GetComponentInParent<InventoryManager>();
        if (inventoryManager != null)
        {
            detailsUI = inventoryManager.GetComponentInChildren<InventoryDetailsUI>(true);
        }

        UpdateUI();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SelectSlot();
        }
    }

    private void SelectSlot()
    {
        if (quantity <= 0 || itemSo == null || detailsUI == null)
        {
            return;
        }

        detailsUI.SelectSlot(this);
    }

    public void UpdateUI()
    {
        if (quantity <= 0)
        {
            itemSo = null;
        }

        if (itemSo != null)
        {
            itemImage.sprite = itemSo.itemIcon;
            itemImage.gameObject.SetActive(true);
            quantityText.text = quantity.ToString();
        }
        else
        {
            itemImage.gameObject.SetActive(false);
            quantityText.text = "";
        }
    }
}
