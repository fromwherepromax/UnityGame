using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CoverController : MonoBehaviour
{
    [Header("要控制的黑色遮罩")]
    public SpriteRenderer coverSprite;

    // 用于记录当前有多少个触发器里有玩家
    private int playersInTrigger = 0;

    // 当玩家进入任何一个触发器
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersInTrigger++;

            // 只要玩家进入任何一个触发器，就隐藏阴影
            if (playersInTrigger > 0)
            {
                coverSprite.enabled = false;
            }
        }
    }

    // 当玩家离开任何一个触发器
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersInTrigger--;

            // 只有当玩家离开了所有触发器，才恢复阴影
            if (playersInTrigger <= 0)
            {
                coverSprite.enabled = true;
            }
        }
    }
}