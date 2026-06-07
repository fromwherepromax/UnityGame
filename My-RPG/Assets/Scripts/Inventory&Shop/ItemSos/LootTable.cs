using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 掉落表 - ScriptableObject 数据资产
/// 用于配置敌人或可破坏物的掉落物列表。
/// 创建方式：在 Project 窗口右键 → Create → Loot → Loot Table
/// </summary>
[CreateAssetMenu(fileName = "New Loot Table", menuName = "Loot/Loot Table")]
public class LootTable : ScriptableObject
{
    /// <summary>
    /// 单条掉落条目，定义一个物品的掉落数量范围和概率。
    /// </summary>
    [System.Serializable]
    public class LootEntry
    {
        [Tooltip("要掉落的物品（ItemSo 引用）")]
        public ItemSo item;

        [Tooltip("最少掉落数量")]
        public int minQuantity = 1;

        [Tooltip("最多掉落数量（实际数量在此范围内随机）")]
        public int maxQuantity = 1;

        [Tooltip("掉落概率，0 = 不掉，1 = 必掉")]
        [Range(0f, 1f)]
        public float dropChance = 1f;
    }

    [Tooltip("掉落条目列表，每个条目独立判定是否掉落")]
    public List<LootEntry> lootEntries = new List<LootEntry>();

    /// <summary>
    /// 根据掉落表随机生成本次掉落的物品列表。
    /// 每个条目独立进行概率判定，命中后在 [minQuantity, maxQuantity] 范围内随机数量。
    /// </summary>
    /// <returns>本次掉落的 (物品, 数量) 列表</returns>
    public List<(ItemSo item, int quantity)> GetDrops()
    {
        List<(ItemSo, int)> drops = new List<(ItemSo, int)>();

        foreach (LootEntry entry in lootEntries)
        {
            // 按概率判定该条目是否掉落
            if (Random.value <= entry.dropChance)
            {
                // 在 [minQuantity, maxQuantity] 范围内随机数量
                int quantity = Random.Range(entry.minQuantity, entry.maxQuantity + 1);
                drops.Add((entry.item, quantity));
            }
        }

        return drops;
    }
}