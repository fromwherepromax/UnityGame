using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalActivator : MonoBehaviour
{
    // 游戏开始时，传送门是隐藏的
    void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        // 如果玩家获得了钥匙，而且传送门还没激活
        if (GameManager1.hasKey && !gameObject.activeSelf)
        {
            // 激活传送门
            gameObject.SetActive(true);
        }
    }
}