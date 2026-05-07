using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDetailsUI : MonoBehaviour
{
    [Header("UI Root")]
    // 右侧详情面板根节点，直接在 Scene 里搭好，脚本只负责驱动显示与刷新
    [SerializeField] private CanvasGroup panel;
    [SerializeField] private RectTransform panelRect;

    [Header("Content")]
    // 面板里的图标、标题、描述、数量和属性文本
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private TMP_Text statText;

    [Header("Buttons")]
    // 使用、丢弃、关闭三个按钮
    [SerializeField] private Button useButton;
    [SerializeField] private Button dropButton;
    [SerializeField] private Button closeButton;

    [Header("Style")]
    // 统一使用 Bangers 字体，并保留按钮文字的上移微调
    [SerializeField] private TMP_FontAsset bangersFont;
    [SerializeField] private Vector2 buttonLabelOffset = new Vector2(0f, 4f);

    private InventoryManager inventoryManager;
    private InventorySlot currentSlot;
    private bool listenersBound;

    public InventorySlot CurrentSlot => currentSlot;

    private void Awake()
    {
        CacheReferences();
        ApplyFont();
        ApplyButtonLabelOffset();
        BindButtons();
        ClearVisuals();
    }

    private void Start()
    {
        // 运行时先隐藏，等背包真正打开时再显示空白面板或选中内容
        HidePanel();
        currentSlot = null;
        ClearVisuals();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            return;
        }

        CacheReferences();
        ApplyFont();
        ApplyButtonLabelOffset();
    }

    public void SelectSlot(InventorySlot slot)
    {
        currentSlot = slot;
        RefreshSelection();
    }

    public void RefreshSelection()
    {
        if (!HasValidSelection())
        {
            currentSlot = null;
            ShowEmptyState();
            return;
        }

        ShowCurrentSelection();
    }

    public void ShowCurrentSelection()
    {
        if (!HasValidSelection())
        {
            currentSlot = null;
            ShowEmptyState();
            return;
        }

        // 选中有效物品时，显示右侧详情
        SetPanelVisible(true);

        ItemSo item = currentSlot.itemSo;

        if (itemIcon != null)
        {
            itemIcon.enabled = item.itemIcon != null;
            itemIcon.sprite = item.itemIcon;
        }

        if (itemNameText != null)
        {
            itemNameText.text = item.itemName;
        }

        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = string.IsNullOrWhiteSpace(item.itemDescription) ? "No description." : item.itemDescription;
        }

        if (quantityText != null)
        {
            quantityText.text = $"x{currentSlot.quantity}";
        }

        if (statText != null)
        {
            statText.text = BuildStats(item);
        }

        SetButtonState(canUseAndDrop: !item.isGold && !item.isExp);
    }

    public void ShowEmptyState()
    {
        // 没有选中物品时，右侧仍显示面板，但内容保持空白
        SetPanelVisible(true);
        ClearVisuals();
        SetButtonState(canUseAndDrop: false);
    }

    public void HidePanel()
    {
        if (panel != null)
        {
            panel.alpha = 0f;
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }
    }

    public void ClearSelection()
    {
        currentSlot = null;
        ClearVisuals();
    }

    public void OnUseClicked()
    {
        if (inventoryManager != null && currentSlot != null)
        {
            inventoryManager.UseItem(currentSlot);
        }
    }

    public void OnDropClicked()
    {
        if (inventoryManager != null && currentSlot != null)
        {
            inventoryManager.DropItem(currentSlot);
        }
    }

    private void CacheReferences()
    {
        if (inventoryManager == null)
        {
            inventoryManager = GetComponentInParent<InventoryManager>();
        }

        if (bangersFont == null)
        {
            bangersFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/Bangers SDF");
        }

        if (panelRect == null)
        {
            panelRect = GetComponent<RectTransform>();
        }

        if (panel == null)
        {
            panel = GetComponent<CanvasGroup>();
        }

        if (panelRect == null)
        {
            return;
        }

        // 场景里已经搭好的 UI 组件，允许通过名字兜底找一遍，方便迁移到别的 Scene
        if (itemIcon == null)
        {
            itemIcon = panelRect.Find("ItemIcon")?.GetComponent<Image>();
        }

        if (itemNameText == null)
        {
            itemNameText = panelRect.Find("Title")?.GetComponent<TextMeshProUGUI>();
        }

        if (itemDescriptionText == null)
        {
            itemDescriptionText = panelRect.Find("Description")?.GetComponent<TextMeshProUGUI>();
        }

        if (quantityText == null)
        {
            quantityText = panelRect.Find("Quantity")?.GetComponent<TextMeshProUGUI>();
        }

        if (statText == null)
        {
            statText = panelRect.Find("Stats")?.GetComponent<TextMeshProUGUI>();
        }

        if (useButton == null)
        {
            useButton = panelRect.Find("UseButton")?.GetComponent<Button>();
        }

        if (dropButton == null)
        {
            dropButton = panelRect.Find("DropButton")?.GetComponent<Button>();
        }

        if (closeButton == null)
        {
            closeButton = panelRect.Find("CloseButton")?.GetComponent<Button>();
        }
    }

    private void BindButtons()
    {
        if (listenersBound)
        {
            return;
        }

        // 重新绑定前先清理，避免重复监听
        if (useButton != null)
        {
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(OnUseClicked);
        }

        if (dropButton != null)
        {
            dropButton.onClick.RemoveAllListeners();
            dropButton.onClick.AddListener(OnDropClicked);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(HidePanel);
        }

        listenersBound = true;
    }

    private void SetPanelVisible(bool visible)
    {
        if (panel == null)
        {
            return;
        }

        panel.alpha = visible ? 1f : 0f;
        panel.interactable = visible;
        panel.blocksRaycasts = visible;
    }

    private void ClearVisuals()
    {
        // 这里清空的是“内容”，不是把面板隐藏，空白面板会在背包打开时展示
        if (itemIcon != null)
        {
            itemIcon.enabled = false;
            itemIcon.sprite = null;
        }

        if (itemNameText != null)
        {
            itemNameText.text = "";
        }

        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = "";
        }

        if (quantityText != null)
        {
            quantityText.text = "";
        }

        if (statText != null)
        {
            statText.text = "";
        }
    }

    private void SetButtonState(bool canUseAndDrop)
    {
        // 空白状态下只保留 Close 可用，Use/Drop 置灰
        if (useButton != null)
        {
            useButton.interactable = canUseAndDrop;
        }

        if (dropButton != null)
        {
            dropButton.interactable = canUseAndDrop;
        }

        if (closeButton != null)
        {
            closeButton.interactable = true;
        }
    }

    private bool HasValidSelection()
    {
        return currentSlot != null && currentSlot.itemSo != null && currentSlot.quantity > 0;
    }

    private string BuildStats(ItemSo item)
    {
        List<string> lines = new List<string>();

        if (item.currentHealth != 0)
        {
            lines.Add($"Health: {item.currentHealth}");
        }

        if (item.maxHealth != 0)
        {
            lines.Add($"Max HP: {item.maxHealth}");
        }

        if (item.damage != 0)
        {
            lines.Add($"Damage: {item.damage}");
        }

        if (item.speed != 0)
        {
            lines.Add($"Speed: {item.speed}");
        }

        return lines.Count == 0 ? "" : string.Join("\n", lines);
    }

    private void ApplyFont()
    {
        // 把背包详情面板中的所有文字统一换成 Bangers
        if (bangersFont == null)
        {
            return;
        }

        ApplyFontToText(itemNameText);
        ApplyFontToText(itemDescriptionText);
        ApplyFontToText(quantityText);
        ApplyFontToText(statText);
        ApplyFontToButton(useButton);
        ApplyFontToButton(dropButton);
        ApplyFontToButton(closeButton);
    }

    private void ApplyFontToText(TMP_Text text)
    {
        if (text != null)
        {
            text.font = bangersFont;
        }
    }

    private void ApplyFontToButton(Button button)
    {
        if (button == null)
        {
            return;
        }

        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
        {
            label.font = bangersFont;
        }
    }

    private void ApplyButtonLabelOffset()
    {
        // 让按钮文字稍微往上提一点，避免 Bangers 字体看起来偏低
        ApplyButtonLabelOffset(useButton);
        ApplyButtonLabelOffset(dropButton);
        ApplyButtonLabelOffset(closeButton);
    }

    private void ApplyButtonLabelOffset(Button button)
    {
        if (button == null)
        {
            return;
        }

        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label == null)
        {
            return;
        }

        label.rectTransform.anchoredPosition = buttonLabelOffset;
    }
}
