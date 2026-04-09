using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item")]
public class ItemSo : ScriptableObject
{
    public string itemName;
    [TextArea] public string itemDescription;
    public Sprite itemIcon;
    public bool isGold;
    public bool isExp;
    public int stackSize=3;

    [Header("Stats")]
    public int currentHealth;
    public int maxHealth;
    public int speed;
    public int damage;

    [Header("For Temporary Items")]
    public float duration;
}
