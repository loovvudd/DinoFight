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

    private Rigidbody2D rb;
    private bool isGrounded = false;
    public float jumpForce = 7f;

    public int maxLives = 3;
    public int currentLives;

    private bool isInvincible = false;
    public float invincibilityTime = 2f;

    private Animator animator; // Referencia al componente Animator
    public KeyCode pushButton = KeyCode.G;
    public float pushForce = 10f;
    public int pushDamage = 1;

   
    private bool isDead = false;
    private bool isTouchingPlayer1 = false; // Variable para rastrear si el jugador 1 está tocando al jugador 2
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Asignar componente Animator
        currentLives = maxLives;
        StartCoroutine(ShowDialogAndHideAfterDelay());
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
            // Ejecutar animación de correr
            animator.SetBool("IsRunning", false);
        }

        moveSpeed = moveSpeed * moveSpeedMultiplier;

        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

        // Establecer el parámetro "Move" del Animator
        animator.SetFloat("Move", Mathf.Abs(horizontalInput));
        // Girar el personaje en la dirección del movimiento
        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f); // Mirar hacia la derecha
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f); // Mirar hacia la izquierda
        }


        if (Input.GetKeyDown(KeyCode.W))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
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
        }

        if (collision.gameObject.CompareTag("Enemy") && !isInvincible)
        {
            TakeDamage();
        }
        if (collision.gameObject.CompareTag("Player")) // Comprueba si está tocando al jugador 1
        {
            isTouchingPlayer1 = true;
        }
        if (collision.collider.CompareTag("Item") && canTouchObject)
        {
            Transform objectTransform = collision.transform;
            objectTransform.SetParent(transform); // Establecer el objeto colisionado como hijo del jugador
            objectTransform.localPosition = new Vector3(0f, 1f, 0f); // Desplazar el objeto hacia arriba del jugador
            collision.gameObject.GetComponent<Rigidbody2D>().isKinematic = true; // Desactivar la física del objeto colisionado
        }
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
            Debug.Log("Player 2 took damage. Remaining lives: " + currentLives);
            // Activar la animación de daño al instante
            animator.Play("Damage2", -1, 0f); // Reemplaza "DamageAnimation" con el nombre de la animación de daño en el Animator
            animator.Update(0f); // Actualiza el estado de la animación al inicio para que se muestre de inmediato
        }
        IEnumerator DisablePlayerCoroutine()
        {
            yield return new WaitForSeconds(0.89f); // Tiempo de espera para que termine la animación de muerte

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

            player1.TakeDamage();
        }
        animator.SetTrigger("Attack"); // Activar la animación de golpe
    }
}
