using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;


    [Header("Stats")]
    public float speed = 10;
    public float jumpForce = 50;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;

    private Collision coll;
    public Animator anim;
    [HideInInspector]
    public Rigidbody2D rb;

    private Vector2 m_Velocity = Vector2.zero;
    private Vector2 frictionL;

    private void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
    }

    //updates for movment
    //need more methods
    void Update()
    {

        anim.SetFloat("Horizontal", Input.GetAxis("Horizontal"));

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        Walk(dir);

        if (Input.GetButtonDown("Jump"))
        {
            //anim.SetTrigger("jump");

            if (coll.onGround)
                Jump(Vector2.up, false);
            }

            
        }

    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;
        else
        {
            rb.velocity = Vector2.SmoothDamp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), ref m_Velocity , m_MovementSmoothing);

        }
    }
    private void Jump(Vector2 dir, bool wall) {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity += dir * jumpForce;
        }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("coll");
    }
}

// Move the character by finding the target velocity
//Vector3 targetVelocity = new Vector2(moveSpeed * 10f, rb.velocity.y);
// And then smoothing it out and applying it to the character
//m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);


