using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] GameObject s_menu;

    [SerializeField] Slider m_slider;
    [SerializeField] Slider sfx_slider;

    private bool active;

    //handling the settings menu
    public void openSettings()
    {
        m_slider.value = AudioManager.instance.music_volume;
        sfx_slider.value = AudioManager.instance.sfx_volume;

        AudioManager.instance.PlaySingleSFX(AudioManager.instance.buttonpress);
        s_menu.SetActive(true);
        active = true;
    }

    public void closeSettings()
    {
        active = false;
        s_menu.SetActive(false);
    }

    private void Update()
    {
        if(active)
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
}
