using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    public GameObject meteorPrefab; // Prefab del meteorito a instanciar
    public float spawnRate = 2f; // Tasa de generación de meteoritos por segundo

    [SerializeField]
    private float spawnRadius = 5f; // Radio del área de generación visible en la pestaña "Scene"

    public float meteorLifetime = 5f; // Tiempo de vida de los meteoritos clonados

    public string meteorTag = "Enemy"; // Tag asignado a los meteoritos clonados

    private float nextSpawnTime; // Tiempo para la próxima generación

    private void Start()
    {
        nextSpawnTime = Time.time;
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnMeteor();
            CalculateNextSpawnTime();
        }
    }

    private void SpawnMeteor()
    {
        Vector2 spawnPosition = GetRandomSpawnPosition();
        GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);
        meteor.tag = meteorTag; // Asignar el tag "Enemy" al meteorito clonado
        Destroy(meteor, meteorLifetime);
    }

    private Vector2 GetRandomSpawnPosition()
    {
        Vector2 randomPosition = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomPosition.x, randomPosition.y, 0f);
        return spawnPosition;
    }

    private void CalculateNextSpawnTime()
    {
        nextSpawnTime = Time.time + 1f / spawnRate;
    }
}
