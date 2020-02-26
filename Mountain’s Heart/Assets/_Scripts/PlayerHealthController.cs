using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{

    private int health = 8;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Enemy") && health >= 0)
        {
            health -= collision.gameObject.GetComponent<EnemyStats>().getDamage();
            this.gameObject.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(-10000, 5000), ForceMode2D.Impulse);
        }

        if (health < -1)
            health = -1;
    }

    public int getHealth()
    {
        return health;
    }

}
