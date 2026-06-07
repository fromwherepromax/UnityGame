using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class PlayerHealth : MonoBehaviour
{
    public TMP_Text healthText;
    public Animator healthTextAnim;

    private void Start()
    {
        healthText.text = "HP:" + StatsManager.Instance.CurrentHealth + "/" + StatsManager.Instance.MaxHealth;
    }

    public void ChangHealth(int amount)
    {
        StatsManager.Instance.CurrentHealth += amount;
        healthTextAnim.Play("TextUPdate");


        healthText.text = "HP:" + StatsManager.Instance.CurrentHealth + "/" + StatsManager.Instance.MaxHealth;
        if (StatsManager.Instance.CurrentHealth<=0)
        {
            gameObject.SetActive(false);
        }
    }
}
