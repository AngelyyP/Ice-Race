using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinishLine : MonoBehaviour
{
    [Header("Win UI")]
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private TextMeshProUGUI winText;

    private bool triggered = false;

    private void Start()
    {
        if (winCanvas != null)
            winCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (!other.CompareTag("Player")) return;

        Debug.Log("El jugador cruzó la meta");

        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.FinishRace();
        }

        triggered = true;

        if (winCanvas != null)
            winCanvas.SetActive(true);

        if (winText != null)
            winText.text = "¡Ganaste!";
    }
}