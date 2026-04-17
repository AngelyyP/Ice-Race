using UnityEngine;

public class ObstacleRock : MonoBehaviour
{
    [SerializeField] private Vector3 rockScale = new Vector3(1f, 1f, 1f);

    private void Start()
    {
        transform.localScale = rockScale;
    }
}