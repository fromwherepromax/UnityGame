using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public ItemSo itemSo;
    public SpriteRenderer sr;
    public Animator anim;
    public int quantity;
    public bool canBePickedUp = true;

    private void OnValidate()
    {
        UpdateAppearance();
    }
    private void UpdateAppearance()
    {
        if (itemSo != null)
        {
            sr.sprite = itemSo.itemIcon;
            this.name = itemSo.itemName;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canBePickedUp)
        {
            anim.Play("LootPickUp");
            Destroy(gameObject, 0.5f);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canBePickedUp = true;
        }
    }
    public void Initialize(ItemSo itemSo, int quantity)
    {
        this.itemSo = itemSo;
        this.quantity = quantity;
        canBePickedUp = false;
        UpdateAppearance();
    }

}
