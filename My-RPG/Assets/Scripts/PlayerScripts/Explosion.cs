using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public LayerMask enemyLayer;
    public float lifetime = 1f;

    void Start()
    {
        // 从 StatsManager 读取数值
        int damage = StatsManager.Instance != null ? StatsManager.Instance.explosionDamage : 15;
        float radius = StatsManager.Instance != null ? StatsManager.Instance.explosionRadius : 3f;
        float knockbackForce = StatsManager.Instance != null ? StatsManager.Instance.explosionKnockbackForce : 8f;
        float knockbackDuration = StatsManager.Instance != null ? StatsManager.Instance.explosionKnockbackDuration : 0.5f;
        float stunTime = StatsManager.Instance != null ? StatsManager.Instance.explosionStunTime : 0.5f;

        // 缩放特效使其与爆炸半径匹配
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            float spriteSize = sr.bounds.size.x; // 精灵当前的实际尺寸
            if (spriteSize > 0)
            {
                float targetSize = radius * 2f; // 直径
                float scale = targetSize / spriteSize;
                transform.localScale = Vector3.one * scale;
            }
        }
        // 检测范围内所有敌人
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);

        foreach (Collider2D enemy in enemies)
        {
            // 造成伤害
            Enemy_Health enemyHealth = enemy.GetComponentInParent<Enemy_Health>();
            if (enemyHealth != null)
            {
                enemyHealth.ChangeHealth(-damage);
            }

            // 击退效果
            Enemy_knockBack enemyKnockback = enemy.GetComponentInParent<Enemy_knockBack>();
            if (enemyKnockback != null && enemyKnockback.gameObject.activeInHierarchy)
            {
                enemyKnockback.knockback(transform, knockbackForce, knockbackDuration, stunTime);
            }
        }

        // 动画播完后自动销毁
        Destroy(gameObject, lifetime);
    }

    private void OnDrawGizmosSelected()
    {
        float radius = StatsManager.Instance != null ? StatsManager.Instance.explosionRadius : 3f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
