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
    private bool fadeOut = false;
    private bool fadeIn = false;

    // Start is called before the first frame update
    void Start()
    {
        fadeIn = true;
        blackScreen.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeOut)
        {
            currentAlpha = blackScreen.color.a;
            blackScreen.color = new Color(0, 0, 0, currentAlpha + 0.02f);
            if (blackScreen.color.a >= 1)
            {
                blek = true;
                fadeOut = false;
            }
        }

        if (fadeIn)
        {
            currentAlpha = blackScreen.color.a;
            blackScreen.color = new Color(0, 0, 0, currentAlpha - 0.02f);
            if (blackScreen.color.a <= 0)
            {
                blek = false;
                fadeIn = false;
            }
        }
    }


    public bool isBlek()
    {
        
        return blek;
    }

    public void setFadeOut(bool lol)
    {

        fadeOut = lol;
    }
}
