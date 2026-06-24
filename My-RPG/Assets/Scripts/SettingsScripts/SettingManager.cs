using UnityEngine;

/// <summary>
/// 设置管理器（单例）—— 按 Esc 键打开/关闭设置面板。
/// 面板显示/隐藏由 UIManager 统一管理，本脚本只负责快捷键和暂停逻辑。
/// </summary>
public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance;

    [Header("按键设置")]
    public KeyCode toggleKey = KeyCode.Escape;

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
    }

    private void OnEnable()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnPanelClosed += HandlePanelClosed;
        }
    }

    private void OnDisable()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnPanelClosed -= HandlePanelClosed;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleSettings();
        }
    }

    /// <summary>
    /// 切换设置面板的显示/隐藏
    /// </summary>
    public void ToggleSettings()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsPanelOpen(UIPanelType.Settings))
            CloseSettings();
        else
            OpenSettings();
    }

    /// <summary>
    /// 打开设置面板
    /// </summary>
    public void OpenSettings()
    {
        if (UIManager.Instance == null) return;

        UIManager.Instance.OpenPanel(UIPanelType.Settings);

        // 刷新音量 UI（需检查 CanvasGroup 是否已被场景重载销毁）
        CanvasGroup settingsCG = UIManager.Instance.settingsPanel;
        if (settingsCG != null)
        {
            SettingsPanelUI panelUI = settingsCG.GetComponent<SettingsPanelUI>();
            panelUI?.RefreshUI();
        }

        Debug.Log("[SettingManager] 设置面板已打开");
    }

    /// <summary>
    /// 关闭设置面板
    /// </summary>
    public void CloseSettings()
    {
        if (UIManager.Instance == null) return;

        UIManager.Instance.ClosePanel(UIPanelType.Settings);

        Debug.Log("[SettingManager] 设置面板已关闭");
    }

    /// <summary>
    /// 面板是否打开
    /// </summary>
    public bool IsOpen()
    {
        return UIManager.Instance != null && UIManager.Instance.IsPanelOpen(UIPanelType.Settings);
    }

    // ────── 事件处理 ──────

    /// <summary>
    /// 当面板被 UIManager 关闭时（例如打开其他面板导致设置面板自动关闭），
    /// 恢复游戏时间缩放。
    /// </summary>
    private void HandlePanelClosed(UIPanelType panelType)
    {
        if (panelType == UIPanelType.Settings)
        {
            // 暂停逻辑已由 UIManager 统一管理
        }
    }
}
