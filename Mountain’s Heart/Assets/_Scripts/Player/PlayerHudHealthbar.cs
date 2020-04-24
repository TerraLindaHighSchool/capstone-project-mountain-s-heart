using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerHudHealthbar : MonoBehaviour
{

    [SerializeField] private Sprite[] healthState;
    [SerializeField] private Image[] healthBar;
    private GameObject player;
    private int playerHealth;
    private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealthController>().getHealth();
        currentHealth = playerHealth;
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth = player.GetComponent<PlayerHealthController>().getHealth();
        if(currentHealth < playerHealth)
        {
            for(int i = playerHealth; i > currentHealth; i--)
                healthBar[playerHealth].sprite = healthState[0];
        }
        playerHealth = currentHealth;
    }
}