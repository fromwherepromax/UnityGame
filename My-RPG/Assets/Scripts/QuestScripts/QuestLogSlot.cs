using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestLogSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text questNameText; //任务名称文本
    [SerializeField] private TMP_Text questLevelText; //任务等级文本

    public QuestSO currentQuest;
    public QuestLogUI questLogUI; //引用任务日志UI组件

    private void OnValidate()
    {
        if (currentQuest != null)
        {
            SetQuest(currentQuest);
        }
        else
        {
            gameObject.SetActive(false); //如果没有任务数据，隐藏槽位
        }
    }

    public void SetQuest(QuestSO questSO) //设置任务信息
    {
        currentQuest = questSO;
        questNameText.text = questSO.questName;
        questLevelText.text = "Lv. " + questSO.questLevel.ToString();

        gameObject.SetActive(true); //显示槽位
    }
    public void ClearSlot() //清除槽位信息
    {
        currentQuest = null;
        gameObject.SetActive(false); //隐藏槽位
    }

    public void OnSlotClicked() //处理槽位点击事件
    {
        if (currentQuest != null)
        {
            questLogUI.HandleQuestclick(currentQuest); //调用任务日志UI的处理方法
        }
    }
}
