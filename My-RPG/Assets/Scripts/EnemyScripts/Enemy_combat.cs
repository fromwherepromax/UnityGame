using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class En : MonoBehaviour
{
    public int damage = 1;
    public Transform attackPoint;
    public float weaponRange;
    public float knockbackForce;
    public float stunTime;
    public LayerMask playerLayer;

    // public void OnCollisionEnter2D(Collision2D collision)  //触碰玩家时造成伤害
    // {
    //     Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);
    //     if (collision.gameObject.tag=="Player")
    //     {
    //         collision.gameObject.GetComponent<PlayerHealth>().ChangHealth(-damage);
    //         hits[0].GetComponent<PlayerMovemont>().Knockback(transform, knockbackForce, stunTime);
    //     }
        
    // }

    public void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);
        if (hits.Length>0)
        {
            PlayerHealth playerHealth = hits[0].GetComponent<PlayerHealth>();
            playerHealth.ChangHealth(-damage);

            // 玩家死亡后不再击退
            if (!playerHealth.IsDead)
            {
                hits[0].GetComponent<PlayerMovemont>().Knockback(transform,knockbackForce,stunTime);
            }
        }

        Debug.Log("Attacking Player Now!");
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        // 攻击判定范围 —— 红色
        Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }
}
