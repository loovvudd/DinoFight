using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
    [SerializeField] private GameObject botonActivar;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            botonActivar.SetActive(true);
        }
    }

    public void Levels()
    {
        SceneManager.LoadScene("Seleccionar Niveles");
    }

    public void Exit()
    {
        Debug.Log("Saliendo...");
        Application.Quit();
    }
}
