using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Wander : MonoBehaviour
{
    [Header("Wander Area")]
    public float wanderWith = 5f; //漫游区域宽度
    public float wanderHeight = 5f; //漫游区域高度
    public Vector2 startPosition; //漫游起点
    public float WaitingInterval = 1.5f; //漫游目标点切换间隔
    private bool isWaiting = false; //是否正在等待切换目标点
    public float speed = 1f; //漫游速度
    public Vector2 target;//漫游目标点
    private Rigidbody2D rb;
    private Animator anim;
    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }
    private void OnEnable()
    {
        StartCoroutine(WanderRoutine()); //启用时开始漫游
    }
    private void Update()
    {   
        if (isWaiting)
        {
            rb.velocity = Vector2.zero; //停止移动
            return;
        }
       if (Vector2.Distance(transform.position, target) < 0.1f) //当NPC接近目标点时，获取新的漫游目标点
        {
            StartCoroutine(WanderRoutine());
        }
        Move();
    }

    private void Move()
    {
        Vector2 direction = (target - rb.position).normalized; //计算NPC与目标点之间的方向向量
        if (direction.x < 0 && transform.localScale.x > 0 || direction.x > 0 && transform.localScale.x < 0) //根据移动方向调整NPC的朝向
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        rb.velocity = direction * speed; //设置NPC的速度，使其朝向目标点移动
    }
    IEnumerator WanderRoutine() //漫游协程函数，控制漫游目标点的切换
    {
        isWaiting = true;
        anim.Play("Idle");
        yield return new WaitForSeconds(WaitingInterval); //等待一段时间后切换
        target = GetRandomWanderPoint(); //获取新的漫游目标点
        isWaiting = false;
        anim.Play("Walk");
    }

    private void OnCollisionEnter2D(Collision2D collision) //当NPC与其他物体发生碰撞时，获取新的漫游目标点
    {   
        if(!enabled) return; //如果NPC已经被禁用，直接返回
        if (collision.gameObject.CompareTag("Obstacle")) //如果碰撞对象是障碍物
        {
            target = GetRandomWanderPoint(); //获取新的漫游目标点
        }
    }   

    private Vector2 GetRandomWanderPoint() //在漫游区域内随机生成一个目标点
    {
        float halfWidth = wanderWith / 2f; //计算漫游区域的半宽度
        float halfHeight = wanderHeight / 2f;
        int edge=Random.Range(0, 4); //随机选择一个边界
        return edge switch
        {
            0 => new Vector2(startPosition.x - halfWidth, Random.Range(startPosition.y - halfHeight, startPosition.y + halfHeight)), //左边界
            1 => new Vector2(startPosition.x + halfWidth, Random.Range(startPosition.y - halfHeight, startPosition.y + halfHeight)), //右边界
            2 => new Vector2(Random.Range(startPosition.x - halfWidth, startPosition.x + halfWidth), startPosition.y - halfHeight), //下边界
            _ => new Vector2(Random.Range(startPosition.x - halfWidth, startPosition.x + halfWidth), startPosition.y + halfHeight), //上边界
        };
    }

    private void OnDrawGizmosSelected()  //在编辑器中可视化漫游区域
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(startPosition, new Vector3(wanderWith, wanderHeight, 0));
    }
}
