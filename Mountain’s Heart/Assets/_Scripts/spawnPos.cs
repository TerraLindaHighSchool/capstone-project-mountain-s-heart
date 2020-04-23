using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnPos : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        if(SavedVariables.getSpawnPos() != null)
        {
            transform.position = SavedVariables.getSpawnPos();
            SavedVariables.setSpawnPos(Vector2.zero);
        }
    }

}
