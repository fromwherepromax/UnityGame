using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Change : MonoBehaviour
{
    public player_Combat combat;
    public Player_Bow bow;

    void Update()
    {
        if (Input.GetButtonDown("ChangeEquipment"))
        {
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
