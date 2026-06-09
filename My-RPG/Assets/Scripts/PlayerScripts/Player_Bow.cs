// 脚本说明：Player_Bow 相关逻辑。
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Bow : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject arrowPrefab;
    private Vector2 aimDirection=Vector2.right;
    public float shootCooldown=0.5f;
    private float shootTimer;
    public Animator anim;

    /// <summary>
    /// 冷却进度：0 = 就绪，1 = 刚释放完（冷却最长）
    /// </summary>
    public float CooldownProgress
    {
        get
        {
            return shootCooldown > 0 ? Mathf.Clamp01(shootTimer / shootCooldown) : 0f;
        }
    }

    // Update is called once per frame
    void Update()  
    {   
        shootTimer-=Time.deltaTime;
        HandleAming();
        HandleFacingToMouse();
        if (Input.GetMouseButtonDown(0) && shootTimer<=0)
        {   
            anim.SetBool("isShooting",true);
        }
       
    }
    private void OnEnable()
    {
        anim.SetLayerWeight(0,0);
        anim.SetLayerWeight(1,1);
    }
    private void OnDisable()
    {
        anim.SetBool("isShooting", false);
        anim.SetLayerWeight(0,1);
        anim.SetLayerWeight(1,0);
    }

    public void Shoot()  //实例化箭矢并设置其飞行方向
    {   if (shootTimer<=0)
        {
            Arrow arrow = Instantiate(arrowPrefab, launchPoint.position,Quaternion.identity).GetComponent<Arrow>();
            arrow.direction=aimDirection;
            shootTimer=shootCooldown;
            anim.SetBool("isShooting",false);
            MusicManager.Instance?.PlayArcherShoot();
        }
    }
    public void HandleAming() //根据输入更新瞄准方向
    {
        if (launchPoint == null || Camera.main == null)
        {
            return;
        }

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = launchPoint.position.z;

        Vector2 newDirection = (mouseWorldPosition - launchPoint.position).normalized;
        if (newDirection.sqrMagnitude > 0.0001f)
        {
            aimDirection = newDirection;
        }
    }

    private void HandleFacingToMouse()
    {
        if (Camera.main == null)
        {
            return;
        }

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float deltaX = mouseWorldPosition.x - transform.position.x;

        if (Mathf.Abs(deltaX) < 0.01f)
        {
            return;
        }

        Vector3 scale = transform.localScale;
        float expectedX = deltaX > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        if (!Mathf.Approximately(scale.x, expectedX))
        {
            scale.x = expectedX;
            transform.localScale = scale;
        }
    }
}
