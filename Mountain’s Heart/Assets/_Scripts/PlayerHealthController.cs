using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    private List<GameObject> playerHealth;
    public int healthValue;

    void Update()
    {
        if(healthValue % 2 == 0)
        {
            for(int i = 0; i < healthValue; i++)
            {
               // playerHealth[i] 
            }
        }
    }

}
