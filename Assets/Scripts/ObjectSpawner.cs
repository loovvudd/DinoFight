using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] objectsToSpawn; // Objetos para spawnear
    public int maxObjectsToSpawn = 5; // Número máximo de objetos a spawnear
    public float spawnAreaSizeX = 5f; // Tamaño del área de spawn en el eje X
    public float spawnAreaSizeY = 5f; // Tamaño del área de spawn en el eje Y
    public float spawnInterval = 1f; // Intervalo de tiempo entre cada spawn

    private void OnDrawGizmosSelected()
    {
        // Dibujar un cubo en el área de spawn
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSizeX, spawnAreaSizeY, 0f));
    }

    private void Start()
    {
        InvokeRepeating("SpawnObject", 0f, spawnInterval);
    }

    private void SpawnObject()
    {
        if (transform.childCount >= maxObjectsToSpawn)
        {
            return; // Ya se han spawnado el número máximo de objetos
        }

        // Generar una posición aleatoria dentro del área de spawn
        float randomX = Random.Range(-spawnAreaSizeX, spawnAreaSizeX);
        float randomY = Random.Range(-spawnAreaSizeY, spawnAreaSizeY);
        Vector2 randomPosition = (Vector2)transform.position + new Vector2(randomX, randomY);

        // Elegir un objeto aleatorio de la lista
        GameObject objectToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];

        // Spawnear el objeto solo si no ha sido destruido
        if (objectToSpawn != null)
        {
            Instantiate(objectToSpawn, randomPosition, Quaternion.identity);
        }
    }
}
