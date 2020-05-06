using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealthController : MonoBehaviour
{

    private int health = 8;
    private Rigidbody2D rb;
    private Vector2 recoil;
    private bool invincible = false;
    [SerializeField] private float invincibilityCountdown = 1;
    [SerializeField] float recoilModifier = 30;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Enemy") && !invincible)
        {
            health -= collision.gameObject.GetComponent<EnemyDamageController>().getDamage();
            recoil = new Vector2(recoilModifier * (this.transform.position.x - collision.gameObject.transform.position.x),
                                    recoilModifier * (this.transform.position.y - collision.gameObject.transform.position.y));
            invincible = true;
            StartCoroutine(invincibility(invincibilityCountdown));
            GetComponent<Rigidbody2D>().AddForce(recoil);

            if (health < -1)
                health = -1;
        }
    }

    public int getHealth()
    {
        return health;
    }

    IEnumerator invincibility(float time)
    {
        float startTime = Time.time;
        Physics2D.IgnoreLayerCollision(11, 10, true);
        while (invincible)
        {
            if (Time.time - startTime >= invincibilityCountdown)
            {
                Physics2D.IgnoreLayerCollision(11, 10, false);
                invincible = false;
            }

            yield return null;
        }
    }

    private void Update()
    {
        if (health < 0)
            SceneManager.LoadScene(sceneName: "Main Menu");
    }

}
