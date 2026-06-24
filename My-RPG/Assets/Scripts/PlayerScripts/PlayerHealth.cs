using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class PlayerHealth : MonoBehaviour
{
    public TMP_Text healthText;
    public Animator healthTextAnim;

    public bool IsDead => StatsManager.Instance != null && StatsManager.Instance.CurrentHealth <= 0;

    private void Start()
    {
        healthText.text = "HP:" + StatsManager.Instance.CurrentHealth + "/" + StatsManager.Instance.MaxHealth;
    }

    public void ChangHealth(int amount)
    {
        if (amount < 0) MusicManager.Instance?.PlayFemaleHit();
        StatsManager.Instance.CurrentHealth += amount;
        StatsManager.Instance.CurrentHealth = Mathf.Clamp(StatsManager.Instance.CurrentHealth, 0, StatsManager.Instance.MaxHealth);
        healthTextAnim.Play("TextUPdate");


        healthText.text = "HP:" + StatsManager.Instance.CurrentHealth + "/" + StatsManager.Instance.MaxHealth;
        if (StatsManager.Instance.CurrentHealth<=0)
        {
            gameObject.SetActive(false);
            GameManager.Instance.RespawnPlayer(gameObject);
        }
    }
}
