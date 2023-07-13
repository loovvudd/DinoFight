using UnityEngine;

public class PowerDown : MonoBehaviour
{
    public float duration = 10f; // Duración del efecto de Power Down

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player1 player1 = collision.GetComponent<Player1>();
        Player2 player2 = collision.GetComponent<Player2>();

        if (player1 != null)
        {
            player1.ApplyPowerDown(duration);
        }

        if (player2 != null)
        {
            player2.ApplyPowerDown(duration);
        }

        Destroy(gameObject); // Destruir el objeto Power Down
    }
}
