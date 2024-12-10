using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] GameObject s_menu;

    //handling the settings menu
    public void openSettings()
    {
        s_menu.SetActive(true);
    }

    public void closeSettings()
    {
        s_menu.SetActive(false);
    }
}
