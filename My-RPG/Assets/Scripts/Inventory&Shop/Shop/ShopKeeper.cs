using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShopKeeper : MonoBehaviour
{   
    public static ShopKeeper currentShopKeeper; //当前活跃的商店老板实例
    public Animator anim;
    private bool playerInRange;
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

    private void OnEnable()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.OnPanelClosed += HandlePanelClosed;
    }

    private void OnDisable()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.OnPanelClosed -= HandlePanelClosed;
    }

    public void Update()
    {
        if(playerInRange)
        {   
            if(Input.GetButtonDown("Interact"))
            {   
                if(!isShopOpen)
                {   
                    OpenShop();
                }
                else
                {   
                    CloseShop();
                }
            }
        }
    }

    /// <summary>
    /// 打开商店（通过 UIManager 管理面板可见性）
    /// </summary>
    public void OpenShop()
    {
        if (UIManager.Instance == null) return;

        currentShopKeeper = this;
        UIManager.Instance.OpenPanel(UIPanelType.Shop);
        isShopOpen = true;
        shopKeeperCam.transform.position = transform.position + CameraOffset;
        shopKeeperCam.gameObject.SetActive(true);
        OpenItemShop();
        OnShopStateChanged?.Invoke(shopManager, true);
    }

    /// <summary>
    /// 关闭商店
    /// </summary>
    public void CloseShop()
    {
        if (UIManager.Instance == null) return;

        currentShopKeeper = null;
        UIManager.Instance.ClosePanel(UIPanelType.Shop);
        isShopOpen = false;
        shopKeeperCam.gameObject.SetActive(false);
        OnShopStateChanged?.Invoke(shopManager, false);
    }

    /// <summary>
    /// 当面板被 UIManager 关闭时清理商店状态
    /// </summary>
    private void HandlePanelClosed(UIPanelType panelType)
    {
        if (panelType == UIPanelType.Shop && isShopOpen)
        {
            currentShopKeeper = null;
            isShopOpen = false;
            shopKeeperCam.gameObject.SetActive(false);
            OnShopStateChanged?.Invoke(shopManager, false);
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
