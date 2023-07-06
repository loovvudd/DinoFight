using UnityEngine;
using System.Collections;

public class Player1 : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeedMultiplier = 2f;
    public KeyCode runKey = KeyCode.LeftShift;
    public float maxRunTime = 5f;
    public float waitTimeAfterRun = 2f;
    public GameObject dialogBubble;
    public float dialogDuration = 3f; 
    private float currentRunTime = 0f;
    private bool isWaiting = false;
    public float dialogOffsetY = 1f; 
    public float dialogOffsetX = 0f; 
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool isDead = false;
    public GameObject objetoSeguidor;
    public float fuerzaLanzamiento = 5f; 

    public Color healColor = Color.green;
    public float colorChangeDuration = 1f;
    private bool canTouchObject = true; 

    public AudioSource audioSource;
    public AudioClip damage; 

    public float jumpForce = 7f;
    
    private BarraDeVida barraDeVida;
    public int maxLives = 3;
    public int currentLives;

    private bool isInvincible = false;
    public float invincibilityTime = 2f;
    private Animator animator; 
    public KeyCode pushButton = KeyCode.L;
    public float pushForce = 10f;
    public int pushDamage = 1;

    public Player2 player2; 

    private bool isTouchingPlayer2 = false; 

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentLives = maxLives;
        barraDeVida = FindObjectOfType<BarraDeVida>();
        barraDeVida.InicializarBarraDeVida(currentLives);
        animator = GetComponent<Animator>(); 
        StartCoroutine(ShowDialogAndHideAfterDelay());
        audioSource = GetComponent<AudioSource>();
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

            animator.SetBool("IsRunning", true);
        }
        else
        {
            if (currentRunTime < maxRunTime)
            {
                currentRunTime += Time.deltaTime;
                isWaiting = false;
            }

            animator.SetBool("IsRunning", false);
        }

        moveSpeed = moveSpeed * moveSpeedMultiplier;

        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        animator.SetFloat("Move", Mathf.Abs(horizontalInput));
        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f); 
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f); 
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isGrounded && Input.GetKey(runKey))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            }
            else
            {
                TryJumpOnPlayer2();
            }
            if (isGrounded) 
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            }
            animator.SetTrigger("Jump");
        }

        if (Input.GetKeyDown(pushButton) && isTouchingPlayer2)
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

        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer2 = true;
        }
        
            if (collision.collider.CompareTag("Item") && canTouchObject)
            {
            Transform objectTransform = collision.transform;
            objectTransform.SetParent(transform);
            objectTransform.localPosition = new Vector3(0f, 1f, 0f);
            collision.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
            }
        

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = false;

        }

        if (collision.gameObject.CompareTag("Player"))
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
   

    public void TakeDamage()
    {
        if (isDead)
            return;
        currentLives--; 
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
            animator.Play("Damage1", -1, 0f);
            animator.Update(0f);
            audioSource.PlayOneShot(damage);
        }
        
        IEnumerator DisablePlayerCoroutine()
        {
            yield return new WaitForSeconds(0.890f);

            gameObject.SetActive(false); 
        }
        if (objetoSeguidor != null && objetoSeguidor.transform.parent == transform)
        {
            Rigidbody2D objetoRigidbody = objetoSeguidor.GetComponent<Rigidbody2D>();
            objetoSeguidor.transform.SetParent(null);
            objetoRigidbody.isKinematic = false;
            objetoRigidbody.velocity = new Vector2(0f, fuerzaLanzamiento);
        }
    }

   
    public IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibilityTime);

        isInvincible = false;

       
    }
    private IEnumerator ShowDialogAndHideAfterDelay()
    {
        dialogBubble.SetActive(true);

        yield return new WaitForSeconds(dialogDuration);

        dialogBubble.SetActive(false);
    }
    private void TryJumpOnPlayer2()
    {
        RaycastHit2D hitGround = Physics2D.Raycast(transform.position, Vector2.down, 1f);
        RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Player"));

        if (hitGround.collider != null && hitGround.collider.CompareTag("Ground") && hitPlayer.collider != null && hitPlayer.collider.CompareTag("Player"))
        {
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

            player2.TakeDamage();
           
        }
        
    }
}
