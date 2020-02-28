using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{

    bool vis = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("h");
            if (!vis)
            {
                this.GetComponentInChildren<Text>().enabled = true;
                vis = true;
            }
            else
            {
                this.GetComponentInChildren<Text>().enabled = false;
                vis = false;
            }
        }
    }
}
