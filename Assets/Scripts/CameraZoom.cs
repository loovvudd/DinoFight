using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject player1;
    public GameObject player2;
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 10f;

    private bool player1Alive = true;
    private bool player2Alive = true;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (player1 == null || !player1.activeSelf)
        {
            player1Alive = false;
        }

        if (player2 == null || !player2.activeSelf)
        {
            player2Alive = false;
        }

        if (!player1Alive && player2Alive)
        {
            ZoomToPlayer(player2.transform);
        }
        else if (player1Alive && !player2Alive)
        {
            ZoomToPlayer(player1.transform);
        }
    }

    private void ZoomToPlayer(Transform playerTransform)
    {
        float currentSize = mainCamera.orthographicSize;
        float targetSize = Mathf.Clamp(currentSize - zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        mainCamera.orthographicSize = targetSize;

        Vector3 targetPosition = playerTransform.position;
        targetPosition.z = transform.position.z;
        transform.position = targetPosition;
    }
}
