using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 音乐管理器（单例）—— 挂载到一个独立的空 GameObject 上，会自动 DontDestroyOnLoad。
/// BGM 在场景切换时根据 MusicLibrary 自动淡入淡出；SFX 通过 PlayOneShot 播放。
/// 音量设置通过 PlayerPrefs 持久化。
/// </summary>
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    // ───────────────────────── Inspector 字段 ─────────────────────────
    [Header("引用")]
    [Tooltip("拖入在 Project 中创建的 MusicLibrary ScriptableObject")]
    public MusicLibrary musicLibrary;

    [Header("音频源（留空则自动创建）")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("BGM 设置")]
    [Range(0f, 1f)] public float bgmVolume = 0.7f;
    [Tooltip("淡入淡出时长（秒）")]
    public float fadeDuration = 1.5f;
    public bool bgmMuted = false;

    [Header("SFX 音效片段 —— 从 Music/SFX 拖入")]
    public AudioClip saberCombatClip;       // Saber_combat.mp3
    public AudioClip archerShootClip;       // Archer_shoot.mp3
    public AudioClip casterExplosionClip;   // Caster_explosion.mp3
    public AudioClip casterExplosion2Clip;  // Caster_explosion2.mp3
    public AudioClip monkHealClip;          // Monk_heal.mp3
    public AudioClip monkHeal2Clip;         // Monk_heal2.mp3
    public AudioClip femaleHitClip;         // Female_hit.mp3
    public AudioClip levelUpClip;           // LevelUp.mp3
    public AudioClip pickUpClip;            // PickUp.mp3
    public AudioClip completeTaskClip;      // CompleteTask.mp3
    public AudioClip buttonClip;            // Button.mp3
    public AudioClip clickClip;             // click.mp3

    [Header("SFX 设置")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    public bool sfxMuted = false;

    // ───────────────────────── 私有字段 ─────────────────────────
    private Coroutine fadeCoroutine;
    private string currentSceneBGM = "";

    private const string BGM_VOL_KEY   = "BGMVolume";
    private const string SFX_VOL_KEY   = "SFXVolume";
    private const string BGM_MUTE_KEY  = "BGMMuted";
    private const string SFX_MUTE_KEY  = "SFXMuted";

    // ═══════════════════════════ 生命周期 ═══════════════════════════
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
            EnsureAudioSources();
            Debug.Log("[MusicManager] 初始化成功，BGM音量=" + bgmVolume + " SFX音量=" + sfxVolume + " bgmMuted=" + bgmMuted + " sfxMuted=" + sfxMuted);
        }
        else
        {
            Destroy(gameObject);
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

    private void Update()
    {
        // 按 M 键切换 BGM 静音
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleBGMMute();
            Debug.Log("[MusicManager] BGM 静音切换: " + (bgmMuted ? "已静音" : "已开启"));
        }
        // 按 N 键切换 SFX 静音
        if (Input.GetKeyDown(KeyCode.N))
        {
            ToggleSFXMute();
            Debug.Log("[MusicManager] SFX 静音切换: " + (sfxMuted ? "已静音" : "已开启"));
        }
    }

    private void Start()
    {
        // 第一个场景已加载，sceneLoaded 不会再为它触发，所以手动播放当前场景的 BGM
        PlayBGMForScene(SceneManager.GetActiveScene().name);
    }

    // ═══════════════════════════ 初始化 ═══════════════════════════
    private void EnsureAudioSources()
    {
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            bgmSource.spatialBlend = 0f; // 确保 2D 音频，不依赖位置
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.spatialBlend = 0f;
        }
        ApplyVolume();
    }

    // ═══════════════════════════ 场景切换 ═══════════════════════════
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
    }

    // ═══════════════════════════ BGM 公共方法 ═══════════════════════════
    #region BGM

    /// <summary>根据 MusicLibrary 中的映射自动播放当前场景的 BGM</summary>
    public void PlayBGMForScene(string sceneName)
    {
        if (musicLibrary == null)
        {
            Debug.LogWarning("[MusicManager] MusicLibrary 未设置！请在 Inspector 中拖入 MusicLibrary 资源。");
            return;
        }
        var entry = musicLibrary.GetBGMForScene(sceneName);
        if (entry != null && entry.bgmClip != null && currentSceneBGM != sceneName)
        {
            Debug.Log("[MusicManager] 播放场景BGM: " + sceneName + " -> " + entry.bgmClip.name + " (isPlaying=" + bgmSource.isPlaying + ")");
            // 首次播放（无当前BGM）直接播放，后续切换用交叉淡入淡出
            if (string.IsNullOrEmpty(currentSceneBGM))
            {
                PlayBGM(entry.bgmClip, entry.volume);
            }
            else
            {
                CrossFadeBGM(entry.bgmClip, entry.volume);
            }
            currentSceneBGM = sceneName;
        }
        else if (entry == null)
        {
            Debug.Log("[MusicManager] 场景 '" + sceneName + "' 在 MusicLibrary 中无对应BGM配置。");
        }
    }

    /// <summary>立即播放指定 BGM（无淡入）</summary>
    public void PlayBGM(AudioClip clip, float volume = -1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("[MusicManager] PlayBGM: AudioClip 为空！");
            return;
        }
        if (volume < 0f) volume = bgmVolume;
        float finalVol = bgmMuted ? 0f : volume;
        bgmSource.clip = clip;
        bgmSource.volume = finalVol;
        bgmSource.Play();
        Debug.Log("[MusicManager] PlayBGM: " + clip.name + " volume=" + finalVol + " isPlaying=" + bgmSource.isPlaying);
    }

    public void StopBGM()
    {
        bgmSource.Stop();
        currentSceneBGM = "";
    }

    public void PauseBGM()  => bgmSource.Pause();
    public void ResumeBGM() => bgmSource.UnPause();

    /// <summary>淡出当前 BGM</summary>
    public void FadeOutBGM(float duration = -1f)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOutCoroutine(duration < 0f ? fadeDuration : duration));
    }

    /// <summary>淡入指定 BGM</summary>
    public void FadeInBGM(AudioClip clip, float duration = -1f, float targetVolume = -1f)
    {
        if (clip == null) return;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        if (targetVolume < 0f) targetVolume = bgmVolume;
        fadeCoroutine = StartCoroutine(FadeInCoroutine(clip, duration < 0f ? fadeDuration : duration, targetVolume));
    }

    /// <summary>交叉淡入淡出切换 BGM</summary>
    public void CrossFadeBGM(AudioClip newClip, float targetVolume = -1f, float duration = -1f)
    {
        if (newClip == null) return;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        if (targetVolume < 0f) targetVolume = bgmVolume;
        fadeCoroutine = StartCoroutine(CrossFadeCoroutine(newClip, duration < 0f ? fadeDuration : duration, targetVolume));
    }

    // ────── 协程 ──────
    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVol = bgmSource.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVol, 0f, t / duration);
            yield return null;
        }
        bgmSource.volume = 0f;
        bgmSource.Stop();
    }

    private IEnumerator FadeInCoroutine(AudioClip clip, float duration, float targetVol)
    {
        bgmSource.clip = clip;
        bgmSource.volume = 0f;
        bgmSource.Play();
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, targetVol, t / duration);
            yield return null;
        }
        bgmSource.volume = targetVol;
    }

    private IEnumerator CrossFadeCoroutine(AudioClip newClip, float duration, float targetVol)
    {
        float half = duration * 0.5f;
        // 先淡出
        float startVol = bgmSource.volume;
        float t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVol, 0f, t / half);
            yield return null;
        }
        bgmSource.volume = 0f;
        // 切换并淡入
        bgmSource.clip = newClip;
        bgmSource.Play();
        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, targetVol, t / half);
            yield return null;
        }
        bgmSource.volume = targetVol;
    }

    #endregion

    // ═══════════════════════════ SFX 公共方法 ═══════════════════════════
    #region SFX

    /// <summary>播放一次音效</summary>
    public void PlaySFX(AudioClip clip, float volumeScale = -1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("[MusicManager] PlaySFX: AudioClip 为空，无法播放！请检查 Inspector 中 SFX 字段是否已拖入音频。", this);
            return;
        }
        if (sfxMuted) return;
        if (volumeScale < 0f) volumeScale = sfxVolume;
        sfxSource.PlayOneShot(clip, volumeScale);
    }

    // ── 便捷方法（可直接在动画事件或其他脚本中调用）──
    public void PlaySaberCombat()    => PlaySFX(saberCombatClip);
    public void PlayArcherShoot()    => PlaySFX(archerShootClip);
    public void PlayCasterExplosion()=> PlaySFX(casterExplosionClip);
    public void PlayCasterExplosion2()=> PlaySFX(casterExplosion2Clip);
    public void PlayMonkHeal()       => PlaySFX(monkHealClip);
    public void PlayMonkHeal2()      => PlaySFX(monkHeal2Clip);
    public void PlayFemaleHit()      => PlaySFX(femaleHitClip);
    public void PlayLevelUp()        => PlaySFX(levelUpClip);
    public void PlayPickUp()         => PlaySFX(pickUpClip);
    public void PlayCompleteTask()   => PlaySFX(completeTaskClip);
    public void PlayButton()         => PlaySFX(buttonClip);
    public void PlayClick()          => PlaySFX(clickClip);

    #endregion

    // ═══════════════════════════ 音量控制 ═══════════════════════════
    #region Volume

    public void SetBGMVolume(float v)
    {
        bgmVolume = Mathf.Clamp01(v);
        if (!bgmMuted) bgmSource.volume = bgmVolume;
        SaveSettings();
    }

    public void SetSFXVolume(float v)
    {
        sfxVolume = Mathf.Clamp01(v);
        SaveSettings();
    }

    public void ToggleBGMMute()
    {
        bgmMuted = !bgmMuted;
        bgmSource.volume = bgmMuted ? 0f : bgmVolume;
        SaveSettings();
    }

    public void ToggleSFXMute()
    {
        sfxMuted = !sfxMuted;
        SaveSettings();
    }

    public float GetBGMVolume() => bgmVolume;
    public float GetSFXVolume() => sfxVolume;
    public bool  IsBGMMuted()   => bgmMuted;
    public bool  IsSFXMuted()   => sfxMuted;

    private void ApplyVolume()
    {
        bgmSource.volume = bgmMuted ? 0f : bgmVolume;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(BGM_VOL_KEY, bgmVolume);
        PlayerPrefs.SetFloat(SFX_VOL_KEY, sfxVolume);
        // 不保存静音状态
        PlayerPrefs.DeleteKey(BGM_MUTE_KEY);
        PlayerPrefs.DeleteKey(SFX_MUTE_KEY);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        bgmVolume = PlayerPrefs.GetFloat(BGM_VOL_KEY, 0.7f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOL_KEY, 1f);
        // 不加载静音状态，清除残留数据
        bgmMuted = false;
        sfxMuted = false;
        PlayerPrefs.DeleteKey(BGM_MUTE_KEY);
        PlayerPrefs.DeleteKey(SFX_MUTE_KEY);
        PlayerPrefs.Save();
    }

    #endregion
}
