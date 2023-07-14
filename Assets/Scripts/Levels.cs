using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Levels : MonoBehaviour
{
    public void lvl1()
    {
        SceneManager.LoadScene("Escena1");
    }

    public void lvl2()
    {
        SceneManager.LoadScene("Escena2");
    }

    public void lvl3()
    {
       SceneManager.LoadScene("Escena3");
    }
    public void Creditos()
    {
        SceneManager.LoadScene("Creditos");
    }
    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
    public void BacktoMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
