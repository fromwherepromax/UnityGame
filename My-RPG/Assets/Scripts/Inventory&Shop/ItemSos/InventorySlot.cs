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

    private InventoryDetailsUI detailsUI;

    private void Start()
    {
        detailsUI = GetComponentInParent<InventoryDetailsUI>();
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
