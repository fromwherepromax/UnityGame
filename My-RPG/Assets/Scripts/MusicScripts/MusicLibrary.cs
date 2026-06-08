using UnityEngine;

/// <summary>
/// 音乐库 ScriptableObject —— 在 Project 窗口右键 Create → Music → Music Library 创建
/// </summary>
[CreateAssetMenu(fileName = "MusicLibrary", menuName = "Music/Music Library")]
public class MusicLibrary : ScriptableObject
{
    [System.Serializable]
    public class SceneBGM
    {
        public string sceneName;           // 场景名（须与 Build Settings 中一致）
        public AudioClip bgmClip;          // 对应的背景音乐
        [Range(0f, 1f)]
        public float volume = 0.7f;        // 该场景的 BGM 音量
    }

    [Header("场景背景音乐映射")]
    [Tooltip("每个条目将一个场景名映射到一首 BGM")]
    public SceneBGM[] sceneBGMs;

    /// <summary>
    /// 根据场景名查找对应的 BGM 条目
    /// </summary>
    public SceneBGM GetBGMForScene(string sceneName)
    {
        if (sceneBGMs == null) return null;
        foreach (var entry in sceneBGMs)
        {
            if (entry.sceneName == sceneName)
                return entry;
        }
        return null;
    }
}
