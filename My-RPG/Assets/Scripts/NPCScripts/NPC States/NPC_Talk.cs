using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Talk : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public Animator interactAnim;
    public DialogueSO currentDialogue; //对话数据
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
    private void Update()
    {
        if (Input.GetButtonDown("Interact")) //按下F键触发对话
        {
            if(DialogueManager.Instance.isDialogueActive) //如果对话已经激活，结束对话
            {
                DialogueManager.Instance.AdvanceDialogue(); //推进对话
            }
            else //如果对话未激活，开始对话
            {
                DialogueManager.Instance.StartDialogue(currentDialogue); //开始当前对话
            }
        }
    }
}
