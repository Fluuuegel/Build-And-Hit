using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuVolumeSlider : MonoBehaviour
{
    public Slider mVolumeSlider;
    public AudioSource mAudioSource;

    private static float mSavedVolume = 0.5f;

    void Start()
    {
        mVolumeSlider.value = mSavedVolume;
        mAudioSource = FindObjectOfType<AudioSource>();
        mAudioSource.volume = mVolumeSlider.value;
    }

    // Update is called once per frame
    void Update()
    {
        mAudioSource.volume = mVolumeSlider.value;
        mSavedVolume = mVolumeSlider.value;
    }

    public void OnVolumeChanged()
    {
        
    }
}
