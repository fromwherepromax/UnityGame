using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 1.5f; // 交互距离


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("【PlayerInteraction】你按下了E键！");
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            // 使用 OverlapCircle 检测玩家周围一圈的所有碰撞体
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, interactRange);

            foreach (var hitCollider in hitColliders)
            {
                // 尝试发送交互消息
                Debug.Log("检测到附近物体: " + hitCollider.gameObject.name);
                hitCollider.gameObject.SendMessage("Interact", SendMessageOptions.DontRequireReceiver);
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            // 检查一下检测范围
            Debug.Log("你按了E，检测范围是: " + interactRange);

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, interactRange);
            Debug.Log("检测到的碰撞体数量: " + hitColliders.Length); // 看看是否 > 0

            foreach (var hitCollider in hitColliders)
            {
                Debug.Log("圈内检测到了物体: " + hitCollider.gameObject.name);
                // ...
            }
        }
    }

    // 在 Scene 视图里画个圆圈，方便调试距离
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}