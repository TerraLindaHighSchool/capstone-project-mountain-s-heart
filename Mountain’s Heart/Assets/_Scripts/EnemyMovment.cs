using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovment : MonoBehaviour
{
    [Header("Wall Checking")]
    [SerializeField] Transform groundTransform; //This is supposed to be a transform childed to the player just around their collider.
    [SerializeField] float wallCheckX = 1; // You probably want this to be the same as groundCheckX
    [SerializeField] LayerMask groundLayer;
    [Space(5)]

    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;

    public float moveSpeed;
    private Rigidbody2D rb;
    private Vector2 m_Velocity = Vector2.zero;
    private Vector2 frictionL;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 targetVelocity = new Vector2(moveSpeed, rb.velocity.y);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        if (Walled())
        {
            moveSpeed *= -1;
            transform.Rotate(0, 180, 0);
            transform.position = new Vector3(transform.position.x + moveSpeed/10, transform.position.y, transform.position.z);
        }
    }

    public bool Walled()
    {
        if (Physics2D.Raycast(groundTransform.position, Vector2.left, wallCheckX, groundLayer) || Physics2D.Raycast(groundTransform.position, Vector2.right, wallCheckX, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
  
