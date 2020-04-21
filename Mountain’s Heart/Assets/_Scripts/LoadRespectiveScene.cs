using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadRespectiveScene : MonoBehaviour
{
    [SerializeField] private Object scene;
    public Animator transition;
    public float transitionTime = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            StartCoroutine(LoadLevel());
        }
    }

    public void LoadNextLevel()
    {
        //Debug.Log(SceneManager.GetActiveScene().buildIndex);
       // Debug.Log(SceneManager.GetSceneByName(scene.name).buildIndex);
        //StartCoroutine(LoadLevel(SceneManager.g(scene.name).buildIndex));
    }

    IEnumerator LoadLevel()
    {
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName: scene.name);
    }

}