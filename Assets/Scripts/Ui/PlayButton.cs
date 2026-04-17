using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public GameObject menuUI;
    public GameObject gameplay;

    public void PlayGame()
    {
        menuUI.SetActive(false);
        gameplay.SetActive(true);
    }
}