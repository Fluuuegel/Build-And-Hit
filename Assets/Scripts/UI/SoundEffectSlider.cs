using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundEffectSlider : MonoBehaviour
{
    public Slider mVolumeSlider;
    public AudioSource mAudioSource;

    private static float mSavedEffectVolume = 0.5f;

    void Start()
    {
        mVolumeSlider.value = mSavedEffectVolume;
        mAudioSource = FindObjectOfType<AudioSource>();
        mAudioSource.volume = mVolumeSlider.value;
    }

    // Update is called once per frame
    void Update()
    {
        mAudioSource.volume = mVolumeSlider.value;
        mSavedEffectVolume = mVolumeSlider.value;
    }

    public void OnVolumeChanged()
    {

    }
}
