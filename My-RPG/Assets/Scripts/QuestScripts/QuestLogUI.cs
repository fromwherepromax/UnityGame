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
    [SerializeField] private QuestSO noAvailableQuest; //没有可用任务时显示的默认任务数据
    [SerializeField] private QuestLogSlot[] questLogSlots; //任务日志槽位数组
    [SerializeField] private CanvasGroup questCanvas; //任务提供界面
    [SerializeField] private CanvasGroup acceptCanvas; //任务接受界面
    [SerializeField] private CanvasGroup declineCanvas; //任务拒绝界面
    [SerializeField] private CanvasGroup completeCanvas; //任务完成界面

    public void OnEnable() //当脚本启用时
    {
        QuestEvents.OnQuestofferRequested += ShowQuestOffer; //订阅任务提供事件
        QuestEvents.OnQuestTurnInRequested += ShowQuestTurnIn; //订阅任务交付事件
    }
    public void OnDisable() //当脚本禁用时
    {
        QuestEvents.OnQuestofferRequested -= ShowQuestOffer; //取消订阅任务提供事件
        QuestEvents.OnQuestTurnInRequested -= ShowQuestTurnIn; //取消订阅任务交付事件
    }

    public void ShowQuestOffer(QuestSO incomingQuest) //显示任务提供界面
    {   
        if(questManager.IsQuestAccepted(incomingQuest) || questManager.GetCompleteQuest(incomingQuest)) //如果任务已经被接受
        {
            questSo=noAvailableQuest; //显示没有可用任务的默认数据
            SetCanvasState(acceptCanvas, false); //显示任务接受界面
            SetCanvasState(declineCanvas, true); //显示任务拒绝界面
            SetCanvasState(completeCanvas, false); //隐藏任务完成界面
        }
        else
        {   
            questSo=incomingQuest; //显示传入的任务数据
            SetCanvasState(acceptCanvas, true); //显示任务接受界面
            SetCanvasState(declineCanvas, true); //显示任务拒绝界面
            SetCanvasState(completeCanvas, false); //隐藏任务完成界面
        }
        HandleQuestclick(questSo); //处理任务点击事件
        SetCanvasState(questCanvas, true); //显示任务提供界面
    }
    private void ShowQuestTurnIn(QuestSO incomingQuestSo) //显示任务交付界面
    {
        questSo = incomingQuestSo; //显示传入的任务数据
        HandleQuestclick(questSo); //处理任务点击事件
        SetCanvasState(acceptCanvas, false); //隐藏任务接受界面
        SetCanvasState(declineCanvas, false); //隐藏任务拒绝界面
        SetCanvasState(completeCanvas, true); //显示任务完成界面
        SetCanvasState(questCanvas, true); //显示任务提供界面
    }
    public void OnAcceptQuestClick() //当点击接受任务按钮时
    {
        if (questSo == null || questSo == noAvailableQuest)
        {
            return;
        }

        questManager.AcceptQuest(questSo); //接受当前任务
        QuestEvents.OnQuestAccepted?.Invoke(questSo); //触发任务被接受事件
        SetCanvasState(acceptCanvas, false); //隐藏任务接受界面
        SetCanvasState(completeCanvas, false); //隐藏任务完成界面
        SetCanvasState(declineCanvas, false); //隐藏任务拒绝界面
        RefreshQuestList(); //刷新任务列表
        HandleQuestclick(noAvailableQuest); //更新任务详情显示
    }
    public void OnDeclineQuestClick() //当点击拒绝任务按钮时
    {
        SetCanvasState(questCanvas, false); //隐藏任务提供界面
    }

    public void OnCompleteQuestClick() //当点击完成任务按钮时
    {
        questManager.CompleteQuest(questSo); //完成当前任务
        RefreshQuestList(); //刷新任务列表，移除已完成任务
        HandleQuestclick(noAvailableQuest); //更新任务详情显示
        SetCanvasState(completeCanvas, false); //隐藏任务提供界面
    }
    private void SetCanvasState(CanvasGroup canvasGroup, bool state) //设置界面状态
    {
        canvasGroup.alpha = state ? 1 : 0; //根据状态设置界面透明度
        canvasGroup.interactable = state; //根据状态设置界面交互
        canvasGroup.blocksRaycasts = state; //根据状态设置界面射线检测
    }

    public void RefreshQuestList() //刷新任务列表
    {
       List<QuestSO> activeQuests = questManager.GetActiveQuests(); //获取当前所有活跃的任务
        for (int i = 0; i < questLogSlots.Length; i++)
        {
            if (i < activeQuests.Count)
            {
                questLogSlots[i].SetQuest(activeQuests[i]); //设置槽位显示任务信息
            }
            else
            {
                questLogSlots[i].ClearSlot(); //清除多余的槽位信息
            }
        }
    }

    public void HandleQuestclick(QuestSO questSO) //处理任务点击事件
    {   
        this.questSo = questSO; //保存当前任务数据
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
