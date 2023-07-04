using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public float currentTime = 0;
    public float startingTime = 3;
    public float countdownSpeed = 1f; // Factor de velocidad
    public TextMeshProUGUI countdownText;

    void Start()
    {
        currentTime = startingTime;
    }

    void Update()
    {
        currentTime -= countdownSpeed * Time.deltaTime;
        countdownText.text = currentTime.ToString();

        if (currentTime <= 0)
        {
            gameObject.SetActive(false);
            enabled = false;
        }
    }
}
