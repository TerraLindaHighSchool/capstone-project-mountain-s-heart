using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthController : MonoBehaviour
{

    private int health = 8;
    [SerializeField] FadeCamera fader;

    private void OnTriggerEnter2D(Collider2D collision)

    {
        if (collision.gameObject.GetComponent<EnemyStats>() != null && health >= 0)
        {
            health -= collision.gameObject.GetComponent<EnemyStats>().getDamage();
        }

        if (health < -1)
        {
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
        {
            fader.setFadeOut(true);
            if (fader.isBlek())
                SceneManager.LoadScene(sceneName: "Main Menu");
        }
    }

}
