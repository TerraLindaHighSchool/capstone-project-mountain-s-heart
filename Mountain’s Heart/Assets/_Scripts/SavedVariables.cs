using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedVariables : MonoBehaviour
{
    public static Vector2 spawnPos;

    private SavedVariables()
    {

    }

    public static Vector2 getSpawnPos()
    {
        return spawnPos;
    }

    public static void setSpawnPos(Vector2 v)
    {
        spawnPos = v;
    }


}
