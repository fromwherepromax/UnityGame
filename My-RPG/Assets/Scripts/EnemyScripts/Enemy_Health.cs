// 脚本说明：EnemyHealth 相关逻辑。
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Health : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    public float hitFlashDuration=0.1f;
    public int expReward=3;
    public delegate void EnemyDefeated(int expReward);
    public static event EnemyDefeated OnEnemyDefeated;

    [Header("Health Bar")]
    public GameObject healthBarPrefab;
    public Vector3 healthBarOffset = new Vector3(0, 1f, 0);

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine hitFlashCoroutine;
    private Slider healthSlider;
    private GameObject healthBarInstance;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        SpawnHealthBar();
    }

    private void LateUpdate()
    {
        if (healthBarInstance != null && Camera.main != null)
        {
            healthBarInstance.transform.rotation = Camera.main.transform.rotation;
        }
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        if (amount < 0)
        {
            FlashRed();
        }

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            OnEnemyDefeated(expReward);
            Enemy_Loot enemyLoot = GetComponent<Enemy_Loot>();
            if (enemyLoot != null) enemyLoot.DropLoot();
            gameObject.SetActive(false);
            return;
        }

        UpdateHealthBar();
    }

    private void SpawnHealthBar()
    {
        if (healthBarPrefab == null) return;

        GameObject bar = Instantiate(healthBarPrefab, transform);
        bar.transform.localPosition = healthBarOffset;
        bar.transform.localScale = Vector3.one * 0.01f;
        healthBarInstance = bar;

        Canvas canvas = bar.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = 10;
        }

        healthSlider = bar.GetComponentInChildren<Slider>();
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    private void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    private void FlashRed()
    {
        if (spriteRenderer == null)
            return;

        if (hitFlashCoroutine != null)
        {
            StopCoroutine(hitFlashCoroutine);
        }

        hitFlashCoroutine = StartCoroutine(HitFlashCoroutine());
    }

    private IEnumerator HitFlashCoroutine()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
        hitFlashCoroutine = null;
    }
}
