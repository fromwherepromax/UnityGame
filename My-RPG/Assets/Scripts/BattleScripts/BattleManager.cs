using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗流程管理器：负责生成敌人、监听击败事件、触发中间对话和胜利对话
/// 支持多种类、多数量敌人
/// </summary>
public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    private BattleEncounter currentEncounter;  // 当前战斗配置
    private List<GameObject> activeEnemies = new List<GameObject>(); // 所有存活的敌人
    private Enemy_Health mainEnemyHealth;      // 主敌血量组件（中间对话只看主敌）
    private bool midBattleDialogueTriggered;   // 中间对话是否已触发
    private bool battleActive;                 // 战斗是否正在进行

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        QuestEvents.OnQuestAccepted += OnQuestAccepted; //订阅任务接受事件
    }

    private void OnDisable()
    {
        QuestEvents.OnQuestAccepted -= OnQuestAccepted; //取消订阅任务接受事件
    }

    /// <summary>
    /// 任务被接受时，检查是否需要触发战斗
    /// </summary>
    private void OnQuestAccepted(QuestSO quest)
    {
        if (quest.battleEncounter != null && !battleActive)
        {
            Debug.Log($"[BattleManager] 任务 '{quest.questName}' 触发战斗！");
            StartBattle(quest.battleEncounter);
        }
    }

    /// <summary>
    /// 开始一场战斗
    /// </summary>
    public void StartBattle(BattleEncounter encounter)
    {
        if (encounter == null || encounter.enemies == null || encounter.enemies.Length == 0)
        {
            Debug.LogWarning("[BattleManager] BattleEncounter 或 enemies 为空！");
            return;
        }

        currentEncounter = encounter;
        midBattleDialogueTriggered = false;
        battleActive = true;
        activeEnemies.Clear();
        mainEnemyHealth = null;

        // 生成所有敌人
        foreach (var entry in encounter.enemies)
        {
            if (entry.enemyPrefab == null) continue;

            for (int i = 0; i < entry.count; i++)
            {
                // 多个敌人按间距水平排列，居中对齐
                float offsetX = 0f;
                if (entry.count > 1)
                {
                    offsetX = (i - (entry.count - 1) / 2f) * entry.spawnSpacing;
                }
                Vector2 pos = entry.spawnPosition + Vector2.right * offsetX;

                GameObject enemy = Instantiate(entry.enemyPrefab, pos, Quaternion.identity);
                activeEnemies.Add(enemy);

                Enemy_Health health = enemy.GetComponent<Enemy_Health>();
                if (health == null)
                {
                    Debug.LogError("[BattleManager] 敌人Prefab上没有 Enemy_Health 组件！");
                }
                // 记录主敌血量组件
                if (entry.isMainEnemy && mainEnemyHealth == null)
                {
                    mainEnemyHealth = health;
                }
            }
        }

        Debug.Log($"[BattleManager] 战斗开始！生成了 {activeEnemies.Count} 个敌人。");

        // 订阅击败事件
        Enemy_Health.OnEnemyDefeated += OnEnemyDefeated;
    }

    private void Update()
    {
        if (!battleActive)
            return;

        // 清理已死亡的敌人（SetActive(false)的）
        activeEnemies.RemoveAll(e => e == null || !e.activeInHierarchy);

        // 检查中间对话触发条件（只看主敌血量）
        if (!midBattleDialogueTriggered
            && mainEnemyHealth != null
            && currentEncounter.midBattleDialogue != null
            && currentEncounter.midBattleDialogueThreshold > 0f)
        {
            float healthPercent = (float)mainEnemyHealth.currentHealth / mainEnemyHealth.maxHealth;
            if (healthPercent <= currentEncounter.midBattleDialogueThreshold)
            {
                midBattleDialogueTriggered = true;
                TriggerMidBattleDialogue();
            }
        }

        // 所有敌人都被击败 → 战斗胜利
        if (activeEnemies.Count == 0)
        {
            OnBattleVictory();
        }
    }

    /// <summary>
    /// 触发战斗中间对话（任意敌人血量降到阈值时）
    /// </summary>
    private void TriggerMidBattleDialogue()
    {
        Debug.Log("[BattleManager] 触发战斗中间对话！");

        // 暂停所有敌人的行动
        SetAllEnemiesPaused(true);

        // 播放中间对话
        GameManager.Instance.dialogueManager.StartDialogue(currentEncounter.midBattleDialogue);

        // 启动协程等待对话结束后恢复战斗
        StartCoroutine(WaitForDialogueEnd());
    }

    /// <summary>
    /// 等待对话结束后恢复敌人行动
    /// </summary>
    private IEnumerator WaitForDialogueEnd()
    {
        yield return new WaitForSeconds(0.1f);

        while (GameManager.Instance.dialogueManager.isDialogueActive)
        {
            yield return null;
        }

        // 恢复所有敌人的行动
        SetAllEnemiesPaused(false);

        Debug.Log("[BattleManager] 中间对话结束，战斗继续！");
    }

    /// <summary>
    /// 暂停/恢复所有敌人的行动
    /// </summary>
    private void SetAllEnemiesPaused(bool paused)
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy == null) continue;

            var movement = enemy.GetComponent<Enemy_Movement>();
            if (movement != null) movement.enabled = !paused;

            var combat = enemy.GetComponent<En>();
            if (combat != null) combat.enabled = !paused;

            var rb = enemy.GetComponent<Rigidbody2D>();
            if (rb != null && paused) rb.velocity = Vector2.zero;
        }
    }

    /// <summary>
    /// 单个敌人被击败时的回调
    /// </summary>
    private void OnEnemyDefeated(int expReward)
    {
        Debug.Log("[BattleManager] 一个敌人被击败！获得经验: " + expReward);

        // 从列表中移除已死亡的敌人（下一帧 Update 也会清理）
        activeEnemies.RemoveAll(e => e == null || !e.activeInHierarchy);
    }

    /// <summary>
    /// 所有敌人被击败后的处理
    /// </summary>
    private void OnBattleVictory()
    {
        battleActive = false;

        // 取消订阅
        Enemy_Health.OnEnemyDefeated -= OnEnemyDefeated;

        Debug.Log("[BattleManager] 战斗胜利！所有敌人已被击败。");

        // 缓存胜利对话
        DialogueSO victoryDialogue = null;
        if (currentEncounter != null && currentEncounter.onVictoryDialogue != null)
        {
            victoryDialogue = currentEncounter.onVictoryDialogue;
        }

        currentEncounter = null;

        // 播放胜利对话
        if (victoryDialogue != null)
        {
            StartCoroutine(PlayVictoryDialogueDelayed(victoryDialogue));
        }
    }

    /// <summary>
    /// 延迟一小段时间后播放胜利对话
    /// </summary>
    private IEnumerator PlayVictoryDialogueDelayed(DialogueSO victoryDialogue)
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.dialogueManager.StartDialogue(victoryDialogue);
    }

    /// <summary>
    /// 当前是否有战斗正在进行
    /// </summary>
    public bool IsBattleActive()
    {
        return battleActive;
    }
}
