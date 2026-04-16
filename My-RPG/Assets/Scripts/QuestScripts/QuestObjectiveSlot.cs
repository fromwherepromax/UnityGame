using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestObjectiveSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text objectiveText; //目标描述文本
    [SerializeField] private TMP_Text trackingText; //目标进度文本

    public void RefreshObjective(string description, string progressText,bool isComplete) //刷新目标信息
    {
        objectiveText.text = description; //更新目标描述文本
        trackingText.text = progressText; //更新目标进度文本
        Color color = isComplete ? Color.gray : Color.white; //根据是否完成设置文本颜色
        objectiveText.color = color;
        trackingText.color = color;
    }
}
