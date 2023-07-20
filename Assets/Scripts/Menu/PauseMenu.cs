using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public GameObject mPauseCanvas;
    [SerializeField] private bool mMenuKey = true;
    [SerializeField] private AudioSource mAudioSource;
    public GameObject mTutorialPanel;
    public GameObject mTutorialBackButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mMenuKey)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                mPauseCanvas.SetActive(true);
                mMenuKey = false;
                Time.timeScale = 0f;
                mAudioSource.Pause();
            }
            
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && !mTutorialPanel.activeInHierarchy)
        {
            mPauseCanvas.SetActive(false);
            mMenuKey = true;
            Time.timeScale = 1f;
            mAudioSource.Play();
        }

        if(Input.GetKeyDown(KeyCode.Escape) && mTutorialPanel.activeInHierarchy) 
        { 
            mTutorialPanel.SetActive(false);
        }
       
    }
    public void ClosePauseMenu()
    {
        mPauseCanvas.SetActive(false);
        mMenuKey = true;
        Time.timeScale = 1f;
        mAudioSource.Play();
    }

    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ShowTutorialPanel()
    {
        mTutorialPanel.SetActive(true);
        mTutorialBackButton.SetActive(true);
    }

    public void UnshowTutorialPanel()
    {
        mTutorialPanel.SetActive(false);
        mTutorialBackButton.SetActive(false);
    }
}
