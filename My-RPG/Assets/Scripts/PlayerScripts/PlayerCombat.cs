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
