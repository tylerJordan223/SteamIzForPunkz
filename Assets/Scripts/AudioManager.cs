using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //information
    public float music_volume;
    public float sfx_volume;

    //audio source
    [Header("Audio Source")]
    [SerializeField] AudioSource sfx;
    [SerializeField] AudioSource music;

    #region audios
    [SerializeField] public AudioClip bossSong;
    [SerializeField] public AudioClip normalSong;

    [SerializeField] public AudioClip spawn;
    [SerializeField] public AudioClip exit;
    [SerializeField] public AudioClip coin;

    #endregion audios

    //getting the audio instance
    public static AudioManager instance;

    private bool ignore_volume;

    private void Awake()
    {
        //Make sure there is only one instance at all times
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        //playing the normal first music
        music.clip = normalSong;
        ignore_volume = false;
        music.loop = true;
        music.Play();
        music_volume = 1.0f;
        sfx_volume = 1.0f;
    }

    public void PlaySingleSFX(AudioClip clip)
    {
        sfx.PlayOneShot(clip);
    }

    public void EndSong(AudioClip song)
    {
        StartCoroutine(FadeMusic(song));
    }

    public void PlaySong(AudioClip song)
    {
        music.clip = song;
        music.Play();
    }

    //music fadeout transition
    private IEnumerator FadeMusic(AudioClip song)
    {
        ignore_volume = true;

        while(music.volume != 0)
        {
            music.volume -= 0.05f;
            yield return new WaitForSeconds(0.1f);
        }

        music.Stop();
        ignore_volume = false;
    }

    private void Update()
    {
        if (!ignore_volume)
        {
            if(music.volume != music_volume)
            {
                music.volume = music_volume;
            }
            if(sfx.volume !=  sfx_volume)
            {
                sfx.volume = sfx_volume;
            }
        }
        
    }
}
