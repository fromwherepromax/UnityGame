using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR;

public class QuestLogUI : MonoBehaviour
{   
    [SerializeField] private QuestManager questManager; //引用任务管理器组件
    public void HandleQuestclick(QuestSO questSO) //处理任务点击事件
    {   
        //在这里可以实现显示任务详情的逻辑，例如打开一个新的UI面板，显示任务描述、目标和奖励等信息
        Debug.Log("Clicked on quest: " + questSO.questName);
        
    }
}
