using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadRespectiveScene : MonoBehaviour
{
    [SerializeField] private Object scene;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("hit");
        //if (collision.gameObject.tag.Equals("Player"))
       // {
            Debug.Log("on");
            SceneManager.LoadScene(sceneName: scene.name);
       // }
    }
}
