using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public GameObject playerSelectionPanel; // Panel con los botones de Player 1 y 2

    // Este se asigna al botón inicial principal "Play"
    public void PlayGame()
    {
        if (playerSelectionPanel != null)
        {
            playerSelectionPanel.SetActive(true);
        }
    }

    public void SelectPlayer1()
    {
        StartGameWithID(0);
    }

    public void SelectPlayer2()
    {
        StartGameWithID(1);
    }

    private void StartGameWithID(int id)
    {
        // Guardamos la decisión en la clase estática para leerla en la otra escena sin chocar en memoria
        PlayerSession.SelectedPlayerId = id;
        
        // Cargamos la escena del juego
        SceneManager.LoadScene("Gameplay");
    }
}