using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Change : MonoBehaviour
{
    public player_Combat combat;
    public Player_Bow bow;
    public float switchCooldown = 0.5f;
    private float switchTimer;

    void Update()
    {
        if (switchTimer > 0)
        {
            switchTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("ChangeEquipment") && switchTimer <= 0)
        {
            switchTimer = switchCooldown;
            if (combat.enabled)
            {
                combat.enabled = false;
                bow.enabled = true;
            }
            else
            {
                combat.enabled = true;
                bow.enabled = false;
            }
        }
    }
}
