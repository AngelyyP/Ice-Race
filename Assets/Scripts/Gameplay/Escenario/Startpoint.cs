using UnityEngine;

public class StartPoint : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CameraFollow assignedCamera;

    private void Start()
    {
        if (playerPrefab == null) return;

        GameObject player = Instantiate(playerPrefab, transform.position, transform.rotation);

        PlayerController pc = player.GetComponent<PlayerController>();
        //if (pc != null && assignedCamera != null)
           // pc.SetCamera(assignedCamera);
    }
}