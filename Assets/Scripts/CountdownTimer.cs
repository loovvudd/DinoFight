using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public float currentTime = 0f;
    public float startingTime = 2f; // Actualizar el valor inicial a 2
    public float countdownSpeed = 1f; // Factor de velocidad
    public TextMeshProUGUI countdownText;
    public GameObject fightText;

    private bool isCountdownFinished = false;

    void Start()
    {
        currentTime = startingTime;
    }

    void Update()
    {
        currentTime -= countdownSpeed * Time.deltaTime;

        // Asegurarse de que el tiempo no sea negativo
        if (currentTime < 1f) // Cambiar el condicional a 1f
        {
            currentTime = 1f; // Ajustar el tiempo restante a 1 en lugar de 0
        }

        // Redondear hacia abajo y convertir a entero
        int displayedTime = Mathf.FloorToInt(currentTime);

        countdownText.text = displayedTime.ToString();

        if (currentTime <= 1f && !isCountdownFinished) // Cambiar el condicional a 1f
        {
            isCountdownFinished = true;
            countdownText.gameObject.SetActive(false); // Ocultar el texto de la cuenta regresiva
            StartCoroutine(ActivateFightText()); // Iniciar la corrutina para activar el texto "FIGHT!"
        }
    }

    IEnumerator ActivateFightText()
    {
        fightText.SetActive(true); // Activar el objeto "fightText"
        yield return new WaitForSeconds(2f); // Esperar 2 segundos
        fightText.SetActive(false); // Desactivar el objeto "fightText"
        gameObject.SetActive(false);
        enabled = false;
    }
}
