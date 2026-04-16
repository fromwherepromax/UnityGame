using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<QuestSO,Dictionary<QuestObjective,int>> questProgress = new Dictionary<QuestSO, Dictionary<QuestObjective, int>>(); //任务进度数据结构
    private List<QuestSO> completedQuests = new List<QuestSO>(); //已完成任务列表
    // public void OnEnable() //当脚本启用时
    // {
    //     QuestEvents.IsQuestComplete += IsQuestComplete; //订阅任务完成检查事件
    // }
    // public void OnDisable()
    // {
    //     QuestEvents.IsQuestComplete -= IsQuestComplete; //取消订阅任务完成检查事件
    // }
    // #region  Quest Accept Logic
    // public bool IsQuestAccepted(QuestSO questSO) //检查任务是否已接受
    // {
    //     return questProgress.ContainsKey(questSO); //检查任务进度字典中是否包含该任务
    // }
    // public List<QuestSO> GetActiveQuests() //获取当前所有活跃的任务
    // {
    //     return new List<QuestSO>(questProgress.Keys); //返回任务进度字典中的所有任务
    // }
    // public void AcceptQuest(QuestSO questSO) //接受任务
    // {
    //     questProgress[questSO] = new Dictionary<QuestObjective, int>(); //在任务进度字典中添加一个新的任务条目
    //     foreach (var objective in questSO.objectives)
    //     {   
    //         UpdateQuestProgress(questSO, objective); //更新任务进度
    //     }
    // }
    // #endregion

    // public bool IsQuestComplete(QuestSO questSO) //检查任务是否完成
    // {
    //     if (!questProgress.TryGetValue(questSO, out var progressDict)) //如果任务不在字典中，返回false
    //     {
    //         return false;
    //     }
    //     foreach (var objective in questSO.objectives) //检查每个任务目标的进度
    //     {
    //         UpdateQuestProgress(questSO, objective); //更新任务进度
    //     }
    //     foreach (var objective in questSO.objectives) //检查每个任务目标是否完成
    //     {
    //         if(progressDict[objective] < objective.requiredAmount) //如果当前数量小于要求数量，返回false
    //         {
    //             return false;
    //         }
    //     }
    //     return true; //如果所有目标都完成，返回true
    // }
    public void UpdateQuestProgress(QuestSO questSO, QuestObjective objective) //更新任务进度
    {
        if (!questProgress.ContainsKey(questSO))
        {
            return; //如果任务不在字典中，直接返回
        }
        var processDict = questProgress[questSO];
        int newAmount = 0;
        if(objective.targetItem != null)
        {
            newAmount = InventoryManager.Instance.GetItemQuantity(objective.targetItem); //如果目标是物品，获取当前数量
        }
        else if(objective.targetNpc != null && GameManager.Instance.dialogueHistoryTracker.HasSpokenWith(objective.targetNpc))
        {
            newAmount = 1; //如果目标是NPC，直接设置为已完成
        }
         else if(objective.targetLocation != null && GameManager.Instance.locationHistoryTracker.HasVisited(objective.targetLocation))
        {
            newAmount = 1; //如果目标是地点，直接设置为已完成
        }

        processDict[objective] = newAmount; //更新目标的当前数量
    }

    public string GetProgressText(QuestSO questSO, QuestObjective objective) //获取任务目标的进度
    {
        int currentAmount = GetCurrentAmount(questSO, objective); //获取当前数量

        if (currentAmount>= objective.requiredAmount)
        {
            return "Completed"; //如果当前数量达到或超过要求，返回已完成
        }
        else if(objective.targetItem !=null)
        {
            return $"{currentAmount}/{objective.requiredAmount}"; //否则返回当前数量和要求数量
        }
        else
        {
            return "In Progress"; //如果目标不是物品，返回进行中
        }
    }
    public int GetCurrentAmount(QuestSO questSO, QuestObjective objective) //获取当前数量
    {
        if (questProgress.TryGetValue(questSO, out var objectiveProgress) && objectiveProgress.TryGetValue(objective, out int currentAmount))
        {
            return currentAmount; //如果任务和目标在字典中，返回当前数量
        }
        return 0; //否则返回0
    }
    // public void CompleteQuest(QuestSO questSO) //完成任务
    // {   
    //     completedQuests.Add(questSO); //将任务添加到已完成列表中
    //     questProgress.Remove(questSO); //从任务进度字典中移除该任务
    //     //这里可以添加完成任务的逻辑，例如给予奖励、更新任务状态等
    //     foreach(var objective in questSO.objectives)
    //     {
    //         if(objective.targetItem != null && objective.requiredAmount>0)
    //          {
    //              InventoryManager.Instance.RemoveItem(objective.targetItem, objective.requiredAmount); //移除任务目标物品
    //          }
    //     }
    //     foreach (var reward in questSO.rewards)
    //     {
    //         InventoryManager.Instance.AddItem(reward.itemSo, reward.amount); //给予奖励物品
    //     }
    // }
    // public bool GetCompleteQuest(QuestSO questSO) //检查任务是否已完成
    // {
    //     return completedQuests.Contains(questSO); //检查已完成列表中是否包含该任务
    // }

}
