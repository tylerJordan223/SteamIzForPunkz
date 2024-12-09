using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static bool paused = false;

    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void Update()
    {
        //swap between paused and not paused by pressing escape
        if (DataManager.playing && Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameObject.Find("Tric").GetComponent<PlayerScript>().canControl)
            {
                CyclePause();
            }
        }
    }

    public void CyclePause()
    {
        if(paused)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            paused = false;
            Time.timeScale = 1;
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            paused = true;
            Time.timeScale = 0;
        }
    }

    public void RestartRun()
    {
        Time.timeScale = 1;
        DataManager.RunStart();
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    //used to change settings, in this case sound bars
    public void SettingsMenu()
    {

    }
}
