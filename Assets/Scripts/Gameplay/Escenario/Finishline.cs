using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        pc.FinishRace();
    }
}
