using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 2f; // Velocidad de movimiento
    public float distance = 5f; // Distancia máxima de movimiento en el eje X

    private Vector3 startPosition; // Posición inicial
    private float direction = 1f; // Dirección del movimiento

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Calcula el desplazamiento en el eje X según la dirección y la velocidad
        float displacement = speed * direction * Time.deltaTime;

        // Calcula la nueva posición en el eje X
        float newX = transform.position.x + displacement;

        // Comprueba si se ha alcanzado la distancia máxima y cambia la dirección
        if (Mathf.Abs(newX - startPosition.x) >= distance)
        {
            direction *= -1f;
        }

        // Actualiza la posición de la plataforma
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}
