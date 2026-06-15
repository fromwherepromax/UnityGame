using System.Collections;
using UnityEngine;

public class UseItem : MonoBehaviour
{
    public void ApplyItemEffect(ItemSo itemSo)
    {
        if (itemSo == null)
        {
            return;
        }

        if (itemSo.currentHealth != 0)
        {
            StatsManager.Instance.UpdateHealth(itemSo.currentHealth);
        }

        if (itemSo.speed != 0)
        {
            StatsManager.Instance.UpdateSpeed(itemSo.speed);
        }
        if (itemSo.damage != 0)
        {
            StatsManager.Instance.UpdateDamage(itemSo.damage);
        }

        if (itemSo.arrowDamage != 0)
        {
            StatsManager.Instance.UpdateArrowDamage(itemSo.arrowDamage);
        }

        if (itemSo.explosionDamage != 0)
        {
            StatsManager.Instance.UpdateExplosionDamage(itemSo.explosionDamage);
        }

        if (itemSo.healAmount != 0)
        {
            StatsManager.Instance.UpdateHealAmount(itemSo.healAmount);
        }

        if (itemSo.duration > 0)
        {
            StartCoroutine(EffectTimer(itemSo, itemSo.duration));
        }
    }

    private IEnumerator EffectTimer(ItemSo itemSo, float duration)
    {
        yield return new WaitForSeconds(duration);

        if (itemSo.currentHealth != 0)
        {
            StatsManager.Instance.UpdateHealth(-itemSo.currentHealth);
        }

        if (itemSo.speed != 0)
        {
            StatsManager.Instance.UpdateSpeed(-itemSo.speed);
        }
        if (itemSo.damage != 0)
        {
            StatsManager.Instance.UpdateDamage(-itemSo.damage);
        }

        if (itemSo.arrowDamage != 0)
        {
            StatsManager.Instance.UpdateArrowDamage(-itemSo.arrowDamage);
        }

        if (itemSo.explosionDamage != 0)
        {
            StatsManager.Instance.UpdateExplosionDamage(-itemSo.explosionDamage);
        }

        if (itemSo.healAmount != 0)
        {
            StatsManager.Instance.UpdateHealAmount(-itemSo.healAmount);
        }
    }
}
