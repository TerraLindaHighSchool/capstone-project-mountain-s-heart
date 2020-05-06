using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyPacing))]
[RequireComponent(typeof(EnemyCombat))]
public class AIController : MonoBehaviour
{
    EnemyUtilities util;
    EnemyPacing pacing;
    EnemyCombat combat;

    // Start is called before the first frame update
    void Start()
    {
        util = this.GetComponent<EnemyUtilities>();
        pacing = this.GetComponent<EnemyPacing>();
        combat = this.GetComponent<EnemyCombat>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (util.playerFound() && util.distToPlayer < util.attackRange)
        {
            combat.Attack();
        }
        else if (util.playerFound() || util.inCombat)
        {
            util.inCombat = true;
            combat.Chase();
        }
        else
        {
            pacing.Pace();
        }
    }
}
