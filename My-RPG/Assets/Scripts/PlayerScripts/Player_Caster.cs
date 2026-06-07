using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Caster : MonoBehaviour
{
    public Animator anim;
    public GameObject explosionPrefab;
    private float castTimer;

    [Header("生命值消耗")]
    [Tooltip("释放爆炸扣除的生命值比例（0.9 = 90%）")]
    public float hpCostRatio = 0.9f;

    void Update()
    {
        castTimer -= Time.deltaTime;

        // 生命值必须大于90%才能释放
        bool hasEnoughHp = StatsManager.Instance != null
            && StatsManager.Instance.CurrentHealth > StatsManager.Instance.MaxHealth * hpCostRatio;

        if (Input.GetMouseButtonDown(0) && castTimer <= 0 && hasEnoughHp)
        {
            anim.SetBool("isCasting", true);
        }
    }

    private void OnEnable()
    {
        anim.SetLayerWeight(0, 0);
        anim.SetLayerWeight(3, 1);
    }

    private void OnDisable()
    {
        if (anim != null)
        {
            anim.SetBool("isCasting", false);
            anim.SetLayerWeight(0, 1);
            anim.SetLayerWeight(3, 0);
        }
    }

    /// <summary>
    /// 由动画事件调用，在鼠标位置生成爆炸
    /// </summary>
    public void CastExplosion()
    {
        float cooldown = StatsManager.Instance != null ? StatsManager.Instance.explosionCooldown : 8f;

        if (castTimer > 0) return;

        // 获取鼠标世界坐标
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // 实例化爆炸预制体
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, mouseWorldPos, Quaternion.identity);
        }

        castTimer = cooldown;
        anim.SetBool("isCasting", false);

        // 扣除90%生命值
        int hpCost = Mathf.FloorToInt(StatsManager.Instance.CurrentHealth * hpCostRatio);
        PlayerHealth playerHealth = GetComponentInParent<PlayerHealth>();
        if (playerHealth != null && hpCost > 0)
        {
            playerHealth.ChangHealth(-hpCost);
        }
    }

    public void FinishCast()
    {
        anim.SetBool("isCasting", false);
    }
}
