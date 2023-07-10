using UnityEngine;
using System.Collections;

public class Player1 : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeedMultiplier = 2f;
    public KeyCode runKey = KeyCode.LeftShift;
    public float maxRunTime = 5f;
    public float waitTimeAfterRun = 2f;
    public GameObject dialogBubble; // Referencia al objeto de la nube de diálogo
    public float dialogDuration = 3f; // Duración en segundos de la nube de diálogo
    private float currentRunTime = 0f;
    private bool isWaiting = false;
    public float dialogOffsetY = 1f; // Desplazamiento vertical de la nube de diálogo con respecto al jugador
    public float dialogOffsetX = 0f; // Desplazamiento horizontal de la nube de diálogo con respecto al jugador
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool isDead = false;
    public GameObject objetoSeguidor;
    public float fuerzaLanzamiento = 5f; // Fuerza con la que se lanza el objeto hacia arriba
    private SpriteRenderer spriteRenderer;
    public Color healColor = Color.green;
    public float colorChangeDuration = 1f;
    private bool canTouchObject = true; // Variable para rastrear si el Jugador 2 puede tocar el objeto seguidor
    private int initialPushDamage;
    public AudioSource audioSource; // Referencia al componente AudioSource del jugador
    public AudioClip damage; // Sonido a reproducir cuando el jugador recibe daño
    private BarraDeVida barraDeVida;
    public float jumpForce = 7f;

    public int maxLives = 3;
    public int currentLives;
    public float powerDownDuration = 10f;
    public float powerDownScaleFactor = 0.5f;
    public float powerDownSpeedMultiplier = 0.5f;
    public int powerDownDamageReduction = 1;

    private bool isPowerDownActive = false;
    private float originalMoveSpeed;
    private Vector3 originalScale;
    private bool isInvincible = false;
    public float invincibilityTime = 2f;
    private Animator animator; // Referencia al componente Animator
    public KeyCode pushButton = KeyCode.L;
    public float pushForce = 10f;
    public int pushDamage = 1;

    public Player2 player2; // Referencia al script del jugador 2

    private bool isTouchingPlayer2 = false; // Variable para rastrear si el jugador 1 está tocando al jugador 2
   public bool isPowerUpActive = false;
    public float powerUpDuration = 10f;

    private float targetScaleFactor = 2f;

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
        barraDeVida = FindObjectOfType<BarraDeVida>();
        barraDeVida.InicializarBarraDeVida(currentLives);
        originalMoveSpeed = moveSpeed;
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

            // Ejecutar animación de correr
            animator.SetBool("IsRunning", true);
        }
        else
        {
            if (currentRunTime < maxRunTime)
            {
                currentRunTime += Time.deltaTime;
                isWaiting = false;
            }

            // Detener animación de correr
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

       
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isGrounded && Input.GetKey(runKey)) // Permitir saltar si está en el suelo o si se mantiene presionada la tecla de correr
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            }
            else
            {
                // Intentar saltar sobre el jugador 2
                TryJumpOnPlayer2();
            }
            if (isGrounded) // Permitir saltar si está en el suelo o si se mantiene presionada la tecla de correr
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            }
            animator.SetTrigger("Jump");
        }

        if (Input.GetKeyDown(pushButton) && isTouchingPlayer2) // Solo permite el ataque si está tocando al jugador 2
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
            TakeDamage(-1);
        }

        if (collision.gameObject.CompareTag("Player")) // Comprueba si está tocando al jugador 2
        {
            isTouchingPlayer2 = true;
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
        if (collision.collider.CompareTag("Item"))
        {
            if (player2 != null && player2.objetoSeguidor != null && player2.objetoSeguidor.transform.parent == transform)
            {
                canTouchObject = false;
            }
            else
            {
                canTouchObject = true;
            }
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
        // Reducir el tamaño del jugador
        transform.localScale = originalScale * powerDownScaleFactor;

        // Reducir la velocidad del jugador
        moveSpeed *= powerDownSpeedMultiplier;

        // Reducir el daño del jugador
        pushDamage -= powerDownDamageReduction;

        yield return new WaitForSeconds(powerDownDuration);

        // Restaurar el tamaño, velocidad y daño original del jugador
        transform.localScale = originalScale;
        moveSpeed = originalMoveSpeed;
        pushDamage = initialPushDamage;

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
    public void TakeDamage(int dmg)
    {
        if (isDead)
            return;
        currentLives -= dmg;
        barraDeVida.CambiarVidaActual(currentLives);

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
            // Activar la animación de daño al instante
            animator.Play("Damage1", -1, 0f); // Reemplaza "DamageAnimation" con el nombre de la animación de daño en el Animator
            animator.Update(0f); // Actualiza el estado de la animación al inicio para que se muestre de inmediato
            audioSource.PlayOneShot(damage);
        }
       

        IEnumerator DisablePlayerCoroutine()
        {
            yield return new WaitForSeconds(0.890f); // Tiempo de espera para que termine la animación de muerte

            gameObject.SetActive(false); // Desactivar el objeto del jugador
        }
        if (objetoSeguidor != null && objetoSeguidor.transform.parent == transform)
        {
            Rigidbody2D objetoRigidbody = objetoSeguidor.GetComponent<Rigidbody2D>();
            objetoSeguidor.transform.SetParent(null); // Despegar el objeto del jugador
            objetoRigidbody.isKinematic = false; // Activar la física del objeto
            objetoRigidbody.velocity = new Vector2(0f, fuerzaLanzamiento); // Lanzar el objeto hacia arriba
        }
    }

    IEnumerator DisablePowerUpAfterDuration()
    {
        yield return new WaitForSeconds(powerUpDuration);

        // Restaurar el tamaño original del jugador
        float scaleFactor = 0.5f; // Factor de escala para reducir el tamaño a la mitad
        SpriteRenderer playerSpriteRenderer = GetComponent<SpriteRenderer>();
        playerSpriteRenderer.transform.localScale /= scaleFactor;

        isPowerUpActive = false;
    }

    public IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibilityTime);

        isInvincible = false;

       
    }
    private IEnumerator ShowDialogAndHideAfterDelay()
    {
        dialogBubble.SetActive(true); // Activa la nube de diálogo

        yield return new WaitForSeconds(dialogDuration);

        dialogBubble.SetActive(false); // Desactiva la nube de diálogo
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

    public void Heal(int amount)
    {
        currentLives += amount;
        Debug.Log("Player 1 healed. Current lives: " + currentLives);

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

            player2.TakeDamage(pushDamage);
           
        }
        
    }
}
