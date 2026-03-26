using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Patrol : MonoBehaviour
{
    public Vector2[] patrolPoints;
    public float speed = 2f;
    public float waitTime = 1f;
    private bool isWaiting = true;
    private Vector2 target;
    private int currentPatrolIndex = 0;
    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        StartCoroutine(SetPatrolPoint());
    }

    void Update()
    {   if (isWaiting)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        Vector2 direction = (target - rb.position).normalized; //使用纯2D方向，避免z轴导致接近目标时减速
        if(direction.x<0 && transform.localScale.x>0 || direction.x>0 && transform.localScale.x<0) //根据移动方向调整NPC的朝向
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        rb.velocity = direction * speed;
        if (Vector2.Distance(rb.position, target) < 0.1f) //当NPC接近目标点时，停止移动并开始等待
        {
            StartCoroutine(SetPatrolPoint());
        }
    }
    IEnumerator SetPatrolPoint() //设置巡逻点  携程函数  等待一段时间后切换到下一个巡逻点
    {   
        isWaiting = true;
        anim.Play("Idle");
        yield return new WaitForSeconds(waitTime);

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        target = patrolPoints[currentPatrolIndex];
        isWaiting = false;
        anim.Play("Walk");
    }
}
