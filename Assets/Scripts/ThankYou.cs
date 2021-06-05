using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThankYou : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Main Menu");
        AudioListener.pause = false;
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
