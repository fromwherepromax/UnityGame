using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance; //数值管理器
    public StatsUI statsUI;
    public TMP_Text healthText;


    [Header("Combat Stats")]
    public int damage;
    public float weaponRange;
    public float knockbackforce;
    public float knockbacktime;
    public float stuntime;

    [Header("Movement Stats")]
    public int speed;


    [Header("Health Stats")]
    public int MaxHealth;
    public int CurrentHealth;

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void BindUIReferences()
    {
        if (statsUI == null)
        {
            statsUI = FindObjectOfType<StatsUI>();
        }

        if (healthText == null)
        {
            GameObject healthObj = GameObject.Find("HealthText");
            if (healthObj != null)
            {
                healthText = healthObj.GetComponent<TMP_Text>();
            }
        }
    }

    public void UpdateHealth(int amount)
    {
        MaxHealth += amount;

        if (healthText == null)
        {
            BindUIReferences();
        }

        if (healthText != null)
        {
            healthText.text = "HP: " + CurrentHealth.ToString() + "/" + MaxHealth.ToString();
        }
    }
    public void UpdateSpeed(int amount)
    {
        speed += amount;

        if (statsUI == null)
        {
            BindUIReferences();
        }

        if (statsUI != null)
        {
            statsUI.UpdateAllStats();
        }
    }

}
