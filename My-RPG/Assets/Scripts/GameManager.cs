using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public UIManager uiManager; //UI管理器引用
    public DialogueManager dialogueManager; //对话管理器引用
    public DialogueHistoryTracker dialogueHistoryTracker; //对话历史记录器引用
    public LocationHistoryTracker locationHistoryTracker; //地点历史记录器引用
    public QuestManager questManager; //任务管理器引用
    public MusicManager musicManager; //音乐管理器引用
    public SettingManager settingManager; //设置管理器引用
    public BattleManager battleManager; //战斗管理器引用

   [Header("Persistent Objects")]
   public GameObject[] persistentObjects;

   [Header("Respawn")]
   public Vector3 respawnPosition = Vector3.zero;
   public float respawnDelay = 1.5f;


   private void Awake()
   {
       GameObject rootObject = transform.root.gameObject;

       if (Instance == null)
       {
           Instance = this;
           DontDestroyOnLoad(rootObject);
           MarkPersistentObjects();
           EnsureEventSystemExists();
       }
       else
       {    
           CleanUpAndDestroy(rootObject);
           return;
       }
   }

   private void OnEnable()
   {
       SceneManager.sceneLoaded += OnSceneLoaded;
   }

   private void OnDisable()
   {
       SceneManager.sceneLoaded -= OnSceneLoaded;
   }

   private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
   {
       EnsureEventSystemExists();
   }

   private void MarkPersistentObjects()
   {
       foreach (GameObject obj in persistentObjects)
       {
           if(obj != null)
           {
               DontDestroyOnLoad(obj);
           }
       }
   }

   private void CleanUpAndDestroy(GameObject currentRoot)
   {
       foreach (GameObject obj in persistentObjects)
       {
           if (obj == null)
           {
               continue;
           }

           // Keep EventSystem alive so UI remains clickable after scene switches.
           if (obj.GetComponent<EventSystem>() != null || obj.GetComponentInChildren<EventSystem>(true) != null)
           {
               continue;
           }

           Destroy(obj);
       }

       if (currentRoot != null)
       {
           Destroy(currentRoot);
       }
   }

   private void EnsureEventSystemExists()
   {
       EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();

       if (eventSystems.Length == 0)
       {
           GameObject eventSystemGO = new GameObject("EventSystem");
           eventSystemGO.AddComponent<EventSystem>();
           eventSystemGO.AddComponent<StandaloneInputModule>();
           return;
       }

       for (int i = 1; i < eventSystems.Length; i++)
       {
           if (eventSystems[i] != null)
           {
               Destroy(eventSystems[i].gameObject);
           }
       }
   }

   /// <summary>
   /// 玩家死亡时调用，延迟后传送到复活点并回满血
   /// </summary>
   public void RespawnPlayer(GameObject player)
   {
       StartCoroutine(RespawnCoroutine(player));
   }

   private IEnumerator RespawnCoroutine(GameObject player)
   {
       // 恢复时间流逝（防止面板打开时卡住）
       Time.timeScale = 1f;

       yield return new WaitForSeconds(respawnDelay);

       // 关闭所有打开的 UI 面板
       if (UIManager.Instance != null)
       {
           UIManager.Instance.CloseAllPanels();
       }

       if (player == null)
       {
           Debug.LogWarning("[GameManager] 复活失败：玩家引用为空！");
           yield break;
       }

       // 回满血
       if (StatsManager.Instance != null)
       {
           StatsManager.Instance.CurrentHealth = StatsManager.Instance.MaxHealth;
       }

       // 传送到复活点
       player.transform.position = respawnPosition;

       // 重新激活玩家
       player.SetActive(true);

       // 更新血量显示
       PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
       if (playerHealth != null && playerHealth.healthText != null)
       {
           playerHealth.healthText.text = "HP:" + StatsManager.Instance.CurrentHealth + "/" + StatsManager.Instance.MaxHealth;
       }

       Debug.Log($"[GameManager] 玩家已复活，传送至 {respawnPosition}");
   }
}
