using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyUtilities))]
public class EnemyCombat : MonoBehaviour
{
    EnemyUtilities util;
    [SerializeField] private Vector2 attackMoveSpeedMod = Vector2.one;
    [SerializeField] private bool checkForDrops, enableFlight, ignoreTerrain;
    public float coolDownTime;

    float ogHorzSpeed;
    Vector2 playerPos = Vector2.zero;

    private void Start()
    {
        util = GetComponent<EnemyUtilities>();
        ogHorzSpeed = util.horzSpeed;
    }

    private void Update()
    {
        if (ignoreTerrain)
            Physics2D.IgnoreLayerCollision(10, 8, true);
        else
            Physics2D.IgnoreLayerCollision(10, 8, false);
    }

    public void Chase()
    {
        if (util.distToPlayer > util.agressionRange)
            StartCoroutine(util.CombatCoolDown(coolDownTime));

        if(!util.playerObscured() || ignoreTerrain)
            playerPos = util.player.transform.position;

        Vector2 targetVelocity = new Vector2(util.horzSpeed * attackMoveSpeedMod.x, util.rb.velocity.y);
        util.rb.velocity = Vector2.SmoothDamp(util.rb.velocity, targetVelocity, ref util.m_Velocity, util.m_VertMovementSmoothing);

        if ((playerPos.x - transform.position.x < 0) && util.movingRight)
        {
            util.horzSpeed *= -1;
            util.movingRight = false;
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (playerPos.x - transform.position.x > 0 && !util.movingRight)
        {
            util.horzSpeed *= -1;
            util.movingRight = true;
            transform.eulerAngles = new Vector3(0, -180, 0);
        }

        if (Vector2.Distance(transform.position, playerPos) < 0.5 && util.playerObscured() && !ignoreTerrain)
            util.rb.velocity = Vector2.zero;

        if (checkForDrops)
        {
            if (util.approachingDrop())
                util.rb.velocity = new Vector2(0, util.rb.velocity.y);
        }

        if (enableFlight)
        {
            float targetHeight = playerPos.y;

            util.flyToAltitude(targetHeight, util.vertSpeed + attackMoveSpeedMod.y);
        }
    }

    public void Attack()
    {
        //Place holder for attack behavior
        util.rb.velocity = Vector3.zero;
    }
}
