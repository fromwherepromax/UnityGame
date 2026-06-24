using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 设置面板 UI —— 管理音量调节、快捷功能按钮、系统设置。
/// 挂载到设置面板的根 Panel 上。
/// </summary>
public class SettingsPanelUI : MonoBehaviour
{
    // ═══════════════════════ 音量控制 ═══════════════════════
    [Header("BGM 控件")]
    public Slider bgmSlider;
    public TMP_Text bgmVolumeText;

    [Header("SFX 控件")]
    public Slider sfxSlider;
    public TMP_Text sfxVolumeText;

    // ═══════════════════════ 快捷功能按钮 ═══════════════════════
    [Header("快捷功能按钮")]
    public Button openInventoryBtn;   // 打开背包
    public Button openStatsBtn;       // 打开属性面板
    public Button openQuestLogBtn;    // 打开任务日志

    // ═══════════════════════ 系统设置 ═══════════════════════
    [Header("系统设置")]
    public Button closeBtn;           // 关闭面板按钮
    public Button quitBtn;            // 退出游戏按钮

    private bool initialized = false;

    private void OnEnable()
    {
        if (!initialized)
        {
            InitUI();
            initialized = true;
        }
        RefreshUI();
    }

    // ────── 初始化 ──────
    private void InitUI()
    {
        // --- BGM ---
        if (bgmSlider != null)
        {
            bgmSlider.minValue = 0f;
            bgmSlider.maxValue = 1f;
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }

        // --- SFX ---
        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0f;
            sfxSlider.maxValue = 1f;
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        // --- 快捷功能按钮 ---
        if (openInventoryBtn != null)
            openInventoryBtn.onClick.AddListener(OnOpenInventory);

        if (openStatsBtn != null)
            openStatsBtn.onClick.AddListener(OnOpenStats);

        if (openQuestLogBtn != null)
            openQuestLogBtn.onClick.AddListener(OnOpenQuestLog);

        // --- 系统设置 ---
        if (closeBtn != null)
            closeBtn.onClick.AddListener(OnClosePanel);

        if (quitBtn != null)
            quitBtn.onClick.AddListener(OnQuitGame);
    }

    /// <summary>
    /// 刷新所有 UI 控件的显示值（面板打开时调用）
    /// </summary>
    public void RefreshUI()
    {
        if (MusicManager.Instance == null) return;

        // BGM
        if (bgmSlider != null)
            bgmSlider.value = MusicManager.Instance.GetBGMVolume();
        // SFX
        if (sfxSlider != null)
            sfxSlider.value = MusicManager.Instance.GetSFXVolume();
        UpdateVolumeTexts();
    }

    // ═══════════════════════ 音量回调 ═══════════════════════

    private void OnBGMVolumeChanged(float value)
    {
        MusicManager.Instance?.SetBGMVolume(value);
        UpdateVolumeTexts();
    }

    private void OnSFXVolumeChanged(float value)
    {
        MusicManager.Instance?.SetSFXVolume(value);
        UpdateVolumeTexts();
    }

    private void UpdateVolumeTexts()
    {
        if (MusicManager.Instance == null) return;
        if (bgmVolumeText != null)
            bgmVolumeText.text = Mathf.RoundToInt(MusicManager.Instance.GetBGMVolume() * 100) + "%";
        if (sfxVolumeText != null)
            sfxVolumeText.text = Mathf.RoundToInt(MusicManager.Instance.GetSFXVolume() * 100) + "%";
    }

    // ═══════════════════════ 快捷功能回调 ═══════════════════════

    /// <summary>
    /// 打开背包 —— 先关闭设置面板，再打开背包
    /// </summary>
    private void OnOpenInventory()
    {
        MusicManager.Instance?.PlayClick();
        OnClosePanel();
        if (UIManager.Instance != null)
            UIManager.Instance.OpenPanel(UIPanelType.Inventory);
    }

    /// <summary>
    /// 打开属性面板 —— 先关闭设置面板，再打开属性面板
    /// </summary>
    private void OnOpenStats()
    {
        MusicManager.Instance?.PlayClick();
        OnClosePanel();
        if (UIManager.Instance != null)
            UIManager.Instance.OpenPanel(UIPanelType.Stats);
    }

    /// <summary>
    /// 打开任务日志 —— 先关闭设置面板，再打开任务日志
    /// </summary>
    private void OnOpenQuestLog()
    {
        MusicManager.Instance?.PlayClick();
        OnClosePanel();
        if (UIManager.Instance != null)
            UIManager.Instance.OpenPanel(UIPanelType.Quest);
    }

    // ═══════════════════════ 系统设置回调 ═══════════════════════

    /// <summary>
    /// 关闭设置面板
    /// </summary>
    private void OnClosePanel()
    {
        MusicManager.Instance?.PlayClick();
        if (SettingManager.Instance != null)
            SettingManager.Instance.CloseSettings();
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    private void OnQuitGame()
    {
        MusicManager.Instance?.PlayClick();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
