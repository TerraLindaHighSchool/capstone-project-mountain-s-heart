using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadRespectiveScene : MonoBehaviour
{
    [SerializeField] private Object scene;
    [SerializeField] FadeCamera fader;
    private bool load;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            fader.setFadeOut(true);
            load = true;
        }
    }

    private void Update()
    {
        if (fader.isBlek() && load == true)
        {
            SceneManager.LoadScene(sceneName: scene.name);
            load = false;
        }
    }
}
