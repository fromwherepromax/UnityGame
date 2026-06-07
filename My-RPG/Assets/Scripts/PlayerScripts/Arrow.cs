// 脚本说明：Arrow 相关逻辑。
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{   
    public Rigidbody2D rb;
    public Vector2 direction=Vector2.right;
    public float lifetime=5f;
    public float speed=20f;
    public int damage=2;
    public float knockbackForce=5f;
    public float knockbackDuration=0.5f;
    public float stunTime=0.5f;

    public LayerMask enemyLayer;
    public LayerMask obstacleLayer;
    public SpriteRenderer sr;
    public Sprite burnedSprite;
    private bool hasHit;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity=direction*speed;
        RotateRow();
        Destroy(gameObject,lifetime);
    }
    private void RotateRow() //旋转箭头使其面向飞行方向
    {
        float angle=Mathf.Atan2(direction.y,direction.x)*Mathf.Rad2Deg;
        transform.rotation=Quaternion.Euler(new Vector3(0,0,angle));
    }
    public void OnTriggerEnter2D(Collider2D collision) //检测与敌人的碰撞
    {
        TryApplyHit(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryApplyHit(collision.collider);
    }

    private void TryApplyHit(Collider2D collision)
    {
        if (hasHit)
        {
            return;
        }

        // Ignore utility trigger colliders (for example, enemy detection ranges).
        if (collision.isTrigger)
        {
            return;
        }

        bool isEnemy = ((1 << collision.gameObject.layer) & enemyLayer.value) != 0;
        bool isObstacle = ((1 << collision.gameObject.layer) & obstacleLayer.value) != 0;

        if (!isEnemy && !isObstacle)
        {
            return;
        }

        if (isEnemy)
        {
            Enemy_Health enemyHealth = collision.GetComponentInParent<Enemy_Health>();
            Enemy_knockBack enemyKnockback = collision.GetComponentInParent<Enemy_knockBack>();

            if (enemyHealth != null)
            {
                enemyHealth.ChangeHealth(-damage);
            }

            if (enemyKnockback != null && enemyKnockback.gameObject.activeInHierarchy)
            {
                enemyKnockback.knockback(transform,knockbackForce,knockbackDuration,stunTime); //击退敌人
            }
        }

        AttachToTarget(collision.transform);
    }

    private void AttachToTarget(Transform target) //箭矢击中后附着在目标上
    {
        hasHit = true;

        if (sr != null && burnedSprite != null)
        {
            sr.sprite=burnedSprite;
        }

        rb.velocity=Vector2.zero;
        rb.isKinematic=true;
        rb.simulated = false;

        Collider2D arrowCollider = GetComponent<Collider2D>();
        if (arrowCollider != null)
        {
            arrowCollider.enabled = false;
        }

        transform.SetParent(target, true);
    }
}
