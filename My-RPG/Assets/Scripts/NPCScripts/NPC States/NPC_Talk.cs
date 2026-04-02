using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Talk : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public Animator interactAnim;
    private void Awake()  
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }  
    private void OnEnable()
    {
        rb.velocity = Vector2.zero; //启用时停止NPC移动
        rb.isKinematic=true; //启用时设置刚体为Kinematic，防止物理干扰
        anim.Play("Idle"); //启用时播放闲置动画
        interactAnim.Play("OpenSpeechIcon"); //启用时播放交互动画
    }
    private void OnDisable()
    {
        interactAnim.Play("CloseSpeechIcon"); //禁用时播放交互结束动画
        rb.isKinematic=false; //禁用时恢复刚体物理属性
    }
}
