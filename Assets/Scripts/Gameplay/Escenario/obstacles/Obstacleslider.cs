using UnityEngine;

public class ObstacleSlider : MonoBehaviour
{
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] private float moveSpeed = 2f;

    private Vector3 origin;

    private void Start()
    {
        origin = transform.position;
    }

    private void Update()
    {

        float offsetX = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = new Vector3(origin.x + offsetX, origin.y, origin.z);
    }
}