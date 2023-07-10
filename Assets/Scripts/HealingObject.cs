using UnityEngine;
using System.Collections;

public class HealingObject : MonoBehaviour
{
    public int healingAmount = 1;
    public Color healColor = Color.green;
    public float colorChangeDuration = 1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player1 player1 = collision.gameObject.GetComponent<Player1>();
            Player2 player2 = collision.gameObject.GetComponent<Player2>();

            if (player1 != null)
            {
                player1.Heal(healingAmount);
                StartCoroutine(ChangePlayerColor(player1));
            }
            else if (player2 != null)
            {
                player2.Heal(healingAmount);
                StartCoroutine(ChangePlayerColor(player2));
            }

            Destroy(gameObject);
        }
    }

    private IEnumerator ChangePlayerColor(Player1 player)
    {
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = healColor;
            yield return new WaitForSeconds(colorChangeDuration);
            spriteRenderer.color = originalColor;
        }
    }

    private IEnumerator ChangePlayerColor(Player2 player)
    {
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = healColor;
            yield return new WaitForSeconds(colorChangeDuration);
            spriteRenderer.color = originalColor;
        }
    }
}

