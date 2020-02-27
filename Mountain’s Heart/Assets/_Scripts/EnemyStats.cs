using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private int health;

    public int getDamage()
    {
        return damage;
    }
}
