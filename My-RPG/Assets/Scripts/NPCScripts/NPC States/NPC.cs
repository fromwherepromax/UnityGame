using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public enum NPCState { Default,Patrol, Wander, Talk ,Idle}
    public NPCState currentState=NPCState.Wander;
    private NPCState defaultState;
    public NPC_Patrol patrol;
    public NPC_Wander wander;
    public NPC_Talk talk;
    private Rigidbody2D rb;
    private Animator anim;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        defaultState = currentState; //记录初始状态
        SwitchState(currentState); //根据初始状态启用对应的行为
    }

    public void SwitchState(NPCState newState) //切换状态函数，根据传入的新状态启用对应的行为组件，并禁用其他组件
    {
        currentState = newState;
        patrol.enabled = (currentState == NPCState.Patrol);
        wander.enabled = (currentState == NPCState.Wander);
        talk.enabled = (currentState == NPCState.Talk);

        // 当切换到 Idle 或 Default 状态时，停止移动并播放 Idle 动画
        if (currentState == NPCState.Idle || currentState == NPCState.Default)
        {
            if (rb != null) rb.velocity = Vector2.zero;
            if (anim != null) anim.Play("Idle");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) //当玩家进入NPC的触发范围时，切换到交互状态
    {
        if (collision.CompareTag("Player"))
        {
            SwitchState(NPCState.Talk);
        }
    }
    private void OnTriggerExit2D(Collider2D collision) //当玩家离开NPC的触发范围时，切换回默认状态
    {
        if (collision.CompareTag("Player"))
        {
            SwitchState(defaultState);
        }
    }
}
