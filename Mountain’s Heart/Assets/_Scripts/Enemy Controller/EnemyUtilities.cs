using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyDamageController))]
public class EnemyUtilities : MonoBehaviour
{
    public GameObject player;

    [Header("Wall Checking")]
    public float wallCheck;
    public Vector2 groundCheck = Vector2.zero;
    public LayerMask groundLayer;
    [Space(5)]

    [Range(0, .3f)] public float m_HorzMovementSmoothing = .05f;
    [Range(0, .3f)] public float m_VertMovementSmoothing = .05f;

    public float horzSpeed = 1f;
    public float vertSpeed = 0f;
    public float flightHeight = 0f;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Vector2 m_Velocity = Vector2.zero;
    [HideInInspector] public Vector2 frictionL;
    [HideInInspector] public float distToPlayer, angleToPlayer, curAltitude;
    [HideInInspector] public bool inCombat;
    [HideInInspector] public bool movingRight;
    [HideInInspector] public Animator anim;
    public float agressionRange;
    public float fieldOfView;
    public float attackRange;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        anim = this.GetComponent<Animator>();
        movingRight = false;
        horzSpeed *= -1;
    }

    private void Update()
    {
        distToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (movingRight)
            angleToPlayer = Vector2.Angle(player.transform.position - transform.position, Vector2.right);
        else
            angleToPlayer = Vector2.Angle(player.transform.position - transform.position, Vector2.left);
    }

    public IEnumerator CombatCoolDown(float cd)
    {
        float startTime = Time.time;

        while (inCombat)
        {
            if (Time.time - startTime >= cd)
            {
                inCombat = false;
            }

            yield return null;
        }
    }

    #region Movement
    public void flyToAltitude(float targetHeight, float speed)
    {
        Vector2 targetVelocity = Vector2.zero;
        if (transform.position.y > targetHeight)
        {
            targetVelocity = new Vector2(rb.velocity.x, -speed);
        }
        if (transform.position.y < targetHeight)
        {
            targetVelocity = new Vector2(rb.velocity.x, speed);
        }
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_VertMovementSmoothing);
    }

    #endregion
    #region Detection
    public bool Walled()
    {
        if (movingRight)
        {
            if (Physics2D.Raycast(transform.position, Vector2.right, wallCheck))
                return true;
            else
                return false;
        }
        else
        {
            if (Physics2D.Raycast(transform.position, Vector2.left, wallCheck))
                return true;
            else
                return false;
        }
    }

    [HideInInspector] public bool grounded = true;
    public bool approachingDrop()
    {
        bool right = !Physics2D.Linecast(transform.position + Vector3.right * groundCheck.x, (transform.position + Vector3.right * groundCheck.x) + Vector3.down * groundCheck.y);
        bool left = !Physics2D.Linecast(transform.position + Vector3.left * groundCheck.x, (transform.position + Vector3.left * groundCheck.x) + Vector3.down * groundCheck.y);

        if (left || right)
            grounded = false;
        if (!left || !right)
            grounded = true;

        if (movingRight)
        {
            if (right && grounded)
            {
                return true;
            }
            else if (!right && !grounded)
            {
                return false;
            }
            else if (right && !grounded)
            {
                return false;
            }
            else
                return false;
        }
        else
        {
            if (left && grounded)
            {
                return true;
            }
            else if (!left && !grounded)
            {
                return false;
            }
            else if (left && !grounded)
            {
                return false;
            }
            else
                return false;
        }
    }

    public bool playerFound()
    {
        if (distToPlayer <= agressionRange && angleToPlayer <= fieldOfView)
        {
            if(!playerObscured())
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }
    }

    public bool playerObscured()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position);
        if (!hit.collider.gameObject.tag.Equals("Player"))
            return true;
        else
            return false;
    }
    #endregion


    private void OnDrawGizmos()
    {
        #region Player Check
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, agressionRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        if (movingRight)
        {
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(fieldOfView * Mathf.Deg2Rad), Mathf.Sin(fieldOfView * Mathf.Deg2Rad), 0) * agressionRange);
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(-fieldOfView * Mathf.Deg2Rad), Mathf.Sin(-fieldOfView * Mathf.Deg2Rad), 0) * agressionRange);
            Gizmos.DrawLine(transform.position + new Vector3(Mathf.Cos(fieldOfView * Mathf.Deg2Rad), Mathf.Sin(fieldOfView * Mathf.Deg2Rad), 0) * agressionRange, transform.position + Vector3.right * agressionRange);
            Gizmos.DrawLine(transform.position + new Vector3(Mathf.Cos(fieldOfView * Mathf.Deg2Rad), Mathf.Sin(-fieldOfView * Mathf.Deg2Rad), 0) * agressionRange, transform.position + Vector3.right * agressionRange);

        }
        else
        {
            Gizmos.DrawLine(transform.position, transform.position - new Vector3(Mathf.Cos(fieldOfView * Mathf.Deg2Rad), Mathf.Sin(fieldOfView * Mathf.Deg2Rad), 0) * agressionRange);
            Gizmos.DrawLine(transform.position, transform.position - new Vector3(Mathf.Cos(-fieldOfView * Mathf.Deg2Rad), Mathf.Sin(-fieldOfView * Mathf.Deg2Rad), 0) * agressionRange);
            Gizmos.DrawLine(transform.position - new Vector3(Mathf.Cos(fieldOfView * Mathf.Deg2Rad), Mathf.Sin(fieldOfView * Mathf.Deg2Rad), 0) * agressionRange, transform.position + Vector3.left * agressionRange);
            Gizmos.DrawLine(transform.position - new Vector3(Mathf.Cos(fieldOfView * Mathf.Deg2Rad), Mathf.Sin(-fieldOfView * Mathf.Deg2Rad), 0) * agressionRange, transform.position + Vector3.left * agressionRange);

        }
        #endregion

        #region Blocked Check
        Gizmos.color = Color.green;
        if (movingRight)
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheck);
        else
            Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheck);


        if (movingRight)
            Gizmos.DrawLine(transform.position + Vector3.right * groundCheck.x, (transform.position + Vector3.right * groundCheck.x) + Vector3.down * groundCheck.y);
        else
            Gizmos.DrawLine(transform.position + Vector3.left * groundCheck.x, (transform.position + Vector3.left * groundCheck.x) + Vector3.down * groundCheck.y);

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, Vector3.down * flightHeight);

        #endregion
    }
}
