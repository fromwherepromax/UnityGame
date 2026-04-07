using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_knockBack : MonoBehaviour
{
    private Rigidbody2D rb;
    private Enemy_Movement enemy_Movement;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy_Movement = GetComponent<Enemy_Movement>();
    }

    public void knockback(Transform playerTransform,float knockbackForce,float knockbacktime, float stuntime)
    {
        enemy_Movement.ChangeState(EnemyState.knockback);
        StartCoroutine(StunTime(knockbacktime,stuntime));
        Vector2 direction = (transform.position - playerTransform.position).normalized;
        rb.velocity = direction * knockbackForce;
        Debug.Log("knockback applied");
    }
    IEnumerator StunTime(float knockbacktime, float stuntime)
    {
        yield return new WaitForSeconds(knockbacktime);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(stuntime);
        enemy_Movement.ChangeState(EnemyState.Idle);
    }


}
