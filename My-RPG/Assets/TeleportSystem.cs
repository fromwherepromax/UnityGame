using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TeleportSystem : MonoBehaviour
{
    [Header("传送目标")]
    public Transform destinationPortal;

    [Header("传送设置")]
    public float teleportDelay = 1.0f;

    // 全局传送冷却（所有传送门共享）
    private static bool globalCooldown = false;
    private static float cooldownTime = 2.0f; // 传送后2秒内不能再次传送

    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (globalCooldown) return;      // 🔥 全局冷却中，不触发传送
        if (isTeleporting) return;
        if (destinationPortal == null)
        {
            Debug.LogError("⚠️ 传送门缺少目标！请在 Inspector 里拖入另一个传送门。");
            return;
        }

        StartCoroutine(TeleportPlayer(other.gameObject));
    }

    private IEnumerator TeleportPlayer(GameObject player)
    {
        isTeleporting = true;

        // 1. 开启全局冷却（防止任何传送门在此期间被触发）
        globalCooldown = true;

        // 2. 等待玩家设定的延迟时间
        yield return new WaitForSeconds(teleportDelay);

        // 3. 传送玩家（保留Z轴，防止渲染错乱）
        Vector3 targetPos = destinationPortal.position;
        targetPos.z = player.transform.position.z;
        player.transform.position = targetPos;

        // 4. 🔥 强制重置玩家的渲染层级
        //SpriteRenderer playerRenderer = player.GetComponent<SpriteRenderer>();
        //if (playerRenderer != null)
        //{
        //    playerRenderer.sortingLayerName = "Default";
        //    playerRenderer.sortingOrder = 10;
        //}

        // 5. 玩家在冷却时间内无法触发任何传送
        yield return new WaitForSeconds(cooldownTime);
        globalCooldown = false;

        // 6. 解锁当前传送门
        yield return new WaitForSeconds(0.2f);
        isTeleporting = false;

        Debug.Log("✅ 传送完成，冷却结束，可以再次传送。");
    }
}