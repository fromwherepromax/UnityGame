// 脚本说明：InventoryManager 相关逻辑。
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class InventoryManager : MonoBehaviour
{   
    public InventorySlot[] inventorySlots;
    public static InventoryManager Instance; //单例实例
    public int gold;
    public TMP_Text goldText;
    public GameObject lootPrefab;
    public Transform player;
    public CanvasGroup canvasGroup; //用于控制UI显示的CanvasGroup组件
    public UseItem useItem; //用于应用物品效果的组件
    private bool isInventoryOpen=false; //当前背包UI是否打开
    [SerializeField] private KeyCode toggleInventoryKey = KeyCode.B; //切换背包的快捷键

    public static event Action<int> OnExpGained; //经验增加事件
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; //设置单例实例
        }
        else
        {
            Destroy(gameObject); //如果已经存在实例，销毁当前对象
        }
    }
    private void Update()  //监听背包切换按键
    {
        if (Input.GetKeyDown(toggleInventoryKey))
        {
            ToggleInventory();
        }
    }

    private void Start() //初始化背包UI状态
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
        if (itemSo.isGold)  //如果是金钱，直接增加数量并更新UI
        {
            gold += quantity;
            goldText.text = gold.ToString();
            return;
        }
        if (itemSo.isExp)
        {
            OnExpGained?.Invoke(quantity); //触发经验增加事件
            return;
        }
        foreach (var slot in inventorySlots)  //先检查是否已有该物品，若有则叠加数量
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
                return;
            }
        }
        if (quantity > 0)
        {
            Debug.Log("Inventory is full! Could not add all items.");
            DropLoot(itemSo, quantity);
        }
    }
    public void DropItem(InventorySlot slot) //丢弃物品
    {
        DropLoot(slot.itemSo, 1);
        slot.quantity--;
        if (slot.quantity <= 0)
        {
            slot.itemSo = null;
        }
        slot.UpdateUI();
    }
    private void DropLoot(ItemSo itemSo, int quantity)  //在玩家位置生成掉落物
    {
        Debug.Log($"Dropped {quantity} of {itemSo.itemName} at player's position.");
        Loot loot = Instantiate(lootPrefab, player.position, Quaternion.identity).GetComponent<Loot>();
        loot.Initialize(itemSo, quantity);
    }

    public void UseItem(InventorySlot slot)
    {
        if (slot.itemSo != null && slot.quantity > 0)
        {
            useItem.ApplyItemEffect(slot.itemSo);
            slot.quantity--;
            if (slot.quantity <= 0)
            {
                slot.itemSo = null;
            }
            slot.UpdateUI();
        }
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
}
