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

    [Header("Roof Checking")]
    [SerializeField] Transform roofTransform; //This is supposed to be a transform childed to the player just above their collider.
    [SerializeField] float roofCheckY = 0.2f;
    [SerializeField] float roofCheckX = 1; // You probably want this to be the same as groundCheckX
    [Space(5)]

    [Header("Wall Checking")]
    [SerializeField] Transform wallTransform; //This is supposed to be a transform childed to the player just around their collider.
    [SerializeField] float wallCheckY = 0.2f;
    [SerializeField] float wallCheckX = 1; // You probably want this to be the same as groundCheckX
    [Space(5)]


    public float moveSpeed;
    public float jumpHeight;
    private Rigidbody2D rb;
    public Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //updates for movment
    //need more methods
    void Update()
    {
        animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));


        if (Input.GetKeyDown(KeyCode.W) && Grounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
        }

        if (Input.GetKey(KeyCode.D) && !WalledRight())
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }

        if (Input.GetKey(KeyCode.A) && !WalledLeft())
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
        }
        //Roofed();
    }

    public bool Grounded()
    {

        //this does three small raycasts at the specified positions to see if the player is grounded.
        if (Physics2D.Raycast(groundTransform.position, Vector2.down, groundCheckY, groundLayer) ||
            Physics2D.Raycast(groundTransform.position + new Vector3(groundCheckX, 0), Vector2.down + new Vector2(this.GetComponent<BoxCollider2D>().size.x / 2, 0), groundCheckY, groundLayer) ||
            Physics2D.Raycast(groundTransform.position + new Vector3(-groundCheckX, 0), Vector2.down - new Vector2(this.GetComponent<BoxCollider2D>().size.x / 2, 0), groundCheckY, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Roofed()
    {
        //This does the same thing as grounded but checks if the players head is hitting the roof instead.
        //Used for canceling the jump.
        if (Physics2D.Raycast(roofTransform.position, Vector2.up, roofCheckY, groundLayer) ||
            Physics2D.Raycast(roofTransform.position + new Vector3(roofCheckX, 0), Vector2.up + new Vector2(this.GetComponent<BoxCollider2D>().size.x / 2, 0), roofCheckY, groundLayer) ||
            Physics2D.Raycast(roofTransform.position + new Vector3(-roofCheckX, 0), Vector2.up - new Vector2(this.GetComponent<BoxCollider2D>().size.x / 2, 0), roofCheckY, groundLayer))
        {
            Debug.Log("Roofed!");
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool WalledLeft()
    {
        //This does the same thing as grounded but checks if the players head is hitting the roof instead.
        //Used for canceling the jump.
        if (Physics2D.Raycast(wallTransform.position, Vector2.left, wallCheckX, groundLayer) ||
            Physics2D.Raycast(wallTransform.position + new Vector3(wallCheckX, 0), Vector2.left + new Vector2(0, this.GetComponent<BoxCollider2D>().size.y / 2), wallCheckY, groundLayer) ||
            Physics2D.Raycast(wallTransform.position + new Vector3(-wallCheckX, 0), Vector2.left - new Vector2(0, this.GetComponent<BoxCollider2D>().size.y / 2), wallCheckY, groundLayer))
        {
            Debug.Log("WalledL!");
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool WalledRight()
    {
        //This does the same thing as grounded but checks if the players head is hitting the roof instead.
        //Used for canceling the jump.
        if (Physics2D.Raycast(wallTransform.position, Vector2.right, wallCheckX, groundLayer) ||
            Physics2D.Raycast(wallTransform.position + new Vector3(wallCheckX, 0), Vector2.right + new Vector2(0, this.GetComponent<BoxCollider2D>().size.y / 2), wallCheckY, groundLayer) ||
            Physics2D.Raycast(wallTransform.position + new Vector3(-wallCheckX, 0), Vector2.right - new Vector2(0, this.GetComponent<BoxCollider2D>().size.y / 2), wallCheckY, groundLayer))
        {
            Debug.Log("WalledR!");
            return true;
        }
        else
        {
            return false;
        }
    }


}

