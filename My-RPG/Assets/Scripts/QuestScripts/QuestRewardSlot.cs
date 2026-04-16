using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestRewardSlot : MonoBehaviour
{
    public Image rewardImage;  //奖励图标
    public TMP_Text rewardQuantity;  //奖励数量文本

    public void DisplayReward(Sprite sprite, int quantity)
    {
        rewardImage.sprite = sprite; //设置奖励图标
        rewardQuantity.text = quantity.ToString(); //设置奖励数量文本
    }
}
