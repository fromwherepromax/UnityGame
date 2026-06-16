using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGlow : MonoBehaviour
{
    [Header("闪烁设置")]
    public float blinkSpeed = 2.0f;        // 闪烁速度（越大闪越快）
    public float minAlpha = 0.2f;          // 最暗时透明度
    public float maxAlpha = 1.0f;          // 最亮时透明度

    [Header("旋转设置")]
    public float rotationSpeed = 50.0f;    // 旋转速度（度/秒）

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 1. 闪烁（透明度来回变化）
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(Time.time * blinkSpeed, 1));
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;

        // 2. 旋转
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}