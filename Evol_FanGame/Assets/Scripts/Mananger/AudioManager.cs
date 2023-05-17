using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager am;

    public AudioMixerGroup musicMixer;
    public AudioMixerGroup SFXMixer;
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public float musVol;
    public float sfxVol;

    [Space]
    [Header("Clips")]
    [NamedArray(typeof(eMusic))] public AudioClip[] aMusic;
    [NamedArray(typeof(eSFX))] public AudioClip[] aSFX;



    void Start()
    {
        if (am == null)
        {
            am = this;
        }
    }

    void PlayClickSound()
    {
        sfxSource.PlayOneShot(aSFX[(int)eSFX.click]);
    }

    public void ChangeSFXVolume(float _newVol)
    {
        SFXMixer.audioMixer.SetFloat("sfxVol", Mathf.Log10(_newVol) * 20);
    }

    public void ChangeMusicVolume(float _newVol)
    {
        musicMixer.audioMixer.SetFloat("musicVol", Mathf.Log10(_newVol) * 20);

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlayClickSound();
        }
    }


    public void PlaySFX(eSFX _sfx)
    {
        sfxSource.PlayOneShot(aSFX[(int)_sfx]);
    }

    public void StartMusic(eMusic _music)
    {
        musicSource.clip = aMusic[(int)_music];
        musicSource.Play();
    }



}
