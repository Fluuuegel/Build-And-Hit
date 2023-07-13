using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player2Select : MonoBehaviour
{
    static public string player2Select = null;

    public Button BackToPlayer1Select;
    public void PVPGameActive()
    {
        Debug.Log("PVP game is now active!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Player2Select");
    }

    public void SelectChar1()
    {
        player2Select = "Char1";
    }

    public void SelectChar2()
    {
        player2Select = "Char2";
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            BackToPlayer1Select.onClick.Invoke();
        }
    }
}
