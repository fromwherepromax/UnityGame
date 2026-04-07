using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Bow : MonoBehaviour
{
    public PlayerMovemont pm;

    public Transform launchPoint;
    public GameObject arrowPrefb;

    public Vector2 aimDirection = Vector2.right;
    public float shootCooldown=.5f;
    private float shootTimer;
    public Animator anim;
    // Update is called once per frame

    private void OnEnable()
    {
        anim.SetLayerWeight(0, 0);
        anim.SetLayerWeight(1, 1);
    }
    private void OnDisable()
    {
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(1, 0);
    }


    void Update()
    {
        shootTimer -= Time.deltaTime;
        HandleAiming();
        if (Input.GetButtonDown("Shoot") && shootTimer<=0)
        {
            pm.isShooting = true;
            anim.SetBool("isShooting", true);
          
        }
    }

    private void HandleAiming()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vectical = Input.GetAxisRaw("Vertical");
        if (horizontal!=0||vectical!=0)
        {
            aimDirection = new Vector2(horizontal, vectical).normalized;
            
        }
    }
    public void Shoot()
    {
        if (shootTimer<=0)
        {
            Arrow arrow = Instantiate(arrowPrefb, launchPoint.position, Quaternion.identity).GetComponent<Arrow>();
            arrow.direction = aimDirection;
            shootTimer = shootCooldown;
        }

        anim.SetBool("isShooting", false);
        pm.isShooting = false;
    }
}
