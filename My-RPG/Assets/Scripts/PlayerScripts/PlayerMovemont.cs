using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerMovemont : MonoBehaviour
{

    public Rigidbody2D rb;
    public Animator anim;
    public int facingDirection=1;
    private bool isKnockback;
    public bool isShooting;

    public player_Combat player_Combat;
    private void Update()
    {
        if (Input.GetButtonDown("Slash"))
        {
            player_Combat.Attack();
        }
    }



    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        if (isShooting==true)
        {
            rb.velocity = Vector2.zero;
        }
        else if (isKnockback==false)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");



            if (horizontal > 0 && transform.localScale.x < 0 || horizontal < 0 && transform.localScale.x > 0)
            {
                Flip();
            }
            anim.SetFloat("horizontal", Mathf.Abs(horizontal));
            anim.SetFloat("vertical", Mathf.Abs(vertical));

            rb.velocity = new Vector2(horizontal, vertical) * StatsManager.Instance.speed;
        }


    }


    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);

    }


    public void Knockback(Transform enemy,float force,float stunTime)
    {
        isKnockback = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.velocity = direction*force;
        StartCoroutine(KnockbackCounter(stunTime));
    }


    IEnumerator KnockbackCounter(float stuntime)
    {
        yield return new WaitForSeconds(stuntime);
        rb.velocity = Vector2.zero;
        isKnockback = false;
    }


}
