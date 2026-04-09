// 脚本说明：UseItem 相关逻辑。
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItem : MonoBehaviour
{
    public void ApplyItemEffect(ItemSo itemSo)
    {
        if(itemSo.currentHealth > 0)
        {
            StatsManager.Instance.UpdateHealth(itemSo.currentHealth);
        }
        if(itemSo.speed > 0)
        {
            StatsManager.Instance.UpdateSpeed(itemSo.speed);
        }
        if(itemSo.duration > 0)
        {
            StartCoroutine(EffectTimer(itemSo, itemSo.duration));
        }
    }

    private IEnumerator EffectTimer(ItemSo itemSo, float duration)
    {
        yield return new WaitForSeconds(duration);
         if(itemSo.currentHealth > 0)
        {
            StatsManager.Instance.UpdateHealth(-itemSo.currentHealth);
        }
        if(itemSo.speed > 0)
        {
            StatsManager.Instance.UpdateSpeed(-itemSo.speed);
        }
    }
}