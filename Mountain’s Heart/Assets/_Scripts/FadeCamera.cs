using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeCamera : MonoBehaviour
{
    [SerializeField] private Image blackScreen;
    private float currentAlpha;
    private bool blek = false;
    private bool turnBlek = false;
    private bool turnKlere = false;

    // Start is called before the first frame update
    void Start()
    {
        turnKlere = true;
        blackScreen.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (turnBlek)
        {
            currentAlpha = blackScreen.color.a;
            blackScreen.color = new Color(0, 0, 0, currentAlpha + 0.02f);
            if (blackScreen.color.a >= 1)
            {
                blek = true;
                turnBlek = false;
            }
        }

        if (turnKlere)
        {
            currentAlpha = blackScreen.color.a;
            blackScreen.color = new Color(0, 0, 0, currentAlpha - 0.02f);
            if (blackScreen.color.a <= 0)
            {
                blek = false;
                turnKlere = false;
            }
        }
    }


    public bool isBlek()
    {
        
        return blek;
    }

    public void setTurnBlek(bool lol)
    {
        
        turnBlek = lol;
    }

    public void setTurnKlere(bool lol)
    {
        turnKlere = lol;

    }
}
