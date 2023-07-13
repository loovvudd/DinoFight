using UnityEngine;
using System.Collections;

public class Player2 : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeedMultiplier = 2f;
    public KeyCode runKey = KeyCode.LeftShift;
    public float maxRunTime = 5f;
    public float waitTimeAfterRun = 2f;
    public GameObject dialogBubble2; // Referencia al objeto de la nube de diálogo
    public float dialogDuration = 3f; // Duración en segundos de la nube de diálogo
    public float dialogOffsetY = 1f; // Desplazamiento vertical de la nube de diálogo con respecto al jugador
    public float dialogOffsetX = 0f; // Desplazamiento horizontal de la nube de diálogo con respecto al jugador
    private float currentRunTime = 0f;
    private bool isWaiting = false;
    public GameObject objetoSeguidor;
    public float fuerzaLanzamiento = 5f; // Fuerza con la que se lanza el objeto hacia arriba
    public Player1 player1; // Referencia al script del Jugador 1
    private bool canTouchObject = true; // Variable para rastrear si el Jugador 2 puede tocar el objeto seguidor
    public AudioSource audioSource; // Referencia al componente AudioSource del jugador
    public AudioClip damage; // Sonido a reproducir cuando el jugador recibe daño
    private Rigidbody2D rb;
    private bool isGrounded = false;
    public float jumpForce = 7f;
    public Color healColor = Color.green;
    public float colorChangeDuration = 1f;
    public int maxLives = 3;
    public int currentLives;
    private SpriteRenderer spriteRenderer;
    private bool isInvincible = false;
    public float invincibilityTime = 2f;
    public float powerDownDuration = 10f;
    public float powerDownScaleFactor = 0.5f;
    public float powerDownSpeedMultiplier = 0.5f;
    public float powerDownJumpMultiplier = 0.5f;
    public int powerDownDamageReduction = 1;
    private bool isPowerDownActive = false;
    private float originalMoveSpeed;
    private Animator animator; // Referencia al componente Animator
    public KeyCode pushButton = KeyCode.G;
    public float pushForce = 10f;
    public int pushDamage = 1;
    private float originalJumpForce;
    private bool isDead = false;
    private bool isTouchingPlayer1 = false; // Variable para rastrear si el jugador 1 está tocando al jugador 2
    public bool isPowerUpActive = false;
    public float powerUpDuration = 10f;
    private Vector3 originalScale;
    private float targetScaleFactor = 2f;
    private int initialPushDamage;
    private BarraDeVida2 barraDeVida2;
    private bool hasJumpedInAir = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentLives = maxLives;
        animator = GetComponent<Animator>(); // Asignar componente Animator
        StartCoroutine(ShowDialogAndHideAfterDelay());
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        initialPushDamage = pushDamage;
        isPowerUpActive = false;
        barraDeVida2 = FindObjectOfType<BarraDeVida2>();
        barraDeVida2.InicializarBarraDeVida(currentLives);
        originalMoveSpeed = moveSpeed;
        originalJumpForce = jumpForce;
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal2");
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

            animator.SetBool("IsRunning", true);
        }
        else
        {
            if (currentRunTime < maxRunTime)
            {
                currentRunTime += Time.deltaTime;
                isWaiting = false;
            }
            // Ejecutar animación de correr
            animator.SetBool("IsRunning", false);
        }

        moveSpeed = moveSpeed * moveSpeedMultiplier;

        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

        // Establecer el parámetro "Move" del Animator
        animator.SetFloat("Move", Mathf.Abs(horizontalInput));
        // Girar el personaje en la dirección del movimiento
        if (isPowerUpActive)
        {
            // Voltear el sprite en la dirección del movimiento
            if (horizontalInput > 0)
            {
                spriteRenderer.flipX = false; // No voltear horizontalmente
            }
            else if (horizontalInput < 0)
            {
                spriteRenderer.flipX = true; // Voltear horizontalmente
            }
        }
        else
        {
            // Girar el sprite en la dirección del movimiento
            if (horizontalInput > 0)
            {
                spriteRenderer.flipX = false; // No voltear horizontalmente
            }
            else if (horizontalInput < 0)
            {
                spriteRenderer.flipX = true; // Voltear horizontalmente
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (isGrounded || !hasJumpedInAir)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

                if (!isGrounded)
                {
                    hasJumpedInAir = true; // Registrar el salto en el aire
                }
            }
            if (isGrounded && Input.GetKey(runKey)) // Permitir saltar si está en el suelo o si se mantiene presionada la tecla de correr
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            }
            else
            {
                // Intentar saltar sobre el jugador 1
                TryJumpOnPlayer1();
            }
            animator.SetTrigger("Jump");
        }
        if (Input.GetKeyDown(pushButton) && isTouchingPlayer1) // Solo permite el ataque si está tocando al jugador 1
        {
            PushPlayer1();
        }
        if (dialogBubble2 != null)
        {
            Vector3 dialogOffset = new Vector3(dialogOffsetX, dialogOffsetY, 0f);
            dialogBubble2.transform.position = transform.position + dialogOffset;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = true;
            hasJumpedInAir = false;
        }

        if (collision.gameObject.CompareTag("Enemy") && !isInvincible)
        {
            TakeDamage(-1);
            barraDeVida2.CambiarVidaActual(currentLives);
        }
        if (collision.gameObject.CompareTag("Player")) // Comprueba si está tocando al jugador 1
        {
            isTouchingPlayer1 = true;
        }
        if (collision.collider.CompareTag("PowerUp"))
        {
            ActivatePowerUp();
            Destroy(collision.gameObject);
        }
        if (collision.collider.CompareTag("PowerDown"))
        {
            ApplyPowerDown(powerDownDuration);
            Destroy(collision.gameObject);
        }
        if (collision.collider.CompareTag("Item") && canTouchObject)
        {
            Transform objectTransform = collision.transform;
            objectTransform.SetParent(transform); // Establecer el objeto colisionado como hijo del jugador
            objectTransform.localPosition = new Vector3(0f, 1f, 0f); // Desplazar el objeto hacia arriba del jugador
            collision.gameObject.GetComponent<Rigidbody2D>().isKinematic = true; // Desactivar la física del objeto colisionado
        }
    }

    public void ApplyPowerDown(float powerDownDuration)
    {
        if (!isPowerDownActive)
        {
            isPowerDownActive = true;

            StartCoroutine(PowerDownCoroutine());
        }
    }

    public void ActivatePowerUp()
    {
        if (!isPowerUpActive)
        {
            isPowerUpActive = true;
            pushDamage *= 2;
            pushForce *= 3;

            StartCoroutine(ScalePlayerOverTime(targetScaleFactor, powerUpDuration));
            StartCoroutine(ResetPushDamageAfterDelay(powerUpDuration));
        }
    }
    private IEnumerator PowerDownCoroutine()
    {
        // Guardar el valor original de la velocidad
        float originalMoveSpeed = moveSpeed;

        // Guardar el valor original de la fuerza de salto
        float originalJumpForce = jumpForce;

        // Reducir el tamaño del jugador
        transform.localScale = originalScale * powerDownScaleFactor;

        // Reducir la velocidad del jugador
        moveSpeed *= powerDownSpeedMultiplier;

        // Reducir la fuerza de salto del jugador
        jumpForce *= powerDownJumpMultiplier;

        yield return new WaitForSeconds(powerDownDuration);

        // Restaurar el tamaño original del jugador
        transform.localScale = originalScale;

        // Restaurar la velocidad original del jugador
        moveSpeed = originalMoveSpeed;

        // Restaurar la fuerza de salto original del jugador
        jumpForce = originalJumpForce;

        // Restaurar el daño original del jugador
       

        isPowerDownActive = false;
    }

    private IEnumerator ResetPushDamageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        pushDamage = initialPushDamage; // Restablecer el valor inicial de pushDamage
    }
    private IEnumerator ScalePlayerOverTime(float targetScaleFactor, float duration)
    {
        float initialScaleFactor = transform.localScale.x;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / duration);
            float currentScaleFactor = Mathf.Lerp(initialScaleFactor, targetScaleFactor, t);

            transform.localScale = new Vector3(currentScaleFactor, currentScaleFactor, 1f);

            yield return null;
        }

        transform.localScale = originalScale; // Restaurar el tamaño original del personaje

        isPowerUpActive = false; // Restablecer el estado del power-up
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = false;
        }

        if (collision.gameObject.CompareTag("Player")) // Comprueba si está tocando al jugador 1
        {
            isTouchingPlayer1 = false;
        }
        if (collision.collider.CompareTag("Item"))
        {
            if (player1 != null && player1.objetoSeguidor != null && player1.objetoSeguidor.transform.parent == transform)
            {
                canTouchObject = false;
            }
            else
            {
                canTouchObject = true;
            }
        }
    }
    public void Heal(int amount)
    {
        currentLives += amount;
        Debug.Log("Player 2 healed. Current lives: " + currentLives);
        barraDeVida2.CambiarVidaActual(currentLives);
        StartCoroutine(ChangePlayerColor(healColor, colorChangeDuration));
    }

    private IEnumerator ChangePlayerColor(Color color, float duration)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(duration);
            spriteRenderer.color = originalColor;
        }
    }
    public void TakeDamage(int dmg)
    {
        if (isDead)
            return;
        currentLives-=dmg;
        barraDeVida2.CambiarVidaActual(currentLives);

        if (currentLives <= 0 && !isDead)
        {
            Debug.Log("Game Over");
            animator.SetTrigger("Die");
            isDead = true;
            StartCoroutine(DisablePlayerCoroutine());

           
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
            Debug.Log("Player 2 took damage. Remaining lives: " + currentLives);
            // Activar la animación de daño al instante
            animator.Play("Damage2", -1, 0f); // Reemplaza "DamageAnimation" con el nombre de la animación de daño en el Animator
            animator.Update(0f); // Actualiza el estado de la animación al inicio para que se muestre de inmediato
            audioSource.PlayOneShot(damage);
        }
        IEnumerator DisablePlayerCoroutine()
        {
            yield return new WaitForSeconds(0.90f); // Tiempo de espera para que termine la animación de muerte

            gameObject.SetActive(false); // Desactivar el objeto del jugador

        }
        if (objetoSeguidor != null && objetoSeguidor.transform.parent == transform)
        {
            Rigidbody2D objetoRigidbody = objetoSeguidor.GetComponent<Rigidbody2D>();
            objetoSeguidor.transform.SetParent(null); // Despegar el objeto del jugador
            objetoRigidbody.isKinematic = false; // Activar la física del objeto
            objetoRigidbody.velocity = new Vector2(0f, fuerzaLanzamiento); // Lanzar el objeto hacia arriba
        }
        barraDeVida2.CambiarVidaActual(currentLives);
    }


    private IEnumerator ShowDialogAndHideAfterDelay()
    {
        dialogBubble2.SetActive(true); // Activa la nube de diálogo

        yield return new WaitForSeconds(dialogDuration);

        dialogBubble2.SetActive(false); // Desactiva la nube de diálogo
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        // Realizar acciones visuales para indicar la invulnerabilidad del jugador (por ejemplo, cambiar el color del sprite)

        yield return new WaitForSeconds(invincibilityTime);

        isInvincible = false;
    }

    private void TryJumpOnPlayer1()
    {
        RaycastHit2D hitGround = Physics2D.Raycast(transform.position, Vector2.down, 1f);
        RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Player"));

        if (hitGround.collider != null && hitGround.collider.CompareTag("Ground") && hitPlayer.collider != null && hitPlayer.collider.CompareTag("Player"))
        {
            // Saltar sobre el jugador 1
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void PushPlayer1()
    {
        if (player1 != null)
        {
            Vector2 pushDirection = (player1.transform.position - transform.position).normalized;
            Rigidbody2D player1Rb = player1.GetComponent<Rigidbody2D>();

            if (player1Rb != null)
            {
                player1Rb.velocity = Vector2.zero;
                player1Rb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
            }

            player1.TakeDamage(pushDamage);
        }
        animator.SetTrigger("Attack"); // Activar la animación de golpe
    }
}
