using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float powerUpDuration = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player1 player = other.GetComponent<Player1>();
            if (player != null)
            {
                player.ActivatePowerUp();
                Destroy(gameObject);
            }
        }
    }
}
