using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestSO", menuName = "QuestSO")]
public class QuestSO : ScriptableObject //任务数据类
{
    public string questName; //任务名称
    public int questLevel; //任务等级
    [TextArea(3, 10)]
    public string questDescription; //任务描述

    public List<QuestObjective> objectives; //任务目标列表
    public List<QuestReward> rewards; //任务奖励列表
}

[System.Serializable]
public class QuestObjective //任务目标类
{
    public string description; //目标描述
    [SerializeField] private Object target; //目标对象，可以是物品、NPC或地点
    public ItemSo targetItem => target as ItemSo; //目标物品
    public ActorSO targetNpc => target as ActorSO; //目标NPC
    public LocationSo targetLocation => target as LocationSo; //目标地点
    public int requiredAmount; //需要的数量
}
[System.Serializable]
public class QuestReward //任务奖励类
{
    public ItemSo itemSo; //奖励物品，可以是经验、金币或其他物品
    public int amount; //奖励数量
}