// 脚本说明：EnemyHealth 相关逻辑。
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    public float hitFlashDuration=0.1f;
    public int expReward=3;
    public delegate void EnemyDefeated(int expReward);
    public static event EnemyDefeated OnEnemyDefeated;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine hitFlashCoroutine;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
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
            gameObject.SetActive(false);

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
