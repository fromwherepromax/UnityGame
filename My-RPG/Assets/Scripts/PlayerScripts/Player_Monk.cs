using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Monk : MonoBehaviour
{
    public Animator anim;
    public int healAmount = 5;
    public float healCooldown = 5f;
    private float healTimer;

    void Update()
    {
        healTimer -= Time.deltaTime;

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
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(2, 0);
    }

    public void Heal()  // 由动画事件调用
    {
        if (healTimer <= 0)
        {
            PlayerHealth playerHealth = GetComponentInParent<PlayerHealth>();
            if (playerHealth != null && StatsManager.Instance.CurrentHealth < StatsManager.Instance.MaxHealth)
            {
                playerHealth.ChangHealth(healAmount);
            }
            healTimer = healCooldown;
        }
    }

    public void FinishHeal()
    {
        anim.SetBool("isHealing", false);
    }
}
