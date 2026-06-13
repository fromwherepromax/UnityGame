using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestBoard : MonoBehaviour
{
    [SerializeField] private QuestSO questToOffer; //要提供的任务数据
    [SerializeField] private QuestSO questToTurnIn; //要交付的任务数据
    private bool playerInRange; //玩家是否在范围内

    private void Update() //每帧更新
    {
        if (playerInRange && Input.GetButtonDown("Interact")) //如果玩家在范围内并按下F键
        {   
            QuestSO questForTurnIn = questToTurnIn != null ? questToTurnIn : questToOffer;
            bool canTurnIn = questForTurnIn != null && QuestEvents.IsQuestComplete?.Invoke(questForTurnIn) == true; //检查是否可以交付任务
            if (canTurnIn)
            {
                QuestEvents.OnQuestTurnInRequested?.Invoke(questForTurnIn); //触发任务交付事件
            }
            else if(questToOffer != null && !GameManager.Instance.questManager.IsQuestAccepted(questToOffer) && !GameManager.Instance.questManager.GetCompleteQuest(questToOffer))
            {
                QuestEvents.OnQuestofferRequested?.Invoke(questToOffer); //只对未接取且未完成的任务触发提供事件
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) //当玩家进入范围时
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true; //设置玩家在范围内
           
        }
    }
    private void OnTriggerExit2D(Collider2D collision) //当玩家离开范围时
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false; //设置玩家不在范围内
        }
    }
}
