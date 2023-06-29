using UnityEngine;

public class Seguidor : MonoBehaviour
{
    private Transform target; // Referencia al transform del jugador
    public float velocidadSeguimiento = 5f;


    public void ComenzarSeguimiento(Transform nuevoTarget)
    {
        target = nuevoTarget;
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector2 direccion = target.position - transform.position;
            direccion.Normalize();
            Vector2 movimiento = direccion * velocidadSeguimiento * Time.fixedDeltaTime;
            GetComponent<Rigidbody2D>().velocity = movimiento;
        }
    }
}

