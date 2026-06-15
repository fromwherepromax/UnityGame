using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "Diologue/DialogueNode")] //创建一个ScriptableObject，命名为DialogueSO，并在Unity编辑器中添加一个菜单项来创建这个ScriptableObject
public class DialogueSO : ScriptableObject
{
    public DialogueLine[] dialogueLines; //对话行数组
    public DialogueOption[] dialogueOptions; //对话选项数组

    [Header("Conditions Required (optional)")] //条件
    public ActorSO[] requiredNPC; //需要的角色
    public LocationSo[] requiredLocations; //需要的地点
    public ItemSo[] requiredItems; //需要的物品  

    [Header("Control Flags")]//控制标志
    public bool removeAfterPlay; //播放后移除
    public List<DialogueSO> removeTheseOnPlay; //播放后移除这些对话

    [Header("Quest offered (optional)")] //提供的任务
    public QuestSO offerQuestOnEnd; //提供的任务数据

    [Header("Complete Quest (optional)")] //完成的任务
    public QuestSO[] requireCompleteQuests; //完成的任务数据

    [Header("Complete Turn-In Quest (optional)")] //交付的任务
    public QuestSO turnInQuest; //交付的任务数据

    [Header("Battle Trigger (optional) (optional)")] //战斗触发
    public BattleEncounter battleEncounter; //对话结束后触发的战斗配置

    [Header("Teleport (optional)")] //传送
    public bool shouldTeleport; //对话结束后是否传送
    public Vector2 teleportPosition; //传送目标坐标

    public bool CheckConditions() //检查条件是否满足
    {
        if (requiredNPC != null && requiredNPC.Length > 0)
        {
            foreach (var npc in requiredNPC) //遍历需要的角色
            {
                if (!GameManager.Instance.dialogueHistoryTracker.HasSpokenWith(npc)) //如果没有与某个角色对话过
                {
                    Debug.Log($"[DialogueSO] '{name}' 条件失败: 未与NPC '{npc.actorName}' 对话过");
                    return false; //条件不满足
                }
            }
        }
        if(requiredLocations != null && requiredLocations.Length > 0)
        {
            foreach (var location in requiredLocations) //遍历需要的地点
            {
                if (!GameManager.Instance.locationHistoryTracker.HasVisited(location)) //如果没有访问过某个地点
                {
                    Debug.Log($"[DialogueSO] '{name}' 条件失败: 未访问过地点 '{location.locationName}'");
                    return false; //条件不满足
                }
            }
        }
        if(requiredItems != null && requiredItems.Length > 0)
        {
            foreach (var item in requiredItems) //遍历需要的物品
            {
                if (!InventoryManager.Instance.HasItem(item)) //如果没有某个物品
                {
                    Debug.Log($"[DialogueSO] '{name}' 条件失败: 缺少物品 '{item.itemName}'");
                    return false; //条件不满足
                }
            }
        }
        if(requireCompleteQuests != null && requireCompleteQuests.Length > 0)
        {
            foreach (var quest in requireCompleteQuests) //遍历需要完成的任务
            {
                if (!GameManager.Instance.questManager.GetCompleteQuest(quest)) //如果没有完成某个任务（检查已完成列表）
                {
                    Debug.Log($"[DialogueSO] '{name}' 条件失败: 未完成任务 '{quest.questName}'");
                    return false; //条件不满足
                }
            }
        }
        return true; //条件满足
    }
}



[System.Serializable] 
public class DialogueLine
{
    public ActorSO speaker; //说话的角色

    [TextArea(3, 5)]
    public string line; //对话内容
}

[System.Serializable]
public class DialogueOption
{
    public string optionText; //选项文本
    public DialogueSO nextDialogue; //选择这个选项后进入的下一个对话
    public QuestSO offerQuest; //选择这个选项后提供的任务
}

