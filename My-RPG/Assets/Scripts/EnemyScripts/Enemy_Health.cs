using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    private void Start()
    {
        StatsManager.Instance.CurrentHealth = StatsManager.Instance.MaxHealth;
    }


    public void ChangeHealth(int amount)
    {
        StatsManager.Instance.CurrentHealth += amount;
        if (StatsManager.Instance.CurrentHealth> StatsManager.Instance.MaxHealth)
        {
            StatsManager.Instance.CurrentHealth = StatsManager.Instance.MaxHealth;
        }

        else if (StatsManager.Instance.CurrentHealth <=0)
        {
            Destroy(gameObject);
        }


    }

}
