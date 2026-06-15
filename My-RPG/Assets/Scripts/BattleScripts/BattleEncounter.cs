using UnityEngine;

/// <summary>
/// 单个敌人的生成配置
/// </summary>
[System.Serializable]
public class EnemySpawnEntry
{
    public GameObject enemyPrefab;           // 敌人Prefab（如 Enemy_RedTorch、frogger 等）
    public Vector2 spawnPosition;            // 生成位置
    [Min(1)] public int count = 1;           // 生成数量
    public float spawnSpacing = 1.5f;        // 多个敌人之间的间距
    public bool isMainEnemy;                 // 是否为主敌（中间对话只看主敌血量）
}

/// <summary>
/// 战斗遭遇配置，用于在对话结束后触发战斗
/// </summary>
[System.Serializable]
public class BattleEncounter
{
    [Header("Enemy Settings")]
    public EnemySpawnEntry[] enemies;        // 敌人生成列表（支持多种类、多数量）

    [Header("Mid-Battle Dialogue (optional)")]
    [Range(0f, 1f)]
    public float midBattleDialogueThreshold = 0.5f; // 触发中间对话的血量比例（默认50%）
    public DialogueSO midBattleDialogue;     // 战斗中血量降到阈值时播放的对话
    public bool midDialogueTriggeredOnce = true; // 中间对话是否只触发一次

    [Header("Victory Dialogue")]
    public DialogueSO onVictoryDialogue;     // 击败所有敌人后播放的对话
}
