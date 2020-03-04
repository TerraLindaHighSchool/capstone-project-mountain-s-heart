using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadRespectiveScene : MonoBehaviour
{
    [SerializeField] private Object scene;
    [SerializeField] FadeCamera fader;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            fader.setTurnBlek(true);
        }
    }

    private void Update()
    {
        if (fader.isBlek())
            SceneManager.LoadScene(sceneName: scene.name);
    }
}
