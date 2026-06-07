using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人掉落组件 - 挂在敌人 GameObject 上
/// 负责在敌人死亡时，根据 LootTable 掉落物生成到场景中。
/// 由 Enemy_Health.ChangeHealth() 在敌人血量归零时调用 DropLoot()。
/// </summary>
public class Enemy_Loot : MonoBehaviour
{
    [Header("掉落配置")]
    [Tooltip("掉落表数据，定义可掉落的物品、数量和概率")]
    public LootTable lootTable;

    [Tooltip("掉落物预制体（即场景中的 LootPerfab），需挂载 Loot 脚本")]
    public GameObject lootPrefab;

    [Tooltip("掉落物散开的半径范围，避免全部叠在一起")]
    public float dropRadius = 0.5f;

    /// <summary>
    /// 执行掉落逻辑。
    /// 1. 从 LootTable 获取本次掉落的物品列表
    /// 2. 在敌人位置周围随机偏移生成每个掉落物
    /// 3. 通过 Loot.Initialize() 设置物品数据和外观
    /// </summary>
    public void DropLoot()
    {
        // 安全检查：缺少配置则跳过
        if (lootTable == null || lootPrefab == null) return;

        // 从掉落表中随机生成本次掉落列表
        List<(ItemSo item, int quantity)> drops = lootTable.GetDrops();

        foreach (var drop in drops)
        {
            // 在敌人位置周围随机偏移，避免掉落物重叠
            Vector2 randomOffset = Random.insideUnitCircle * dropRadius;
            Vector3 spawnPos = transform.position + (Vector3)randomOffset;

            // 实例化掉落物预制体到场景中
            GameObject lootObj = Instantiate(lootPrefab, spawnPos, Quaternion.identity);

            // 初始化掉落物的物品数据和外观
            Loot loot = lootObj.GetComponent<Loot>();
            if (loot != null)
            {
                loot.Initialize(drop.item, drop.quantity);
            }
        }
    }
}
