using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject gameplay;
    public GameObject menuUI;

    private bool isPaused = false;

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void GoToMenu()
    {
        pausePanel.SetActive(false);
        gameplay.SetActive(false);
        menuUI.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
    }
}