using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FinishLine : MonoBehaviour
{
    [Header("Win UI")]
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private Button menuButton;

    private bool triggered = false;

    private void Start()
    {
        if (winCanvas != null)
            winCanvas.SetActive(false);

        if (menuButton != null)
            menuButton.onClick.AddListener(GoToMenu);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (!other.CompareTag("Player")) return;

        Debug.Log("El jugador cruzo la meta");

        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
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

    private void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}