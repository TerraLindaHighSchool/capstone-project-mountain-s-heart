using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;

public class PlayerHud : MonoBehaviour
{

    [SerializeField] private Sprite[] healthState;
    private Sprite[] healthBar = new Sprite[9];
    private GameObject player;
    private int playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        for (int i = 0; i < 9; i++)
        {
            healthBar = healthState[3];
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerHealth = player.GetComponent<PlayerHealthController>().getHealth();

       // healthBar
    }
}
