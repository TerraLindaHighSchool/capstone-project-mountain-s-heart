using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaisicMovment : MonoBehaviour
{
    public float moveSpeed;
    public float jumpHeight;
    private bool onGround = false;
    private Rigidbody2D rb;
    public Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
    animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));

        

     if (Input.GetKeyDown(KeyCode.W) && onGround == true)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            onGround = false;
        }

        if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }

        if (Input.GetKey(KeyCode.A))
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        onGround = true;

    }
}

