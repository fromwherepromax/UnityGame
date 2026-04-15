using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public DialogueManager dialogueManager; //对话管理器引用
    public DialogueHistoryTracker dialogueHistoryTracker; //对话历史记录器引用
    public LocationHistoryTracker locationHistoryTracker; //地点历史记录器引用

   [Header("Persistent Objects")]
   public GameObject[] persistentObjects;


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
}
