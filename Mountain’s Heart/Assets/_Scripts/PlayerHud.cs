using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;

public class PlayerHud : MonoBehaviour
{

    [SerializeField] private Image[] healthBar;
    [SerializeField] private Sprite[] healthState;
    private GameObject player;
    private int playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        playerHealth = player.GetComponent<PlayerHealthController>().getHealth();

        if (healthBar[playerHealth].sprite == healthState[2])
            healthBar[playerHealth].sprite = healthState[0];
        if (healthBar[playerHealth].sprite == healthState[4])
                healthBar[playerHealth].sprite = healthState[2];
    }
}
