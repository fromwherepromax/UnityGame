using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR;

public class QuestLogUI : MonoBehaviour
{   
    [SerializeField] private QuestManager questManager; //引用任务管理器组件
    [SerializeField] private TMP_Text questNameText; //任务名称文本
    [SerializeField] private TMP_Text questDescriptionText; //任务描述文本
    [SerializeField] private QuestObjectiveSlot[] objectiveSlots; //任务目标槽位数组
    [SerializeField] private QuestRewardSlot[] rewardSlots; //任务奖励槽位数组

    private QuestSO questSo; //当前显示的任务数据
    // [SerializeField] private QuestSO noAvailableQuest; //没有可用任务时显示的默认任务数据
    // [SerializeField] private QuestLogSlot[] questLogSlots; //任务日志槽位数组
    // [SerializeField] private CanvasGroup questCanvas; //任务提供界面
    // [SerializeField] private CanvasGroup acceptCanvas; //任务接受界面
    // [SerializeField] private CanvasGroup declineCanvas; //任务拒绝界面
    // [SerializeField] private CanvasGroup completeCanvas; //任务完成界面
    public void HandleQuestclick(QuestSO questSO) //处理任务点击事件
    {   
        this.questSo = questSO; //保存当前点击的任务数据
        Debug.Log("Clicked on quest: " + questSO.questName);
        questNameText.text = questSO.questName; //更新任务名称文本
        questDescriptionText.text = questSO.questDescription; //更新任务描述文本
        DisplayObjective(); //显示任务目标
        DisplayRewards(); //显示任务奖励
    }
    private void DisplayObjective() //显示任务目标
    {
        for (int i = 0; i < objectiveSlots.Length; i++)
        {
            if (i < questSo.objectives.Count)
            {
                var objective = questSo.objectives[i];
                questManager.UpdateQuestProgress(questSo, objective); //更新任务进度
                int currentAmount = questManager.GetCurrentAmount(questSo, objective); //获取当前数量
                Debug.Log($"Objective: {objective.description}, Current Amount: {currentAmount}/{objective.requiredAmount}"); //调试输出目标信息
                string progressText = questManager.GetProgressText(questSo, objective); //获取进度文本
                bool isComplete = currentAmount >= objective.requiredAmount; //判断是否完成

                objectiveSlots[i].gameObject.SetActive(true); //显示目标槽位
                objectiveSlots[i].RefreshObjective(objective.description, progressText, isComplete); //刷新目标槽位信息

            }
            else
            {
                objectiveSlots[i].gameObject.SetActive(false); //隐藏多余的槽位
            }
        }
    }
    private void DisplayRewards() //显示任务奖励
    {
        for (int i = 0; i < rewardSlots.Length; i++)
        {
            if (i < questSo.rewards.Count)
            {
                var reward = questSo.rewards[i];
                rewardSlots[i].gameObject.SetActive(true); //显示奖励槽位
                rewardSlots[i].DisplayReward(reward.itemSo.itemIcon, reward.amount); //显示奖励信息
            }
            else
            {
                rewardSlots[i].gameObject.SetActive(false); //隐藏多余的槽位
            }
        }
    }
}
