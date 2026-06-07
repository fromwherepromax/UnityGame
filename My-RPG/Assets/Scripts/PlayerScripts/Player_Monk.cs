using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Monk : MonoBehaviour
{
    public Animator anim;
    private float healTimer;

    void Update()
    {
        healTimer -= Time.deltaTime;
        float cooldown = StatsManager.Instance != null ? StatsManager.Instance.healCooldown : 5f;

        if (Input.GetMouseButtonDown(0) && healTimer <= 0)
        {
            anim.SetBool("isHealing", true);
        }
    }

    private void OnEnable()
    {
        anim.SetLayerWeight(0, 0);
        anim.SetLayerWeight(2, 1);
    }

    private void OnDisable()
    {
        anim.SetBool("isHealing", false);
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(2, 0);
    }

    public void Heal()  // 由动画事件调用
    {
        if (healTimer <= 0)
        {
            int amount = StatsManager.Instance != null ? StatsManager.Instance.healAmount : 5;
            float cooldown = StatsManager.Instance != null ? StatsManager.Instance.healCooldown : 5f;

            PlayerHealth playerHealth = GetComponentInParent<PlayerHealth>();
            if (playerHealth != null && StatsManager.Instance.CurrentHealth < StatsManager.Instance.MaxHealth)
            {
                playerHealth.ChangHealth(amount);
            }
            healTimer = cooldown;
        }
    }

    public void FinishHeal()
    {
        anim.SetBool("isHealing", false);
    }
}
