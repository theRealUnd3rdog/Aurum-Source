using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EZCameraShake;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        CameraShaker.Instance.ShakeOnce(0.3f, 0.1f, 1f, 2f);
    }

    public void OpenLevelLayout()
    {
        
    }

    public void Settings()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
}
