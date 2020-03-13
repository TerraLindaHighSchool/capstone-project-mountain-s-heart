using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthController : MonoBehaviour
{

    private int health = 8;
    public Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyStats>() != null)
        {
            health -= collision.gameObject.GetComponent<EnemyStats>().getDamage();
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * 15.0f);

            if (health < -1)
                health = -1;
        }
    }

    public int getHealth()
    {
        return health;
    }

    private void Update()
    {
        if (health < 0)
            SceneManager.LoadScene(sceneName: "Main Menu");
    }

}
