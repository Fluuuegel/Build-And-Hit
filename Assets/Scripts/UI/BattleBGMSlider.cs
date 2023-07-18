using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleBGMSlider : MonoBehaviour
{
    public Slider mVolumeSlider;
    public AudioSource mAudioSource;
    private static float mSavedBattleVolume = 0.5f;

    void Start()
    {
        mVolumeSlider.value = mSavedBattleVolume;
        mAudioSource = GameObject.Find("BattleBGM").GetComponent<AudioSource>();
        mAudioSource.volume = mVolumeSlider.value;
    }

    // Update is called once per frame
    void Update()
    {
        mAudioSource.volume = mVolumeSlider.value;
        mSavedBattleVolume = mVolumeSlider.value;
    }
    
}
