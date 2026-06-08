// 脚本说明：ExpManager 相关逻辑。
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ExpManager : MonoBehaviour
{
   public int level;
    public int currentExp = 0;
    public int expToNextLevel = 10;
    public float expMultiplier = 1.2f;
    public Slider expSlider;
    public TMP_Text currentLevelText;
    public static event Action<int> OnLevelUp;

    public void Start()
    {
        UpdateUI();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GainExp(2);
        }
    }
    private void OnEnable()
    {
        Enemy_Health.OnEnemyDefeated += GainExp;
        InventoryManager.OnExpGained += GainExp;
    }
    private void OnDisable()
    {
        Enemy_Health.OnEnemyDefeated -= GainExp;
        InventoryManager.OnExpGained -= GainExp;
    }

    public void GainExp(int amount)
    {
        currentExp += amount;
        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
        UpdateUI();
    }

    private void LevelUp()
    {
        level++;
        currentExp -= expToNextLevel;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * expMultiplier);
        OnLevelUp?.Invoke(1);
        MusicManager.Instance?.PlayLevelUp();
    }
    public void UpdateUI()
    {
        expSlider.maxValue=expToNextLevel;
        expSlider.value=currentExp;
        currentLevelText.text="Level: "+level.ToString();
    }
}
