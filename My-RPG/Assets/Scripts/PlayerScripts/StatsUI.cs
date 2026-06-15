using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class StatsUI : MonoBehaviour
{
    public GameObject[] statsSlots;

    private void Start()
    {
        UpdateAllStats();
    }

    private void Update()
    {
        if (Input.GetButtonDown("ToggleStats"))
        {
            ToggleStats();
        }
    }

    private void OnEnable()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnPanelOpened += HandlePanelOpened;
            UIManager.Instance.OnPanelClosed += HandlePanelClosed;
        }
    }

    private void OnDisable()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnPanelOpened -= HandlePanelOpened;
            UIManager.Instance.OnPanelClosed -= HandlePanelClosed;
        }
    }

    /// <summary>
    /// 切换属性面板的显示/隐藏（通过 UIManager 管理）
    /// </summary>
    public void ToggleStats()
    {
        if (UIManager.Instance == null) return;

        if (UIManager.Instance.IsPanelOpen(UIPanelType.Stats))
        {
            Time.timeScale = 1;
            UIManager.Instance.ClosePanel(UIPanelType.Stats);
        }
        else
        {
            Time.timeScale = 0;
            UIManager.Instance.OpenPanel(UIPanelType.Stats);
            UpdateAllStats();
        }
    }

    // ────── 事件处理 ──────

    private void HandlePanelOpened(UIPanelType panelType)
    {
        if (panelType == UIPanelType.Stats)
        {
            Time.timeScale = 0f;
            UpdateAllStats();
        }
    }

    private void HandlePanelClosed(UIPanelType panelType)
    {
        if (panelType == UIPanelType.Stats)
        {
            Time.timeScale = 1f;
        }
    }

    public void UpdateAllStats()
    {
        UpdateDamage();
        UpdateSpeed();
        UpdateArrowDamage();
        UpdateHealAmount();
        UpdateExplosionDamage();
    }
    public void UpdateDamage()
    {
        statsSlots[0].GetComponentInChildren<TMP_Text>().text="近战攻击:" + StatsManager.Instance.damage;
    }

    public void UpdateSpeed()
    {
        statsSlots[1].GetComponentInChildren<TMP_Text>().text = "速度:" + StatsManager.Instance.speed;
    }

    public void UpdateArrowDamage()
    {
        if (statsSlots.Length > 2)
            statsSlots[2].GetComponentInChildren<TMP_Text>().text = "箭伤:" + StatsManager.Instance.arrowDamage;
    }

    public void UpdateHealAmount()
    {
        if (statsSlots.Length > 3)
            statsSlots[3].GetComponentInChildren<TMP_Text>().text = "治疗:" + StatsManager.Instance.healAmount;
    }

    public void UpdateExplosionDamage()
    {
        if (statsSlots.Length > 4)
            statsSlots[4].GetComponentInChildren<TMP_Text>().text = "爆炸:" + StatsManager.Instance.explosionDamage;
    }
}
