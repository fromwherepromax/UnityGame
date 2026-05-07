using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDetailsUI : MonoBehaviour
{
    [Header("UI Root")]
    [SerializeField] private CanvasGroup panel;
    [SerializeField] private RectTransform panelRect;

    [Header("Content")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private TMP_Text statText;

    [Header("Buttons")]
    [SerializeField] private Button useButton;
    [SerializeField] private Button dropButton;
    [SerializeField] private Button closeButton;

    [Header("Style")]
    [SerializeField] private TMP_FontAsset bangersFont;
    [SerializeField] private Sprite panelSprite;
    [SerializeField] private Sprite buttonSprite;
    [SerializeField] private Vector2 buttonLabelOffset = new Vector2(0f, 4f);

    private InventoryManager inventoryManager;
    private InventorySlot currentSlot;

    public InventorySlot CurrentSlot => currentSlot;

    private void Awake()
    {
        inventoryManager = GetComponent<InventoryManager>();
        if (inventoryManager == null)
        {
            inventoryManager = GetComponentInParent<InventoryManager>();
        }

        if (bangersFont == null)
        {
            bangersFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/Bangers SDF");
        }

        CreatePanelIfNeeded();
        ApplyFont();

        if (useButton != null)
        {
            useButton.onClick.AddListener(OnUseClicked);
        }

        if (dropButton != null)
        {
            dropButton.onClick.AddListener(OnDropClicked);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HidePanel);
        }
    }

    private void Start()
    {
        HidePanel();
    }

    public void SelectSlot(InventorySlot slot)
    {
        currentSlot = slot;
        ShowCurrentSelection();
    }

    public void ShowCurrentSelection()
    {
        if (currentSlot == null || currentSlot.itemSo == null || currentSlot.quantity <= 0)
        {
            HidePanel();
            return;
        }

        if (panel != null)
        {
            panel.alpha = 1f;
            panel.interactable = true;
            panel.blocksRaycasts = true;
        }

        var item = currentSlot.itemSo;

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

        if (useButton != null)
        {
            useButton.interactable = !item.isGold && !item.isExp;
        }

        if (dropButton != null)
        {
            dropButton.interactable = !item.isGold && !item.isExp;
        }
    }

    public void RefreshSelection()
    {
        if (currentSlot == null || currentSlot.itemSo == null || currentSlot.quantity <= 0)
        {
            HidePanel();
            return;
        }

        ShowCurrentSelection();
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

    private string BuildStats(ItemSo item)
    {
        var lines = new System.Collections.Generic.List<string>();

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
        if (bangersFont == null)
        {
            return;
        }

        if (itemNameText != null)
        {
            itemNameText.font = bangersFont;
        }

        if (itemDescriptionText != null)
        {
            itemDescriptionText.font = bangersFont;
        }

        if (quantityText != null)
        {
            quantityText.font = bangersFont;
        }

        if (statText != null)
        {
            statText.font = bangersFont;
        }
    }

    private void CreatePanelIfNeeded()
    {
        if (panel != null)
        {
            return;
        }

        GameObject root = new GameObject("InventoryDetailsPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
        root.transform.SetParent(transform, false);

        panelRect = root.GetComponent<RectTransform>();
        panel = root.GetComponent<CanvasGroup>();

        panelRect.anchorMin = new Vector2(1f, 0.5f);
        panelRect.anchorMax = new Vector2(1f, 0.5f);
        panelRect.pivot = new Vector2(1f, 0.5f);
        panelRect.sizeDelta = new Vector2(460f, 760f);
        panelRect.anchoredPosition = new Vector2(-40f, 0f);

        var bg = root.GetComponent<Image>();
        bg.sprite = panelSprite != null ? panelSprite : Resources.GetBuiltinResource<Sprite>("UI/Skin/Background.psd");
        bg.type = Image.Type.Sliced;
        bg.color = new Color(0.13f, 0.14f, 0.16f, 0.95f);

        CreateContent(root.transform);
    }

    private void CreateContent(Transform root)
    {
        GameObject title = CreateText(root, "Title", "Item", 42, new Vector2(0.5f, 0.92f), new Vector2(0.5f, 0.92f), new Vector2(0.5f, 0.5f), new Vector2(380f, 60f));
        itemNameText = title.GetComponent<TMP_Text>();

        GameObject icon = new GameObject("ItemIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        icon.transform.SetParent(root, false);
        var iconRect = icon.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.5f, 0.76f);
        iconRect.anchorMax = new Vector2(0.5f, 0.76f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.sizeDelta = new Vector2(180f, 180f);
        iconRect.anchoredPosition = Vector2.zero;
        itemIcon = icon.GetComponent<Image>();

        GameObject qty = CreateText(root, "Quantity", "x0", 28, new Vector2(0.5f, 0.62f), new Vector2(0.5f, 0.62f), new Vector2(0.5f, 0.5f), new Vector2(180f, 40f));
        quantityText = qty.GetComponent<TMP_Text>();

        GameObject desc = CreateText(root, "Description", "Description", 24, new Vector2(0.5f, 0.49f), new Vector2(0.5f, 0.49f), new Vector2(0.5f, 0.5f), new Vector2(380f, 190f));
        itemDescriptionText = desc.GetComponent<TMP_Text>();

        GameObject stats = CreateText(root, "Stats", "", 22, new Vector2(0.5f, 0.26f), new Vector2(0.5f, 0.26f), new Vector2(0.5f, 0.5f), new Vector2(360f, 120f));
        statText = stats.GetComponent<TMP_Text>();

        useButton = CreateButton(root, "UseButton", "Use", new Vector2(0.27f, 0.08f));
        dropButton = CreateButton(root, "DropButton", "Drop", new Vector2(0.50f, 0.08f));
        closeButton = CreateButton(root, "CloseButton", "Close", new Vector2(0.73f, 0.08f));
    }

    private GameObject CreateText(Transform parent, string name, string text, int fontSize, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;

        var label = go.GetComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = fontSize;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        label.enableWordWrapping = true;
        label.overflowMode = TextOverflowModes.Overflow;
        label.font = bangersFont;

        return go;
    }

    private Button CreateButton(Transform parent, string name, string text, Vector2 anchor)
    {
        // 按钮文字单独往上提一点，避免 Bangers 字体视觉上偏低
        GameObject buttonRoot = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonRoot.transform.SetParent(parent, false);

        var rect = buttonRoot.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(110f, 50f);
        rect.anchoredPosition = Vector2.zero;

        var img = buttonRoot.GetComponent<Image>();
        img.sprite = buttonSprite != null ? buttonSprite : Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        img.type = Image.Type.Sliced;
        img.color = new Color(0.25f, 0.45f, 0.85f, 1f);

        GameObject labelObj = CreateText(buttonRoot.transform, "Text", text, 22, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero);
        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchoredPosition = new Vector2(buttonLabelOffset.x, buttonLabelOffset.y);

        return buttonRoot.GetComponent<Button>();
    }
}
