using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject keyObject;          // 箱子里的钥匙
    public Sprite openedSprite;           // 箱子打开后的图片
    public GameObject portalObject;       // 【新增】要出现的传送门物体

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        if (!GameManager1.hasKey)
        {
            // 1. 获得钥匙
            GameManager1.hasKey = true;

            // 2. 隐藏钥匙（拿走）
            if (keyObject != null) keyObject.SetActive(false);

            // 3. 切换箱子图片
            if (animator != null) animator.enabled = false;
            if (openedSprite != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = openedSprite;
            }

            // 【4. 激活传送门】
            if (portalObject != null)
            {
                portalObject.SetActive(true);
                Debug.Log("传送门已出现！");
            }

            Debug.Log("获得钥匙！箱子已打开。");
        }
        else
        {
            Debug.Log("箱子已经空了。");
        }
    }
}