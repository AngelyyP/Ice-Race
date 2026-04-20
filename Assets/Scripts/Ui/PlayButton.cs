using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class PlayButton : MonoBehaviour
{
    public GameObject playerSelectionPanel; 

    public void PlayGame()
    {
        if (playerSelectionPanel != null)
        {
            playerSelectionPanel.SetActive(true);
        }
    }

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

                var diff = System.DateTime.Now - File.GetLastWriteTime(path);
                if (diff.TotalSeconds < 15) 
                {
                    string txt = File.ReadAllText(path);
                    int.TryParse(txt, out dynamicId);
                }
            }
            File.WriteAllText(path, (dynamicId + 1).ToString());
        }
        catch 
        {
            dynamicId = 0; 
        }

        int finalId = dynamicId % 2; 


        PlayerSession.SelectedPlayerId = finalId;
        
        PlayerTransfer transfer = FindObjectOfType<PlayerTransfer>();
        if (transfer != null)
        {
            transfer.SelectedPlayerId = finalId;
        }

        SceneManager.LoadScene("Gameplay");
    }
}