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
    public float cooldown;

    [Header("Arrow Stats")]
    public int arrowDamage = 2;
    public float arrowSpeed = 20f;
    public float arrowKnockbackForce = 5f;
    public float arrowKnockbackDuration = 0.5f;
    public float arrowStunTime = 0.5f;

    [Header("Monk Stats")]
    public int healAmount = 5;
    public float healCooldown = 5f;

    [Header("Explosion Stats")]
    public int explosionDamage = 15;
    public float explosionRadius = 3f;
    public float explosionCooldown = 8f;
    public float explosionKnockbackForce = 8f;
    public float explosionKnockbackDuration = 0.5f;
    public float explosionStunTime = 0.5f;

    [Header("Movement Stats")]
    public int speed;


    [Header("Health Stats")]
    public int MaxHealth;
    public int CurrentHealth;

    [Header("Level Up Rewards")]
    public int MaxHealthPerLevel = 10;
    public int damagePerLevel = 1;
    public int arrowDamagePerLevel = 1;
    public int healPerLevel = 1;
    public int explosionDamagePerLevel = 2;

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

    private void OnEnable()
    {
        ExpManager.OnLevelUp += OnLevelUp;
    }

    private void OnDisable()
    {
        ExpManager.OnLevelUp -= OnLevelUp;
    }

    private void OnLevelUp(int level)
    {
        UpdateHealth(MaxHealthPerLevel);
        CurrentHealth = MaxHealth; // 升级回满血
        damage += damagePerLevel;
        arrowDamage += arrowDamagePerLevel;
        healAmount += healPerLevel;
        explosionDamage += explosionDamagePerLevel;

        if (healthText != null)
        {
            healthText.text = "HP:" + CurrentHealth + "/" + MaxHealth;
        }
        if (statsUI != null)
        {
            statsUI.UpdateAllStats();
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
    public void UpdateDamage(int amount)
    {
        damage += amount;

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
