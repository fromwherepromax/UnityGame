using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TeleportWithDelay : MonoBehaviour
{
    [Header("传送设置")]
    public Transform targetDestination; // 目标传送点
    [Tooltip("传送前等待的秒数")]
    public float delayBeforeTeleport = 1.5f; // 这里就是延迟时间，1.5秒

    // 内部变量，用来防止玩家在传送读秒期间反复触发传送
    private bool isTeleporting = false;

    // 当玩家进入传送门的触发器区域
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查是不是玩家，并且当前没有正在传送读秒
        if (collision.CompareTag("Player") && !isTeleporting)
        {
            // 标记正在传送，防止重复触发
            isTeleporting = true;

            // 启动协程，开始读秒
            StartCoroutine(TeleportRoutine(collision));
        }
    }

    // 协程：处理延迟等待和实际传送
    IEnumerator TeleportRoutine(Collider2D playerCollider)
    {
        // 1. (可选) 在这里可以让传送门开始播放动画，或者冻结玩家移动
        // 比如：playerCollider.GetComponent<PlayerMovement>().enabled = false; 
        // 你需要根据自己的玩家控制器脚本替换这行

        // 2. 等待指定的秒数
        yield return new WaitForSeconds(delayBeforeTeleport);

        // 3. 执行传送！（前提是目标点没有丢失）
        if (targetDestination != null)
        {
            playerCollider.transform.position = targetDestination.position;
            Debug.Log("传送成功！");
        }
        else
        {
            Debug.LogError("错误：请把目标传送点拖拽给 Target Destination！");
        }

        // 4. (可选) 在这里恢复玩家移动
        // playerCollider.GetComponent<PlayerMovement>().enabled = true;

        // 5. 重置传送状态，允许下次进入传送门
        isTeleporting = false;
    }
}
