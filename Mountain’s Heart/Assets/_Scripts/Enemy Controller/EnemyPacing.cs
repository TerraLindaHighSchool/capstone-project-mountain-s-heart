using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyUtilities))]
public class EnemyPacing : MonoBehaviour
{
    EnemyUtilities util;
    [SerializeField] private bool checkForDrops;
    [SerializeField] private bool enableFlight;

    private void Start()
    {
        util = GetComponent<EnemyUtilities>();
    }

    public void Pace()
    {
        Vector2 targetVelocity = new Vector2(util.horzSpeed, util.rb.velocity.y);
        util.rb.velocity = Vector2.SmoothDamp(util.rb.velocity, targetVelocity, ref util.m_Velocity, util.m_VertMovementSmoothing);

        if (util.Walled() || (checkForDrops && util.approachingDrop()))
        {
            if (util.movingRight)
            {
                util.horzSpeed *= -1;
                util.movingRight = false;
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                util.horzSpeed *= -1;
                util.movingRight = true;
                transform.eulerAngles = new Vector3(0, -180, 0);
            }
        }

        if (enableFlight)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 100, util.groundLayer);

            float targetHeight = hit.point.y + util.flightHeight;

            util.flyToAltitude(targetHeight, util.vertSpeed);
        }
    }
}
  
