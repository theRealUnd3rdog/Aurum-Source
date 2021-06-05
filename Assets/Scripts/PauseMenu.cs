using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameisPaused = false;

    public GameObject pauseMenuUI;
    public GameObject playerUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (GameisPaused)
            {
                Resume();
            }
            else 
            {
                Pause();
            }
        }
    }

    public void Resume() 
    {
        pauseMenuUI.SetActive(false);
        playerUI.SetActive(true);
        AudioListener.pause = false;
        Time.timeScale = 1f;
        GameisPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause() 
    {
        pauseMenuUI.SetActive(true);
        playerUI.SetActive(false);
        AudioListener.pause = true;
        Time.timeScale = 0f;
        GameisPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        AudioListener.pause = false;
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
