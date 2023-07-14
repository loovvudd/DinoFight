using UnityEngine;

public class ObjectDisappearing : MonoBehaviour
{
    public int tiempoDesaparicion;

    private void Start()
    {
        Invoke("Disappear", tiempoDesaparicion);
    }

    private void Disappear()
    {
        gameObject.SetActive(false);
    }
}
