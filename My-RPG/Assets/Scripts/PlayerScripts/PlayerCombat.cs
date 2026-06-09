using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class player_Combat : MonoBehaviour
{

    public Transform attackPoint;
    public LayerMask enemyLayer;
    public Animator anim;
    private float timer;

    /// <summary>
    /// 冷却进度：0 = 就绪，1 = 刚释放完（冷却最长）
    /// </summary>
    public float CooldownProgress
    {
        get
        {
            float max = StatsManager.Instance != null ? StatsManager.Instance.cooldown : 1f;
            return max > 0 ? Mathf.Clamp01(timer / max) : 0f;
        }
    }

    private void Update()
    {
        if (timer>0)
        {
            timer -= Time.deltaTime;
        }
    }

    public void Attack()
    {
        if (timer<=0)
        {
            anim.SetBool("isAttacking", true);
            timer = StatsManager.Instance.cooldown;
        }
    }

    public void DealDamage()
    {
        MusicManager.Instance?.PlaySaberCombat();
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, StatsManager.Instance.weaponRange, enemyLayer);

        if (enemies.Length > 0)
        {
            enemies[0].GetComponent<Enemy_Health>().ChangeHealth(-StatsManager.Instance.damage);
            if (enemies[0].gameObject.activeInHierarchy)
            {
                enemies[0].GetComponent<Enemy_knockBack>().knockback(transform, StatsManager.Instance.knockbackforce, StatsManager.Instance.knockbacktime, StatsManager.Instance.stuntime);
            }
        }
    }

    public void FinishAttack()
    {
        anim.SetBool("isAttacking", false);
    }

    private void OnDisable()
    {
        if (anim != null) anim.SetBool("isAttacking", false);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null || StatsManager.Instance == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, StatsManager.Instance.weaponRange);
    }
}
