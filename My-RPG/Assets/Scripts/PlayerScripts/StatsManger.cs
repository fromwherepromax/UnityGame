using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;


    [Header("Combat Stats")]
    public int damage;
    public float weaponRange;
    public float knockbackforce;
    public float knockbacktime;
    public float stuntime;

    [Header("Movement Stats")]
    public int speed;


    [Header("Health Stats")]
    public int MaxHealth;
    public int CurrentHealth;

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


}
