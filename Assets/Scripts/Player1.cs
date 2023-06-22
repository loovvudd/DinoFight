using UnityEngine;
using System.Collections;

public class Player1 : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeedMultiplier = 2f;
    public KeyCode runKey = KeyCode.LeftShift;
    public float maxRunTime = 5f;
    public float waitTimeAfterRun = 2f;
    public GameObject dialogBubble; // Referencia al objeto de la nube de di�logo
    public float dialogDuration = 3f; // Duraci�n en segundos de la nube de di�logo
    private float currentRunTime = 0f;
    private bool isWaiting = false;
    public float dialogOffsetY = 1f; // Desplazamiento vertical de la nube de di�logo con respecto al jugador
    public float dialogOffsetX = 0f; // Desplazamiento horizontal de la nube de di�logo con respecto al jugador
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool isDead = false;

    public float jumpForce = 7f;

    public int maxLives = 3;
    public int currentLives;

    private bool isInvincible = false;
    public float invincibilityTime = 2f;
    private Animator animator; // Referencia al componente Animator
    public KeyCode pushButton = KeyCode.L;
    public float pushForce = 10f;
    public int pushDamage = 1;

    public Player2 player2; // Referencia al script del jugador 2

    private bool isTouchingPlayer2 = false; // Variable para rastrear si el jugador 1 est� tocando al jugador 2

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentLives = maxLives;
        animator = GetComponent<Animator>(); // Asignar componente Animator
        StartCoroutine(ShowDialogAndHideAfterDelay());
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        Vector2 direction = new Vector2(horizontalInput, 0f).normalized;

        float moveSpeedMultiplier = 1f;
        float moveSpeed = this.moveSpeed;

        if (Input.GetKey(runKey) && !isWaiting && currentRunTime > 0f)
        {
            moveSpeedMultiplier = runSpeedMultiplier;
            currentRunTime -= Time.deltaTime;

            if (currentRunTime <= 0f)
            {
                currentRunTime = 0f;
                isWaiting = true;
                moveSpeedMultiplier = 1f;
            }

            // Ejecutar animaci�n de correr
            animator.SetBool("IsRunning", true);
        }
        else
        {
            if (currentRunTime < maxRunTime)
            {
                currentRunTime += Time.deltaTime;
                isWaiting = false;
            }

            // Detener animaci�n de correr
            animator.SetBool("IsRunning", false);
        }

        moveSpeed = moveSpeed * moveSpeedMultiplier;

        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        // Establecer el par�metro "Move" del Animator
        animator.SetFloat("Move", Mathf.Abs(horizontalInput));
        // Girar el personaje en la direcci�n del movimiento
        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f); // Mirar hacia la derecha
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f); // Mirar hacia la izquierda
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isGrounded && Input.GetKey(runKey)) // Permitir saltar si est� en el suelo o si se mantiene presionada la tecla de correr
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            }
            else
            {
                // Intentar saltar sobre el jugador 2
                TryJumpOnPlayer2();
            }
            if (isGrounded) // Permitir saltar si est� en el suelo o si se mantiene presionada la tecla de correr
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            }
            animator.SetTrigger("Jump");
        }

        if (Input.GetKeyDown(pushButton) && isTouchingPlayer2) // Solo permite el ataque si est� tocando al jugador 2
        {
            PushPlayer2();
            animator.SetTrigger("Attack");

        }
        if (dialogBubble != null)
        {
            Vector3 dialogOffset = new Vector3(dialogOffsetX, dialogOffsetY, 0f);
            dialogBubble.transform.position = transform.position + dialogOffset;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = true;
            
        }

        if (collision.gameObject.CompareTag("Enemy") && !isInvincible)
        {
            TakeDamage();
        }

        if (collision.gameObject.CompareTag("Player")) // Comprueba si est� tocando al jugador 2
        {
            isTouchingPlayer2 = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = false;
            
        }

        if (collision.gameObject.CompareTag("Player")) // Comprueba si deja de tocar al jugador 2
        {
            isTouchingPlayer2 = false;
        }
    }

    public void TakeDamage()
    {
        if (isDead)
            return;
        currentLives--;

        if (currentLives <= 0)
        {
            Debug.Log("Game Over");
            animator.SetTrigger("Die");
            isDead = true;
            StartCoroutine(DisablePlayerCoroutine());
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
            Debug.Log("Player 1 took damage. Remaining lives: " + currentLives);
            // Activar la animaci�n de da�o al instante
            animator.Play("Damage1", -1, 0f); // Reemplaza "DamageAnimation" con el nombre de la animaci�n de da�o en el Animator
            animator.Update(0f); // Actualiza el estado de la animaci�n al inicio para que se muestre de inmediato
        }
        IEnumerator DisablePlayerCoroutine()
        {
            yield return new WaitForSeconds(0.889f); // Tiempo de espera para que termine la animaci�n de muerte

            gameObject.SetActive(false); // Desactivar el objeto del jugador
        }

    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        // Realizar acciones visuales para indicar la invulnerabilidad del jugador (por ejemplo, cambiar el color del sprite)

        yield return new WaitForSeconds(invincibilityTime);

        isInvincible = false;
    }
    private IEnumerator ShowDialogAndHideAfterDelay()
    {
        dialogBubble.SetActive(true); // Activa la nube de di�logo

        yield return new WaitForSeconds(dialogDuration);

        dialogBubble.SetActive(false); // Desactiva la nube de di�logo
    }
    private void TryJumpOnPlayer2()
    {
        RaycastHit2D hitGround = Physics2D.Raycast(transform.position, Vector2.down, 1f);
        RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Player"));

        if (hitGround.collider != null && hitGround.collider.CompareTag("Ground") && hitPlayer.collider != null && hitPlayer.collider.CompareTag("Player"))
        {
            // Saltar sobre el jugador 2
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }


    void PushPlayer2()
    {
        if (player2 != null)
        {
            Vector2 pushDirection = (player2.transform.position - transform.position).normalized;
            Rigidbody2D player2Rb = player2.GetComponent<Rigidbody2D>();

            if (player2Rb != null)
            {
                player2Rb.velocity = Vector2.zero;
                player2Rb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
            }

            player2.TakeDamage();
           
        }
        
    }
}
