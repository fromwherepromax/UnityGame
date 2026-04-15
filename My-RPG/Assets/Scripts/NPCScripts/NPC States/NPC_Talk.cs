using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Talk : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public Animator interactAnim;
    public List<DialogueSO> conversations; //NPC可用的对话列表，可以在Unity编辑器中设置
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
        if (Input.GetButtonDown("Interact")) //按下交互键
        {
            if(DialogueManager.Instance.isDialogueActive) //如果对话已经激活
            {
                DialogueManager.Instance.AdvanceDialogue(); //推进对话
            }
            else
            {
                CheckForConversations(); //检查是否有可用的对话
                DialogueManager.Instance.StartDialogue(currentDialogue); //开始对话
            }
        }
    }
    private void CheckForConversations()
    {
        for(int i=0; i<conversations.Count; i++) //遍历对话列表
        {
            var convo=conversations[i];
            if(convo!=null && convo.CheckConditions()) //如果对话存在且条件满足
            {
                conversations.RemoveAt(i); //从列表中移除这个对话，确保每个对话只触发一次
                currentDialogue=convo; //设置当前对话
                break; //退出循环，使用第一个满足条件的对话
            }
        }
    }

}
