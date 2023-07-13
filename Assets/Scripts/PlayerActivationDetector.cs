using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerActivationDetector : MonoBehaviour
{
    public string nextSceneName;
    public GameObject player1;
    public GameObject player2;

    private bool isPlayer1Dead = false;
    private bool isPlayer2Dead = false;

    public float timeToWait = 1;
    private void Update()
    {
        if (!isPlayer1Dead && player1.gameObject.activeInHierarchy == false)
        {
            isPlayer1Dead = true;
            CheckPlayersDeath();
        }

        if (!isPlayer2Dead && player2.gameObject.activeInHierarchy == false)
        {
            isPlayer2Dead = true;
            CheckPlayersDeath();
        }
    }

    private void CheckPlayersDeath()
    {
        if (isPlayer1Dead || isPlayer2Dead)
        {
            StartCoroutine(WaitAndLoad());
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator WaitAndLoad()
    {
        yield return new WaitForSeconds(timeToWait);
        LoadNextScene();
    }
}
