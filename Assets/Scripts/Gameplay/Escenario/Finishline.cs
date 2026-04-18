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

        Debug.Log("El jugador cruzo la meta");

        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            // Congelar a todos los jugadores simulando el fin de la carrera
            PlayerController[] allPlayers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
            foreach (var p in allPlayers)
            {
                p.FinishRace();
            }

            triggered = true;

            if (winCanvas != null)
                winCanvas.SetActive(true);

            if (winText != null)
            {
                if (pc.isLocalPlayer)
                {
                    winText.text = "¡Ganaste!";
                    winText.color = Color.green;
                }
                else
                {
                    winText.text = "¡Perdiste!";
                    winText.color = Color.red;
                }
            }
        }
    }
}
