using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerHud : MonoBehaviour
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
        playerHealth = player.GetComponent<PlayerHealthController>().getHealth()+1;
        currentHealth = playerHealth;
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth = player.GetComponent<PlayerHealthController>().getHealth()+1;
        if(currentHealth != playerHealth)
        {
            healthBar[currentHealth].sprite = healthState[0];
        }
        playerHealth = currentHealth;
    }
}