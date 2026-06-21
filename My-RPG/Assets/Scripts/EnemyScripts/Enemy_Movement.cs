using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{

    public float speed;
    public float attackRange = 2;
    public float attackCooldown = 2;
    public float playerDetectRange = 5;
    public float detectionPauseTime = 1.5f; // 击退后暂停检测的时间
    public Transform detectionPiont;
    public LayerMask playerLayer;

    [Header("Wandering")]
    public float wanderRadius = 3f;           // 闲逛半径
    public float wanderSpeed = 1f;            // 闲逛速度（比追逐慢）
    public float wanderWaitTime = 2f;         // 到达目标点后等待时间
    public bool showWanderRange = true;       // 是否显示闲逛范围
    public Color wanderRangeColor = new Color(0f, 1f, 0f, 0.5f); // 闲逛范围颜色

    private float attackCooldownTimer;
    private float detectionPauseTimer;
    private int facingDirection=-1;
    private Transform player;
    private Animator anim;
    private EnemyState enemyState;
    private Rigidbody2D rb;
    private Enemy_Health health;

    private Vector2 startPosition;            // 初始位置
    private Vector2 wanderTarget;             // 当前闲逛目标
    private float wanderTimer;                // 等待计时器
    private bool isWaiting;                   // 是否在等待
    private LineRenderer wanderRangeRenderer; // 闲逛范围显示

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        health = GetComponent<Enemy_Health>();
        startPosition = transform.position;
        
        if (showWanderRange)
        {
            CreateWanderRangeVisual();
        }
        
        ChangeState(EnemyState.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyState!=EnemyState.knockback)
        {
            if (detectionPauseTimer > 0)
            {
                detectionPauseTimer -= Time.deltaTime;
            }
            else
            {
                CheckForPlayer();
            }

            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
            }

            // 状态行为
            switch (enemyState)
            {
                case EnemyState.Chasing:
                    Chase();
                    break;
                case EnemyState.Attacking:
                    rb.velocity = Vector2.zero;
                    break;
                case EnemyState.Wandering:
                    Wander();
                    break;
                case EnemyState.Idle:
                    rb.velocity = Vector2.zero;
                    if (isWaiting)
                    {
                        IdleWait();
                    }
                    break;
            }
        }


    }

    void Chase()  //追踪玩家
    {
        if (player.position.x > transform.position.x && facingDirection == -1 ||
        player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();

        }
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * speed;

    }
    
    void Wander()  //闲逛
    {
        // 移动到目标点
        Vector2 direction = (wanderTarget - (Vector2)transform.position).normalized;
        rb.velocity = direction * wanderSpeed;

        // 检查是否到达目标点
        if (Vector2.Distance(transform.position, wanderTarget) < 0.5f)
        {
            rb.velocity = Vector2.zero;
            isWaiting = true;
            wanderTimer = wanderWaitTime;
            ChangeState(EnemyState.Idle); // 切换到Idle状态
        }

        // 面向移动方向
        if ((direction.x > 0 && facingDirection == -1) || 
            (direction.x < 0 && facingDirection == 1))
        {
            Flip();
        }
    }

    void IdleWait()  //等待期间的处理
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0)
        {
            isWaiting = false;
            ChooseNewWanderTarget();
            ChangeState(EnemyState.Wandering); // 切换回Wandering状态
        }
    }

    void ChooseNewWanderTarget()
    {
        // 在初始位置周围随机选择目标点
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        wanderTarget = startPosition + randomDirection;
    }

    void CreateWanderRangeVisual()
    {
        // 创建 LineRenderer 来绘制圆形范围
        wanderRangeRenderer = gameObject.AddComponent<LineRenderer>();
        wanderRangeRenderer.useWorldSpace = true;
        wanderRangeRenderer.loop = true;
        wanderRangeRenderer.startWidth = 0.05f;
        wanderRangeRenderer.endWidth = 0.05f;
        wanderRangeRenderer.material = new Material(Shader.Find("Sprites/Default"));
        wanderRangeRenderer.startColor = wanderRangeColor;
        wanderRangeRenderer.endColor = wanderRangeColor;
        wanderRangeRenderer.sortingOrder = -1;

        // 生成圆形顶点
        int segments = 64;
        wanderRangeRenderer.positionCount = segments;
        UpdateWanderRangeVisual();
    }

    void UpdateWanderRangeVisual()
    {
        if (wanderRangeRenderer == null) return;

        int segments = wanderRangeRenderer.positionCount;
        Vector2 center = startPosition;

        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * 360f * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * wanderRadius;
            float y = Mathf.Sin(angle) * wanderRadius;
            wanderRangeRenderer.SetPosition(i, new Vector3(center.x + x, center.y + y, 0));
        }
    }
    
    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y * 1, transform.localScale.z);
    }

    private void OnDrawGizmosSelected()
    {
        // 玩家检测范围 —— 黄色
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(detectionPiont != null ? detectionPiont.position : transform.position, playerDetectRange);

        // 进入攻击状态的距离 —— 青色
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 闲逛范围 —— 绿色（半透明填充 + 边框）
        if (showWanderRange)
        {
            Vector2 center = Application.isPlaying ? startPosition : (Vector2)transform.position;
            Gizmos.color = new Color(wanderRangeColor.r, wanderRangeColor.g, wanderRangeColor.b, 0.15f);
            Gizmos.DrawSphere(center, wanderRadius);
            Gizmos.color = wanderRangeColor;
            Gizmos.DrawWireSphere(center, wanderRadius);
        }
    }

    private void CheckForPlayer()  //检查玩家是否在范围内
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPiont.position, playerDetectRange, playerLayer);

        if(hits.Length>0)
        {
            player = hits[0].transform;
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= attackRange && attackCooldownTimer <= 0)
            {
                attackCooldownTimer = attackCooldown;
                ChangeState(EnemyState.Attacking);
            }
            else if(distance > attackRange && enemyState != EnemyState.Attacking)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            // 没有检测到玩家时，进入闲逛状态（但不在等待期间打断）
            if (enemyState != EnemyState.Wandering && !(enemyState == EnemyState.Idle && isWaiting))
            {
                ChangeState(EnemyState.Wandering);
            }
        }
    }

    public void ChangeState(EnemyState newstate) //改变状态机状态
    {
        // 退出当前状态的动画
        switch (enemyState)
        {
            case EnemyState.Idle:
                anim.SetBool("isIdle", false);
                health?.StopRegen();
                break;
            case EnemyState.Wandering:
                anim.SetBool("isWandering", false);
                health?.StopRegen();
                break;
            case EnemyState.Chasing:
                anim.SetBool("isChasing", false);
                break;
            case EnemyState.Attacking:
                anim.SetBool("isAttacking", false);
                break;
        }

        // 击退结束时，暂停检测让敌人有时间进入回血
        if (newstate == EnemyState.Idle && enemyState == EnemyState.knockback)
        {
            detectionPauseTimer = detectionPauseTime;
        }
        
        enemyState = newstate;

        // 进入新状态的动画
        switch (enemyState)
        {
            case EnemyState.Idle:
                anim.SetBool("isIdle", true);
                health?.StartRegen();
                break;
            case EnemyState.Wandering:
                anim.SetBool("isWandering", true);
                health?.StartRegen();
                ChooseNewWanderTarget();
                isWaiting = false;
                break;
            case EnemyState.Chasing:
                anim.SetBool("isChasing", true);
                break;
            case EnemyState.Attacking:
                anim.SetBool("isAttacking", true);
                break;
        }
    }

    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(detectionPiont.position, playerDetectRange);

    // }




}



public enum EnemyState
{
    Idle,
    Wandering,
    Chasing,
    Attacking,
    knockback
}
