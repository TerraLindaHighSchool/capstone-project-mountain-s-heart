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
        Debug.Log("col check");
        //if (collision.gameObject.tag.Equals("Player"))
        //{
            Debug.Log("col work");
            fader.setTurnBlek(true);
       // }
    }

    private void Update()
    {
        if (fader.isBlek())
        {
            Debug.Log("black");
            SceneManager.LoadScene(sceneName: scene.name);
        }
    }
}
