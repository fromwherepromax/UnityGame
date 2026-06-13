using UnityEngine;

/// <summary>
/// 战斗遭遇配置，用于在对话结束后触发战斗
/// </summary>
[System.Serializable]
public class BattleEncounter
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;           // 敌人Prefab（如 Enemy_RedTorch）
    public Vector2 spawnPosition;            // 敌人生成位置

    [Header("Mid-Battle Dialogue (optional)")]
    [Range(0f, 1f)]
    public float midBattleDialogueThreshold = 0.5f; // 触发中间对话的血量比例（默认50%）
    public DialogueSO midBattleDialogue;     // 战斗中血量降到阈值时播放的对话

    [Header("Victory Dialogue")]
    public DialogueSO onVictoryDialogue;     // 击败敌人后播放的对话
}
