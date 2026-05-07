using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlots;
    public static InventoryManager Instance;

    public int gold;
    public TMP_Text goldText;
    public GameObject lootPrefab;
    public Transform player;
    public CanvasGroup canvasGroup;
    public UseItem useItem;
    [SerializeField] private KeyCode toggleInventoryKey = KeyCode.B;
    [SerializeField] private InventoryDetailsUI inventoryDetailsUI;

    public static event Action<int> OnExpGained;
    private bool isInventoryOpen = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (inventoryDetailsUI == null)
        {
            inventoryDetailsUI = GetComponentInChildren<InventoryDetailsUI>(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleInventoryKey))
        {
            ToggleInventory();
        }
    }

    private void Start()
    {
        isInventoryOpen = canvasGroup != null && canvasGroup.alpha > 0f;

        if (canvasGroup != null)
        {
            SetInventoryVisible(isInventoryOpen);
        }

        foreach (var slot in inventorySlots)
        {
            slot.UpdateUI();
        }

        inventoryDetailsUI?.RefreshSelection();
    }

    public void ToggleInventory()
    {
        SetInventoryVisible(!isInventoryOpen);
    }

    public void SetInventoryVisible(bool isVisible)
    {
        isInventoryOpen = isVisible;

        if (canvasGroup == null)
        {
            Debug.LogWarning("InventoryManager: canvasGroup is not assigned.");
            return;
        }

        canvasGroup.alpha = isVisible ? 1f : 0f;
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;

        if (inventoryDetailsUI != null)
        {
            if (isVisible)
            {
                inventoryDetailsUI.ClearSelection();
                inventoryDetailsUI.RefreshSelection();
            }
            else
            {
                inventoryDetailsUI.HidePanel();
            }
        }

        if (!isVisible && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void OnEnable()
    {
        Loot.OnItemLooted += AddItem;
    }

    private void OnDisable()
    {
        Loot.OnItemLooted -= AddItem;
    }

    public void AddItem(ItemSo itemSo, int quantity)
    {
        if (itemSo == null || quantity <= 0)
        {
            return;
        }

        if (itemSo.isGold)
        {
            gold += quantity;
            if (goldText != null)
            {
                goldText.text = gold.ToString();
            }
            return;
        }

        if (itemSo.isExp)
        {
            OnExpGained?.Invoke(quantity);
            return;
        }

        foreach (var slot in inventorySlots)
        {
            if (slot.itemSo == itemSo && slot.quantity < itemSo.stackSize)
            {
                int availableSpace = itemSo.stackSize - slot.quantity;
                int quantityToAdd = Mathf.Min(quantity, availableSpace);
                slot.quantity += quantityToAdd;
                quantity -= quantityToAdd;
                slot.UpdateUI();

                if (quantity <= 0)
                {
                    inventoryDetailsUI?.RefreshSelection();
                    return;
                }
            }
        }

        foreach (var slot in inventorySlots)
        {
            if (slot.itemSo == null)
            {
                int amountToAdd = Mathf.Min(quantity, itemSo.stackSize);
                slot.itemSo = itemSo;
                slot.quantity = amountToAdd;
                slot.UpdateUI();
                inventoryDetailsUI?.RefreshSelection();
                return;
            }
        }

        if (quantity > 0)
        {
            Debug.Log("Inventory is full! Could not add all items.");
            DropLoot(itemSo, quantity);
        }

        inventoryDetailsUI?.RefreshSelection();
    }

    public void DropItem(InventorySlot slot)
    {
        if (slot == null || slot.itemSo == null || slot.quantity <= 0)
        {
            return;
        }

        DropLoot(slot.itemSo, 1);
        slot.quantity--;

        if (slot.quantity <= 0)
        {
            slot.itemSo = null;
        }

        slot.UpdateUI();
        inventoryDetailsUI?.RefreshSelection();
    }

    private void DropLoot(ItemSo itemSo, int quantity)
    {
        if (itemSo == null || lootPrefab == null || player == null)
        {
            return;
        }

        Loot loot = Instantiate(lootPrefab, player.position, Quaternion.identity).GetComponent<Loot>();
        loot.Initialize(itemSo, quantity);
    }

    public void UseItem(InventorySlot slot)
    {
        if (slot == null || slot.itemSo == null || slot.quantity <= 0)
        {
            return;
        }

        useItem.ApplyItemEffect(slot.itemSo);
        slot.quantity--;

        if (slot.quantity <= 0)
        {
            slot.itemSo = null;
        }

        slot.UpdateUI();
        inventoryDetailsUI?.RefreshSelection();
    }

    public bool HasItem(ItemSo itemSo)
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.itemSo == itemSo && slot.quantity > 0)
            {
                return true;
            }
        }

        return false;
    }

    public int GetItemQuantity(ItemSo itemSo)
    {
        int totalQuantity = 0;

        foreach (var slot in inventorySlots)
        {
            if (slot.itemSo == itemSo)
            {
                totalQuantity += slot.quantity;
            }
        }

        return totalQuantity;
    }

    public void RemoveItem(ItemSo itemSo, int quantity)
    {
        if (itemSo == null || quantity <= 0)
        {
            return;
        }

        for (int i = 0; i < inventorySlots.Length && quantity > 0; i++)
        {
            var slot = inventorySlots[i];
            if (slot.itemSo != itemSo)
            {
                continue;
            }

            if (slot.quantity >= quantity)
            {
                slot.quantity -= quantity;
                if (slot.quantity <= 0)
                {
                    slot.itemSo = null;
                }
                slot.UpdateUI();
                break;
            }

            quantity -= slot.quantity;
            slot.itemSo = null;
            slot.quantity = 0;
            slot.UpdateUI();
        }

        inventoryDetailsUI?.RefreshSelection();
    }
}
