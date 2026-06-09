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

    // ═══════════════════════ 外部引用 ═══════════════════════
    [Header("外部引用（可选，不设置则自动查找）")]
    public StatsUI statsUI;
    public QuestLogUI questLogUI;

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

        // 自动查找引用
        if (statsUI == null)
            statsUI = FindObjectOfType<StatsUI>();
        if (questLogUI == null)
            questLogUI = FindObjectOfType<QuestLogUI>();
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
    /// 打开背包 —— 先关闭设置面板，再切换背包
    /// </summary>
    private void OnOpenInventory()
    {
        OnClosePanel();
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ToggleInventory();
        }
    }

    /// <summary>
    /// 打开属性面板 —— 先关闭设置面板，再切换属性面板
    /// </summary>
    private void OnOpenStats()
    {
        OnClosePanel();
        if (statsUI != null)
        {
            statsUI.ToggleStats();
        }
    }

    /// <summary>
    /// 打开任务日志 —— 先关闭设置面板，再显示任务日志
    /// </summary>
    private void OnOpenQuestLog()
    {
        OnClosePanel();
        if (questLogUI != null)
        {
            questLogUI.ToggleQuestLog();
        }
    }

    // ═══════════════════════ 系统设置回调 ═══════════════════════

    /// <summary>
    /// 关闭设置面板
    /// </summary>
    private void OnClosePanel()
    {
        if (SettingManager.Instance != null)
        {
            SettingManager.Instance.CloseSettings();
        }
    }
}
