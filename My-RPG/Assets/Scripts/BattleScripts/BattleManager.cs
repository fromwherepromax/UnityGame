using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗流程管理器：负责生成敌人、监听击败事件、触发中间对话和胜利对话
/// </summary>
public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    private BattleEncounter currentEncounter;  // 当前战斗配置
    private GameObject activeEnemy;            // 当前活跃的敌人
    private Enemy_Health activeEnemyHealth;    // 当前敌人的血量组件
    private bool midBattleDialogueTriggered;   // 中间对话是否已触发（防止重复触发）
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

    /// <summary>
    /// 开始一场战斗
    /// </summary>
    public void StartBattle(BattleEncounter encounter)
    {
        if (encounter == null || encounter.enemyPrefab == null)
        {
            Debug.LogWarning("[BattleManager] BattleEncounter 或 enemyPrefab 为空！");
            return;
        }

        currentEncounter = encounter;
        midBattleDialogueTriggered = false;
        battleActive = true;

        // 生成敌人
        activeEnemy = Instantiate(encounter.enemyPrefab, encounter.spawnPosition, Quaternion.identity);
        activeEnemyHealth = activeEnemy.GetComponent<Enemy_Health>();

        if (activeEnemyHealth == null)
        {
            Debug.LogError("[BattleManager] 敌人Prefab上没有 Enemy_Health 组件！");
            return;
        }

        Debug.Log("[BattleManager] 战斗开始！敌人已生成。");

        // 订阅击败事件
        Enemy_Health.OnEnemyDefeated += OnEnemyDefeated;
    }

    private void Update()
    {
        if (!battleActive || activeEnemyHealth == null)
            return;

        // 检查中间对话触发条件
        if (!midBattleDialogueTriggered
            && currentEncounter.midBattleDialogue != null
            && currentEncounter.midBattleDialogueThreshold > 0f)
        {
            float healthPercent = (float)activeEnemyHealth.currentHealth / activeEnemyHealth.maxHealth;

            if (healthPercent <= currentEncounter.midBattleDialogueThreshold)
            {
                midBattleDialogueTriggered = true;
                TriggerMidBattleDialogue();
            }
        }
    }

    /// <summary>
    /// 触发战斗中间对话（血量降到阈值时）
    /// </summary>
    private void TriggerMidBattleDialogue()
    {
        Debug.Log("[BattleManager] 触发战斗中间对话！");

        // 暂停敌人的行动
        if (activeEnemy != null)
        {
            var movement = activeEnemy.GetComponent<Enemy_Movement>();
            if (movement != null) movement.enabled = false;

            var combat = activeEnemy.GetComponent<En>();
            if (combat != null) combat.enabled = false;

            var rb = activeEnemy.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;
        }

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
        // 等待对话开始
        yield return new WaitForSeconds(0.1f);

        // 等待对话结束
        while (GameManager.Instance.dialogueManager.isDialogueActive)
        {
            yield return null;
        }

        // 恢复敌人行动
        if (activeEnemy != null)
        {
            var movement = activeEnemy.GetComponent<Enemy_Movement>();
            if (movement != null) movement.enabled = true;

            var combat = activeEnemy.GetComponent<En>();
            if (combat != null) combat.enabled = true;
        }

        Debug.Log("[BattleManager] 中间对话结束，战斗继续！");
    }

    /// <summary>
    /// 敌人被击败时的回调
    /// </summary>
    private void OnEnemyDefeated(int expReward)
    {
        // 取消订阅，避免影响其他敌人
        Enemy_Health.OnEnemyDefeated -= OnEnemyDefeated;

        battleActive = false;
        activeEnemy = null;
        activeEnemyHealth = null;

        Debug.Log("[BattleManager] 战斗胜利！获得经验: " + expReward);

        // 在清空 currentEncounter 之前，先缓存胜利对话
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
    /// 延迟一小段时间后播放胜利对话（等敌人消失动画等）
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
