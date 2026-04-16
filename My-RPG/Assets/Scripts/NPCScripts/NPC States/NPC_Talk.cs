using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Talk : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public Animator interactAnim;
    public List<DialogueSO> conversations; //对话选项列表
    public DialogueSO currentDialogue; //对话数据

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }  

    private void Start()
    {
        QuestEvents.OnQuestAccepted += OnQuestAccepted_RemoveOfferings; //订阅任务接受事件
    }
    private void OnDestroy()
    {
        QuestEvents.OnQuestAccepted -= OnQuestAccepted_RemoveOfferings; //取消订阅任务接受事件
    }
    private void OnEnable()
    {
        rb.velocity = Vector2.zero; //启用时停止NPC移动
        rb.isKinematic=true; //启用时设置刚体为Kinematic，防止物理干扰
        anim.Play("Idle"); //启用时播放闲置动画
        interactAnim.Play("Open"); //启用时播放交互动画
    }
    private void OnDisable()
    {
        interactAnim.Play("Close"); //禁用时播放交互结束动画
        rb.isKinematic=false; //禁用时恢复刚体物理属性
    }
    private void Update()
    {
        if (Input.GetButtonDown("Interact")) //按下交互键
        {
            if (GameManager.Instance == null || GameManager.Instance.dialogueManager == null)
            {
                Debug.LogWarning("DialogueManager is null.");
                return;
            }

            if(GameManager.Instance.dialogueManager.isDialogueActive) //如果对话已经激活
            {
               GameManager.Instance.dialogueManager.AdvanceDialogue(); //推进对话
            }
            else
            {   
                if(GameManager.Instance.dialogueManager.CanStartDialogue()) //如果可以开始对话
                {
                    CheckForConversations(); //检查是否有可用的对话
                    if (currentDialogue != null)
                    {
                        GameManager.Instance.dialogueManager.StartDialogue(currentDialogue); //开始新的对话
                    }
                    else
                    {
                        Debug.LogWarning("No valid dialogue found for NPC: " + gameObject.name);
                    }
                }
                
            }
        }
    }
    private void CheckForConversations() //检查对话条件
    {
        if (conversations == null || conversations.Count == 0)
        {
            currentDialogue = null;
            return;
        }

        currentDialogue = null;

        for(int i=0;i<conversations.Count;i++)
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
    private void OnQuestAccepted_RemoveOfferings(QuestSO acceptedQuest) //当任务被接受时检查是否需要移除提供的对话
    {
        if (acceptedQuest == null)
        {
            return;
        }

        for (int i = conversations.Count - 1; i >= 0; i--)
        {
            var convo = conversations[i];
            if (convo != null && convo.offerQuestOnEnd == acceptedQuest) //如果这个对话提供了被接受的任务
            {
                conversations.RemoveAt(i); //从列表中移除这个对话，确保它不再提供同一个任务
            }
        }
    }
}
