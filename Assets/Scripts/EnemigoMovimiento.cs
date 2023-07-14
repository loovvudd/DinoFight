using UnityEngine;

public class EnemigoMovimiento : MonoBehaviour
{
    public float speed = 2f; // Velocidad de movimiento
    public float distance = 5f; // Distancia m�xima de movimiento en el eje X

    private Vector3 startPosition; // Posici�n inicial
    private float direction = 1f; // Direcci�n del movimiento

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Calcula el desplazamiento en el eje X seg�n la direcci�n y la velocidad
        float displacement = speed * direction * Time.deltaTime;

        // Calcula la nueva posici�n en el eje X
        float newX = transform.position.x + displacement;

        // Comprueba si se ha alcanzado la distancia m�xima y cambia la direcci�n
        if (Mathf.Abs(newX - startPosition.x) >= distance)
        {
            direction *= -1f;
        }

        // Actualiza la posici�n de la plataforma
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        // Gira el sprite si la direcci�n es negativa en el eje X
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
