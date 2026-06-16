using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class evalation : MonoBehaviour
{
    // 将此变量暴露在 Inspector 面板中，方便拖拽赋值
    [SerializeField]
    private Collider2D colliderToDisable;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查进入触发器的是否是玩家
        if (collision.gameObject.CompareTag("Player"))
        {
            // 1. 禁用指定的碰撞体，而不是数组中的所有碰撞体
            if (colliderToDisable != null)
            {
                colliderToDisable.enabled = false;
            }

            // 2. 调整玩家的渲染层级
            SpriteRenderer playerSprite = collision.gameObject.GetComponent<SpriteRenderer>();
            if (playerSprite != null)
            {
                playerSprite.sortingOrder = 15;
            }
        }
    }

    // 可选：当玩家离开触发器时，重新启用碰撞体
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (colliderToDisable != null)
            {
                colliderToDisable.enabled = true;
            }

            SpriteRenderer playerSprite = collision.gameObject.GetComponent<SpriteRenderer>();
            if (playerSprite != null)
            {
                playerSprite.sortingOrder = 0; // 或其他默认值
            }
        }
    }
}
