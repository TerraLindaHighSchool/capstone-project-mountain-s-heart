using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageController : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private int health = 1;

    private void Update()
    {
        if (health <= 0)
            Destroy(this.gameObject);
    }

    public int getDamage()
    {
        return damage;
    }

    public void takeDamage(int dmg)
    {
        health -= dmg;
    }
}
