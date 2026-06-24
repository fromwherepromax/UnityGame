using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// UI 管理器（单例）—— 统一管理所有面板的显示/隐藏和互斥逻辑。
/// 建议挂载到与 GameManager 相同的持久化 GameObject 上，并将该对象加入 GameManager 的 persistentObjects 数组。
/// </summary>
[DefaultExecutionOrder(-100)]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    // ═══════════════════════ 面板 CanvasGroup 引用 ═══════════════════════
    [Header("面板 CanvasGroup 引用")]
    [Tooltip("设置面板的 CanvasGroup")]
    public CanvasGroup settingsPanel;
    [Tooltip("背包面板的 CanvasGroup")]
    public CanvasGroup inventoryPanel;
    [Tooltip("属性面板的 CanvasGroup")]
    public CanvasGroup statsPanel;
    [Tooltip("任务面板的 CanvasGroup")]
    public CanvasGroup questPanel;
    [Tooltip("商店面板的 CanvasGroup")]
    public CanvasGroup shopPanel;
    [Tooltip("对话面板的 CanvasGroup")]
    public CanvasGroup dialoguePanel;

    // ═══════════════════════ 快捷键设置 ═══════════════════════
    [Header("快捷键设置")]
    [Tooltip("任务日志快捷键")]
    public KeyCode questLogKey = KeyCode.J;
    [Tooltip("属性面板快捷键")]
    public KeyCode statsKey = KeyCode.C;

    // ═══════════════════════ 内部状态 ═══════════════════════
    private Dictionary<UIPanelType, CanvasGroup> panelMap;
    private HashSet<UIPanelType> openPanels = new HashSet<UIPanelType>();

    // ═══════════════════════ 事件 ═══════════════════════
    /// <summary>面板被打开时触发，参数为面板类型</summary>
    public event Action<UIPanelType> OnPanelOpened;
    /// <summary>面板被关闭时触发，参数为面板类型</summary>
    public event Action<UIPanelType> OnPanelClosed;

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

        panelMap = new Dictionary<UIPanelType, CanvasGroup>
        {
            { UIPanelType.Settings,  settingsPanel  },
            { UIPanelType.Inventory, inventoryPanel },
            { UIPanelType.Stats,     statsPanel     },
            { UIPanelType.Quest,     questPanel     },
            { UIPanelType.Shop,      shopPanel      },
            { UIPanelType.Dialogue,  dialoguePanel  }
        };

        // 启动时隐藏所有面板
        foreach (var kvp in panelMap)
        {
            if (kvp.Value != null)
                SetCanvasGroupVisible(kvp.Value, false);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RebindAllPanels();
    }

    /// <summary>
    /// 场景加载后重新绑定所有面板的 CanvasGroup 引用（场景重载后原引用已失效）
    /// </summary>
    private void RebindAllPanels()
    {
        openPanels.Clear();

        UIPanelIdentifier[] identifiers = FindObjectsOfType<UIPanelIdentifier>(true);
        foreach (UIPanelIdentifier id in identifiers)
        {
            CanvasGroup cg = id.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                panelMap[id.panelType] = cg;

                // 同步到序列化字段，方便 Inspector 查看
                switch (id.panelType)
                {
                    case UIPanelType.Settings:  settingsPanel  = cg; break;
                    case UIPanelType.Inventory: inventoryPanel = cg; break;
                    case UIPanelType.Stats:     statsPanel     = cg; break;
                    case UIPanelType.Quest:     questPanel     = cg; break;
                    case UIPanelType.Shop:      shopPanel      = cg; break;
                    case UIPanelType.Dialogue:  dialoguePanel  = cg; break;
                }
            }
        }

        // 确保所有面板初始为隐藏状态
        foreach (var kvp in panelMap)
        {
            if (kvp.Value != null)
                SetCanvasGroupVisible(kvp.Value, false);
        }

        Debug.Log($"[UIManager] 面板重新绑定完成，共绑定 {identifiers.Length} 个面板。");
    }

    private void Update()
    {
        // J 键切换任务日志
        if (Input.GetKeyDown(questLogKey))
        {
            TogglePanel(UIPanelType.Quest);
        }

        // C 键切换属性面板
        if (Input.GetKeyDown(statsKey))
        {
            TogglePanel(UIPanelType.Stats);
        }
    }

    // ═══════════════════════ 公开方法 ═══════════════════════

    /// <summary>
    /// 打开指定面板（不自动关闭其他面板，各面板独立控制）。
    /// </summary>
    public void OpenPanel(UIPanelType panelType)
    {
        if (!panelMap.TryGetValue(panelType, out CanvasGroup cg) || cg == null)
        {
            Debug.LogWarning($"[UIManager] OpenPanel 失败：面板 {panelType} 的 CanvasGroup 未赋值，请在 Inspector 中检查 UIManager 的面板引用！");
            return;
        }

        if (!openPanels.Contains(panelType))
        {
            SetCanvasGroupVisible(cg, true);
            openPanels.Add(panelType);
            OnPanelOpened?.Invoke(panelType);

            // 有任何面板打开时暂停游戏
            Time.timeScale = 0f;
        }
    }

    /// <summary>
    /// 关闭指定面板。
    /// </summary>
    public void ClosePanel(UIPanelType panelType)
    {
        if (!openPanels.Contains(panelType)) return;

        if (panelMap.TryGetValue(panelType, out CanvasGroup cg) && cg != null)
        {
            SetCanvasGroupVisible(cg, false);
        }
        openPanels.Remove(panelType);
        OnPanelClosed?.Invoke(panelType);

        // 所有面板关闭时恢复游戏
        if (openPanels.Count == 0)
        {
            Time.timeScale = 1f;
        }
    }

    /// <summary>
    /// 切换指定面板（开 ↔ 关）。
    /// </summary>
    public void TogglePanel(UIPanelType panelType)
    {
        if (IsPanelOpen(panelType))
            ClosePanel(panelType);
        else
            OpenPanel(panelType);
    }

    /// <summary>
    /// 关闭所有已打开的面板。
    /// </summary>
    public void CloseAllPanels()
    {
        foreach (var panelType in new List<UIPanelType>(openPanels))
        {
            ClosePanel(panelType);
        }
    }

    /// <summary>
    /// 检查指定面板是否处于打开状态。
    /// </summary>
    public bool IsPanelOpen(UIPanelType panelType)
    {
        return openPanels.Contains(panelType);
    }

    /// <summary>
    /// 是否有任何面板处于打开状态（可用于禁用玩家移动/攻击等）。
    /// </summary>
    public bool IsAnyPanelOpen()
    {
        return openPanels.Count > 0;
    }

    // ═══════════════════════ 内部方法 ═══════════════════════

    private void SetCanvasGroupVisible(CanvasGroup cg, bool visible)
    {
        if (cg == null) return;
        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }
}

/// <summary>UI 面板类型枚举</summary>
public enum UIPanelType
{
    Settings,
    Inventory,
    Stats,
    Quest,
    Shop,
    Dialogue
}
