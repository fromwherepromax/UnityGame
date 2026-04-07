using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;

    public float attackRange = 2;
    public float attackCooldown = 2;
    public float playerDetectRange = 5;
    public Transform detectionPiont;
    public LayerMask playerLayer;


    private float attackCooldownTimer;
    private int facingDirection=-1;
    private Transform player;
    private Animator anim;
    private EnemyState enemyState;

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyState!=EnemyState.knockback)
        {

            CheckForPlayer();
            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
            }



            if (enemyState == EnemyState.Chasing)
            {
                Chase();
            }
            else if (enemyState == EnemyState.Attacking)
            {
                rb.velocity = Vector2.zero;
            }
        }


    }

    void Chase()
    {




        if (player.position.x > transform.position.x && facingDirection == -1 ||
        player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();

        }



        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * speed;

    }




    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y * 1, transform.localScale.z);
    }





    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPiont.position, playerDetectRange, playerLayer);

        if(hits.Length>0)
        {
            player = hits[0].transform;

            if (Vector2.Distance(transform.position, player.position) <= attackRange && attackCooldownTimer <= 0)
            {
                attackCooldownTimer = attackCooldown;
                ChangeState(EnemyState.Attacking);
            }
            else if(Vector2.Distance(transform.position, player.position)>attackRange &&enemyState !=EnemyState.Attacking)
            {
                ChangeState(EnemyState.Chasing);
            }
            
        }
        else
        {
            rb.velocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
        }
    }



    public void ChangeState(EnemyState newstate)
    {
        if (enemyState==EnemyState.Idle)
        {
            anim.SetBool("isIdle", false);
        }
        else if (enemyState == EnemyState.Chasing)
        {
            anim.SetBool("isChasing", false);
        }
        else if (enemyState == EnemyState.Attacking)
        {
            anim.SetBool("isAttacking", false);
        }
        enemyState = newstate;

        if (enemyState == EnemyState.Idle)
        {
            anim.SetBool("isIdle", true);
        }
        else if (enemyState == EnemyState.Chasing)
        {
            anim.SetBool("isChasing", true);
        }
        else if (enemyState == EnemyState.Attacking)
        {
            anim.SetBool("isAttacking", true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPiont.position, playerDetectRange);

    }




}



public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    knockback
}
