using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

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

    // Ahora ambos botones ejecutarán la misma lógica sincronizada
    public void SelectPlayer1() { AssignDynamicAndStart(); }
    public void SelectPlayer2() { AssignDynamicAndStart(); }

    private void AssignDynamicAndStart()
    {
        int dynamicId = 0;
        string path = Application.temporaryCachePath + "/multiplayer_id_sync.txt";
        
        try 
        {
            if (File.Exists(path))
            {
                // Si el registro se modificó hace menos de 15 segundos, significa que
                // el otro cliente acaba de adquirir el player 1, por lo que tomamos el ID acumulado.
                var diff = System.DateTime.Now - File.GetLastWriteTime(path);
                if (diff.TotalSeconds < 15) 
                {
                    string txt = File.ReadAllText(path);
                    int.TryParse(txt, out dynamicId);
                }
            }
            // Anotamos que un jugador más se ha asignado para el siguiente cliente
            File.WriteAllText(path, (dynamicId + 1).ToString());
        }
        catch 
        {
            dynamicId = 0; // Fallback
        }

        // dynamicId % 2 garantizará que el primer click sea 0 (Player 1) y el segundo sea 1 (Player 2)
        int finalId = dynamicId % 2; 

        // Guardamos la decisión
        PlayerSession.SelectedPlayerId = finalId;
        
        PlayerTransfer transfer = FindObjectOfType<PlayerTransfer>();
        if (transfer != null)
        {
            transfer.SelectedPlayerId = finalId;
        }

        // Cargamos la escena del juego
        SceneManager.LoadScene("Gameplay");
    }
}