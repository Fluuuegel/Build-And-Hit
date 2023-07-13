using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Player1Select : MonoBehaviour
{
    static public string player1Select = null;
    public Button BackSceneButton;

    public void BackToLastScene()
    {
        Debug.Log("Back");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Player1Select");
    }

    public void SelectChar1()
    {
        player1Select = "Char1";
    }

    public void SelectChar2()
    {
        player1Select = "Char2";
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            BackSceneButton.onClick.Invoke();
        }
    }
}
