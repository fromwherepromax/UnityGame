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

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);
        if (collision.gameObject.tag=="Player")
        {
            collision.gameObject.GetComponent<PlayerHealth>().ChangHealth(-damage);
            hits[0].GetComponent<PlayerMovemont>().Knockback(transform, knockbackForce, stunTime);
        }
        
    }

    public void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);
        if (hits.Length>0)
        {
            hits[0].GetComponent<PlayerHealth>().ChangHealth(-damage);
            hits[0].GetComponent<PlayerMovemont>().Knockback(transform,knockbackForce,stunTime);
        }

        Debug.Log("Attacking Player Now!");
    }

}
