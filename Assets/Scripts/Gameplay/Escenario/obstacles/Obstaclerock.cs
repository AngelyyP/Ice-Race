using UnityEngine;

public class ObstacleStatic : MonoBehaviour
{
    [SerializeField] private Vector3 rockScale = new Vector3(1f, 1f, 1f);

    private void Start()
    {
        transform.localScale = rockScale;
    }
}