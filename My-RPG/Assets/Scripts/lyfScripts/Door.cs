using UnityEngine;

public class Door : MonoBehaviour
{
    public Sprite openedSprite;           // 开门后的图片
    public GameObject hintText;           // 没钥匙时的提示文本
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;    // 门的碰撞体

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        // 🔥 确保锁住时是实心墙
        if (boxCollider != null)
        {
            boxCollider.isTrigger = false; // 不是触发器，是实体碰撞体
            boxCollider.enabled = true;    // 确保碰撞体启用
        }
    }

    public void Interact()
    {
        if (GameManager1.hasKey)
        {
            // 开门：换图片
            if (openedSprite != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = openedSprite;
            }

            // 🔥 开门：让门变成可通过（禁用碰撞体）
            if (boxCollider != null)
            {
                boxCollider.enabled = false; // 禁用碰撞体，玩家能通过了
            }

            // 禁用脚本，防止反复开门
            this.enabled = false;

            Debug.Log("门已打开！");
        }
        else
        {
            // 没钥匙：提示
            if (hintText != null)
            {
                StartCoroutine(ShowHint());
            }
        }
    }

    System.Collections.IEnumerator ShowHint()
    {
        hintText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        hintText.SetActive(false);
    }
}