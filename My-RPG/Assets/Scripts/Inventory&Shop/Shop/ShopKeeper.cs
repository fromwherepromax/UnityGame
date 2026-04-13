using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShopKeeper : MonoBehaviour
{   
    public static ShopKeeper currentShopKeeper; //当前活跃的商店老板实例
    public Animator anim;
    private bool playerInRange;
    public CanvasGroup shopCanvasGroup;
    private bool isShopOpen;
    public ShopManager shopManager;
    [SerializeField] private List<ShopItem> shopItems;  //商店物品列表，包含物品和价格
    [SerializeField] private List<ShopItem> shopWeapons;  //商店物品列表，包含物品和价格
    [SerializeField] private List<ShopItem> shopArmours;  //商店物品列表，包含物品和价格
    [SerializeField] private Camera shopKeeperCam; //商店界面专用相机
    [SerializeField] private Vector3 CameraOffset=new Vector3(0,0,-1); //商店界面相机偏移量
    public static event Action<ShopManager,bool> OnShopStateChanged; //商店状态改变事件，参数为商店管理器和商店是否打开

     private void Start()
    {
        shopManager.PopulateShopItems(shopItems);
    }
    public void Update()
    {
        if(playerInRange)
        {   
            if(Input.GetButtonDown("Interact"))
            {   
                if(!isShopOpen)
                {   
                    // Time.timeScale = 0f; // Pause the game
                    currentShopKeeper = this;
                    shopCanvasGroup.alpha = 1f;
                    OnShopStateChanged?.Invoke(shopManager, true);
                    shopCanvasGroup.interactable = true;
                    shopCanvasGroup.blocksRaycasts = true;
                    isShopOpen = true;
                    shopKeeperCam.transform.position = transform.position + CameraOffset;
                    shopKeeperCam.gameObject.SetActive(true);
                    OpenItemShop();
                }
                else
                {   
                    // Time.timeScale = 1f; // Resume the game
                    currentShopKeeper = null;
                    shopCanvasGroup.alpha = 0f;
                    OnShopStateChanged?.Invoke(shopManager, false);
                    shopCanvasGroup.interactable = false;
                    shopCanvasGroup.blocksRaycasts = false;
                    isShopOpen = false;
                    shopKeeperCam.gameObject.SetActive(false);
                }
            }
            
        }
    }
    public void OpenItemShop()
    {
        shopManager.PopulateShopItems(shopItems);
    }
    public void OpenWeaponShop()
    {
        shopManager.PopulateShopItems(shopWeapons);
    }
    public void OpenArmourShop()
    {
        shopManager.PopulateShopItems(shopArmours);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            anim.SetBool("playerInRange", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            anim.SetBool("playerInRange", false);
        }
    }

}
