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
            if(GameManager.Instance.dialogueManager.isDialogueActive) //如果对话已经激活
            {
                GameManager.Instance.dialogueManager.AdvanceDialogue(); //推进对话
            }
            else
            {   
                if(GameManager.Instance.dialogueManager.CanStartDialogue()) //如果可以开始对话
                {
                    CheckForConversations(); //检查是否有可用的对话
                    GameManager.Instance.dialogueManager.StartDialogue(currentDialogue); //开始对话
                }
            }
        }
    }
    private void CheckForConversations()
    {
        for(int i=0; i<conversations.Count; i++) //遍历对话列表
        {
            var convo = conversations[i];
            if(convo!=null && convo.CheckConditions()) //如果对话存在且满足条件
            {    
                    currentDialogue = convo; //设置当前对话
                    if(convo.removeAfterPlay) //如果需要播放后移除
                    {
                        conversations.RemoveAt(i); //从列表中移除这个对话，确保每个对话只触发一次
                    }
                    if(convo.removeTheseOnPlay!=null) //如果需要播放后移除其他对话
                    {
                        foreach(var toRemove in convo.removeTheseOnPlay)
                        {
                            conversations.Remove(toRemove); //从列表中移除这些对话，确保每个对话只触发一次
                        }
                    }
                    break; //退出循环，优先使用满足条件的第一个对话
            }
        }
    }

}
