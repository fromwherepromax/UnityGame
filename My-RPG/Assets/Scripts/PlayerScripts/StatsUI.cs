using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class StatsUI : MonoBehaviour
{
    public GameObject[] statsSlots;
    public CanvasGroup statsCanvas;
    private bool statsOpen = false;



    private void Start()
    {
        UpdateAllStats();
    }

    private void Update()
    {
        if (Input.GetButtonDown("ToggleStats"))
        {
            if (statsOpen)
            {
                Time.timeScale = 1;
                statsCanvas.alpha = 0;
                statsOpen = false;
            }
            else
            {
                Time.timeScale = 0;
                statsCanvas.alpha = 1;
                statsOpen = true;
            }
        }
    }


    public void UpdateDamage()
    {
        statsSlots[0].GetComponentInChildren<TMP_Text>().text="攻击:" + StatsManager.Instance.damage;
    }

    public void UpdateSpeed()
    {
        statsSlots[1].GetComponentInChildren<TMP_Text>().text = "速度:" + StatsManager.Instance.speed;
    }

    public void UpdateAllStats()
    {
        UpdateDamage();
        UpdateSpeed();
        UpdateArrowDamage();
        UpdateHealAmount();
        UpdateExplosionDamage();
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
