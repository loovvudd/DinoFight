using UnityEngine;

public class EnemigoMovimiento : MonoBehaviour
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

        // Gira el sprite si la dirección es negativa en el eje X
        if (direction < 0f)
        {
            // Invierte la escala en el eje X
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            // Restaura la escala original en el eje X
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
