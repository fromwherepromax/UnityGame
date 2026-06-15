using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager1 : MonoBehaviour
{
    public static bool hasKey = false; // 记录是否拥有钥匙

    // 开始游戏时重置状态（可选）
    private void Start()
    {
        hasKey = false;
    }
}