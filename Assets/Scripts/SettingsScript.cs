using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] GameObject s_menu;

    [SerializeField] Slider m_slider;
    [SerializeField] Slider sfx_slider;

    //handling the settings menu
    public void openSettings()
    {
        AudioManager.instance.PlaySingleSFX(AudioManager.instance.buttonpress);
        s_menu.SetActive(true);
    }

    public void closeSettings()
    {
        s_menu.SetActive(false);
    }

    private void Update()
    {
        if(AudioManager.instance.music_volume != m_slider.value)
        {
            AudioManager.instance.music_volume = m_slider.value;
        }

        if (AudioManager.instance.sfx_volume != sfx_slider.value)
        {
            AudioManager.instance.sfx_volume = sfx_slider.value;
        }
    }
}
