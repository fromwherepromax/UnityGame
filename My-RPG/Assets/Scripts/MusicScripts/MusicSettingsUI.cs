using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 音量设置 UI 面板 —— 挂载到包含 Slider / Toggle 的 UI Panel 上。
/// 在 Inspector 中拖入对应的控件引用即可。
/// </summary>
public class MusicSettingsUI : MonoBehaviour
{
    [Header("BGM 控件")]
    public Slider bgmSlider;
    public Toggle bgmMuteToggle;
    public TMP_Text bgmVolumeText;

    [Header("SFX 控件")]
    public Slider sfxSlider;
    public Toggle sfxMuteToggle;
    public TMP_Text sfxVolumeText;

    private void Start()
    {
        InitUI();
    }

    private void OnEnable()
    {
        // 每次面板打开时刷新数值
        RefreshUI();
    }

    private void InitUI()
    {
        if (MusicManager.Instance == null) return;

        if (bgmSlider != null)
        {
            bgmSlider.minValue = 0f;
            bgmSlider.maxValue = 1f;
            bgmSlider.value = MusicManager.Instance.GetBGMVolume();
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0f;
            sfxSlider.maxValue = 1f;
            sfxSlider.value = MusicManager.Instance.GetSFXVolume();
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        if (bgmMuteToggle != null)
        {
            bgmMuteToggle.isOn = MusicManager.Instance.IsBGMMuted();
            bgmMuteToggle.onValueChanged.AddListener(OnBGMMuteToggle);
        }

        if (sfxMuteToggle != null)
        {
            sfxMuteToggle.isOn = MusicManager.Instance.IsSFXMuted();
            sfxMuteToggle.onValueChanged.AddListener(OnSFXMuteToggle);
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (MusicManager.Instance == null) return;

        if (bgmSlider != null) bgmSlider.value = MusicManager.Instance.GetBGMVolume();
        if (sfxSlider != null) sfxSlider.value = MusicManager.Instance.GetSFXVolume();
        if (bgmMuteToggle != null) bgmMuteToggle.isOn = MusicManager.Instance.IsBGMMuted();
        if (sfxMuteToggle != null) sfxMuteToggle.isOn = MusicManager.Instance.IsSFXMuted();

        UpdateTexts();
    }

    // ────── 回调 ──────
    private void OnBGMVolumeChanged(float value)
    {
        MusicManager.Instance?.SetBGMVolume(value);
        UpdateTexts();
    }

    private void OnSFXVolumeChanged(float value)
    {
        MusicManager.Instance?.SetSFXVolume(value);
        UpdateTexts();
    }

    private void OnBGMMuteToggle(bool muted)
    {
        if (MusicManager.Instance != null && muted != MusicManager.Instance.IsBGMMuted())
            MusicManager.Instance.ToggleBGMMute();
    }

    private void OnSFXMuteToggle(bool muted)
    {
        if (MusicManager.Instance != null && muted != MusicManager.Instance.IsSFXMuted())
            MusicManager.Instance.ToggleSFXMute();
    }

    private void UpdateTexts()
    {
        if (MusicManager.Instance == null) return;
        if (bgmVolumeText != null)
            bgmVolumeText.text = Mathf.RoundToInt(MusicManager.Instance.GetBGMVolume() * 100) + "%";
        if (sfxVolumeText != null)
            sfxVolumeText.text = Mathf.RoundToInt(MusicManager.Instance.GetSFXVolume() * 100) + "%";
    }
}
