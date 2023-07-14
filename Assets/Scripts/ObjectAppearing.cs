using UnityEngine;

public class ObjectAppearing : MonoBehaviour
{
    private void Start()
    {
        // Invocar la función Disappear después de 3 segundos
        Invoke("Appear", 3f);
    }

    private void Appear()
    {
        // Desactivar el objeto para que desaparezca
        gameObject.SetActive(true);
    }
}
