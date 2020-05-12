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
    public Vector2 spawnPos;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(spawnPos.x != 0 || spawnPos.y != 0)
        {
            SavedVariables.setSpawnPos(spawnPos);
        }

        if (collision.gameObject.tag.Equals("Player"))
        {
            StartCoroutine(LoadLevel());
        }
    }

    IEnumerator LoadLevel()
    {
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName: scene.name);
    }

}