using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour
{
    [Header("要控制的粒子系统")]
    public ParticleSystem fogParticles;

    private ParticleSystem.EmissionModule emissionModule;

    void Start()
    {
        if (fogParticles != null)
        {
            emissionModule = fogParticles.emission;
        }
    }

    // 当玩家进入洞穴
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 停止发射新雾粒，雾会自己慢慢飘散
            emissionModule.enabled = false;

            // 如果想让雾瞬间消失，改用这一行：
            // fogParticles.Stop();
        }
    }

    // 当玩家走出洞穴
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 恢复发射雾粒
            emissionModule.enabled = true;
        }
    }
}
