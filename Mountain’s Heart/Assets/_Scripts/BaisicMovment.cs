using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaisicMovment : MonoBehaviour
{
    [Header("Ground Checking")]
    [SerializeField] Transform groundTransform; //This is supposed to be a transform childed to the player just under their collider.
    [SerializeField] float groundCheckY = 0.2f; //How far on the Y axis the groundcheck Raycast goes.
    [SerializeField] float groundCheckX = 1;//Same as above but for X.
    [SerializeField] LayerMask groundLayer;
    [Space(5)]


    public float moveSpeed;
    public float jumpHeight;
    private Rigidbody2D rb;
    public Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
    animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));


     if (Input.GetKeyDown(KeyCode.W) && Grounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
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

    public bool Grounded()
    {
        //this does three small raycasts at the specified positions to see if the player is grounded.
        if (Physics2D.Raycast(groundTransform.position, Vector2.down, groundCheckY, groundLayer) || Physics2D.Raycast(groundTransform.position + new Vector3(-groundCheckX, 0), Vector2.down, groundCheckY, groundLayer) || Physics2D.Raycast(groundTransform.position + new Vector3(groundCheckX, 0), Vector2.down, groundCheckY, groundLayer))
        {
            Debug.Log("Working");
            return true;          
        }
        else
        {
            return false;
        }

    }
}

