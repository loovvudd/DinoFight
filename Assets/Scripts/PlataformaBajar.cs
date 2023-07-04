using UnityEngine;

public class PlataformaBajar : MonoBehaviour
{
    public KeyCode teclaBajarJugador1 = KeyCode.DownArrow; // Tecla de bajar para el Jugador 1
    public KeyCode teclaBajarJugador2 = KeyCode.S; // Tecla de bajar para el Jugador 2

    private PlatformEffector2D platformEffector; // Referencia al componente PlatformEffector2D

    private void Start()
    {
        platformEffector = GetComponent<PlatformEffector2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(teclaBajarJugador1))
        {
            platformEffector.rotationalOffset = 180f; // Girar la plataforma 180 grados
        }
        else if (Input.GetKeyDown(teclaBajarJugador2))
        {
            platformEffector.rotationalOffset = 180f; // Girar la plataforma 180 grados
        }

        if (Input.GetKeyUp(teclaBajarJugador1) || Input.GetKeyUp(teclaBajarJugador2))
        {
            platformEffector.rotationalOffset = 0f; // Restaurar la rotación de la plataforma a 0 grados
        }
    }
}
