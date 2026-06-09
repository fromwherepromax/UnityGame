using UnityEngine;

/// <summary>
/// 设置管理器（单例）—— 按 Esc 键打开/关闭设置面板。
/// 挂载到一个持久化的 GameObject 上，面板打开时暂停游戏。
/// </summary>
public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance;

    [Header("引用")]
    [Tooltip("拖入设置面板的 CanvasGroup 组件")]
    public CanvasGroup settingsPanel; // 设置面板 CanvasGroup

    [Header("按键设置")]
    public KeyCode toggleKey = KeyCode.Escape;

    private bool isOpen = false;
    private float previousTimeScale = 1f; // 保存暂停前的时间缩放

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

        // 启动时隐藏面板
        SetPanelVisible(false);
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
        if (isOpen)
            CloseSettings();
        else
            OpenSettings();
    }

    /// <summary>
    /// 打开设置面板
    /// </summary>
    public void OpenSettings()
    {
        if (settingsPanel == null) return;

        isOpen = true;
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f; // 暂停游戏
        SetPanelVisible(true);

        // 刷新音量 UI
        SettingsPanelUI panelUI = settingsPanel.GetComponent<SettingsPanelUI>();
        if (panelUI != null)
        {
            panelUI.RefreshUI();
        }

        Debug.Log("[SettingManager] 设置面板已打开");
    }

    /// <summary>
    /// 关闭设置面板
    /// </summary>
    public void CloseSettings()
    {
        if (settingsPanel == null) return;

        isOpen = false;
        Time.timeScale = previousTimeScale; // 恢复游戏
        SetPanelVisible(false);

        Debug.Log("[SettingManager] 设置面板已关闭");
    }

    /// <summary>
    /// 面板是否打开
    /// </summary>
    public bool IsOpen()
    {
        return isOpen;
    }

    /// <summary>
    /// 通过 CanvasGroup 控制面板显示/隐藏
    /// </summary>
    private void SetPanelVisible(bool visible)
    {
        if (settingsPanel == null) return;

        settingsPanel.alpha = visible ? 1f : 0f;
        settingsPanel.interactable = visible;
        settingsPanel.blocksRaycasts = visible;
    }
}
