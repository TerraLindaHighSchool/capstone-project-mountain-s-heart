using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{

    private int health = 8;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyStats>() != null && health >= 0)
        {
            health -= collision.gameObject.GetComponent<EnemyStats>().getDamage();
        }

        if (health < -1)
            health = -1;
    }

    public int getHealth()
    {
        return health;
    }

    private void Update()
    {
        if (health < 0)
            Time.timeScale = 0;
    }

}
