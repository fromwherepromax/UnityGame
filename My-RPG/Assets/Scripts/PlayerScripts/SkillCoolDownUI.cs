using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 技能冷却 UI —— 原神风格的扇形冷却遮罩 + 倒计时文字。
/// 切换职业时自动切换技能图标。
/// </summary>
public class SkillCoolDownUI : MonoBehaviour
{
    // ═══════════════════════ UI 引用 ═══════════════════════
    [Header("UI 控件")]
    [Tooltip("技能图标 Image（显示当前职业的技能图标）")]
    public Image skillIcon;

    [Tooltip("冷却遮罩 Image（Fill Method = Radial 360，灰色半透明）")]
    public Image cooldownOverlay;

    [Tooltip("冷却倒计时文字（如 \"3.2\"）")]
    public TMP_Text cooldownText;

    // ═══════════════════════ 职业技能图标 ═══════════════════════
    [Header("职业技能图标（按顺序：剑士/弓手/僧侣/法师）")]
    public Sprite[] skillIcons = new Sprite[4];

    // ═══════════════════════ 职业脚本引用 ═══════════════════════
    [Header("职业脚本引用")]
    public player_Combat combat;
    public Player_Bow bow;
    public Player_Monk monk;
    public Player_Caster caster;

    // ═══════════════════════ 冷却遮罩颜色 ═══════════════════════
    [Header("冷却遮罩颜色")]
    [Tooltip("冷却中遮罩颜色（灰色半透明）")]
    public Color coolingColor = new Color(0f, 0f, 0f, 0.6f);

    [Tooltip("就绪时遮罩颜色（完全透明）")]
    public Color readyColor = new Color(0f, 0f, 0f, 0f);

    // ═══════════════════════ 私有字段 ═══════════════════════
    private int currentClassIndex = 0; // 0=剑士, 1=弓手, 2=僧侣, 3=法师

    private void Start()
    {
        // 初始化：隐藏冷却遮罩和文字
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0f;
            cooldownOverlay.color = readyColor;
        }
        if (cooldownText != null)
        {
            cooldownText.text = "";
        }

        // 默认显示剑士图标
        UpdateIcon(0);
    }

    private void Update()
    {
        float progress = GetCooldownProgress();

        // --- 更新冷却遮罩 ---
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = progress;

            if (progress > 0f)
            {
                cooldownOverlay.color = coolingColor;
            }
            else
            {
                cooldownOverlay.color = readyColor;
            }
        }

        // --- 更新倒计时文字 ---
        if (cooldownText != null)
        {
            if (progress > 0f)
            {
                float remaining = GetRemainingCooldown();
                cooldownText.text = remaining.ToString("F1");
            }
            else
            {
                cooldownText.text = "";
            }
        }

        // --- 冷却结束时图标恢复亮度 ---
        if (skillIcon != null)
        {
            skillIcon.color = progress > 0f
                ? new Color(0.5f, 0.5f, 0.5f, 1f) // 冷却中：灰色
                : Color.white;                       // 就绪：正常
        }
    }

    // ═══════════════════════ 公开方法 ═══════════════════════

    /// <summary>
    /// 职业切换时调用（由 Player_Change 调用）
    /// </summary>
    /// <param name="classIndex">0=剑士, 1=弓手, 2=僧侣, 3=法师</param>
    public void OnClassChanged(int classIndex)
    {
        currentClassIndex = classIndex;
        UpdateIcon(classIndex);
    }

    // ═══════════════════════ 私有方法 ═══════════════════════

    /// <summary>
    /// 获取当前职业的冷却进度（0=就绪，1=冷却最长）
    /// </summary>
    private float GetCooldownProgress()
    {
        switch (currentClassIndex)
        {
            case 0: return combat  != null ? combat.CooldownProgress  : 0f;
            case 1: return bow     != null ? bow.CooldownProgress     : 0f;
            case 2: return monk    != null ? monk.CooldownProgress    : 0f;
            case 3: return caster  != null ? caster.CooldownProgress  : 0f;
            default: return 0f;
        }
    }

    /// <summary>
    /// 获取当前职业的剩余冷却秒数
    /// </summary>
    private float GetRemainingCooldown()
    {
        switch (currentClassIndex)
        {
            case 0:
                float saberMax = StatsManager.Instance != null ? StatsManager.Instance.cooldown : 1f;
                return saberMax * (combat != null ? combat.CooldownProgress : 0f);
            case 1:
                return bow != null ? bow.shootCooldown * bow.CooldownProgress : 0f;
            case 2:
                float monkMax = StatsManager.Instance != null ? StatsManager.Instance.healCooldown : 5f;
                return monkMax * (monk != null ? monk.CooldownProgress : 0f);
            case 3:
                float casterMax = StatsManager.Instance != null ? StatsManager.Instance.explosionCooldown : 8f;
                return casterMax * (caster != null ? caster.CooldownProgress : 0f);
            default: return 0f;
        }
    }

    /// <summary>
    /// 切换技能图标
    /// </summary>
    private void UpdateIcon(int classIndex)
    {
        if (skillIcon == null) return;
        if (skillIcons != null && classIndex >= 0 && classIndex < skillIcons.Length && skillIcons[classIndex] != null)
        {
            skillIcon.sprite = skillIcons[classIndex];
        }
    }
}
