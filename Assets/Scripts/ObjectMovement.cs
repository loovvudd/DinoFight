using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public float speed = 2f; // Velocidad ajustable del movimiento
    public Transform leftPoint; // Punto izquierdo
    public Transform rightPoint; // Punto derecho
    public float waitTime = 1f; // Tiempo de espera en el punto específico
    public float pushForce = 5f; // Fuerza con la que se empuja al jugador
    public float pushUpForce = 5f; // Fuerza con la que se empuja hacia arriba

    private Transform targetPoint; // Punto objetivo actual
    private bool isMovingRight = true; // Indica si el objeto se está moviendo hacia la derecha

    private void Start()
    {
        targetPoint = rightPoint;
    }

    private void Update()
    {
        MoveToTargetPoint();
    }

    private void MoveToTargetPoint()
    {
        // Calcular el desplazamiento en el eje X
        float movement = speed * Time.deltaTime * (isMovingRight ? 1f : -1f);

        // Calcular la nueva posición en el eje X
        float newPositionX = transform.position.x + movement;

        // Actualizar la posición del objeto
        transform.position = new Vector3(newPositionX, transform.position.y, transform.position.z);

        // Comprobar si el objeto ha llegado al punto objetivo
        if ((isMovingRight && newPositionX >= targetPoint.position.x) || (!isMovingRight && newPositionX <= targetPoint.position.x))
        {
            // Cambiar la dirección de movimiento
            isMovingRight = !isMovingRight;

            // Esperar un tiempo antes de cambiar el punto objetivo
            StartCoroutine(WaitBeforeMoving());
        }
    }

    private System.Collections.IEnumerator WaitBeforeMoving()
    {
        yield return new WaitForSeconds(waitTime);

        // Cambiar el punto objetivo
        targetPoint = (isMovingRight) ? leftPoint : rightPoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto ha chocado con el jugador
        if (other.CompareTag("Player"))
        {
            // Calcular la dirección diagonal del empuje hacia arriba
            Vector3 pushDirection = (isMovingRight) ? new Vector3(1f, 1f, 0f) : new Vector3(-1f, 1f, 0f);

            // Aplicar la fuerza de empuje al jugador
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            playerRigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            playerRigidbody.AddForce(Vector3.up * pushUpForce, ForceMode.Impulse);
        }
    }
}
