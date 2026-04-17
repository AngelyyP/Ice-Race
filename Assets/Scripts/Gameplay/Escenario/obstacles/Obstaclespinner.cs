using UnityEngine;

public class ObstacleSpinner : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 120f;

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
    }
}