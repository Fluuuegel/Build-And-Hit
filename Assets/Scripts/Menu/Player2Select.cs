using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player2Select : MonoBehaviour
{
    public Button BackToPlayer1Select;
    public void PVPGameActive()
    {
        Debug.Log("PVP game is now active!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
    // Start is called before the first frame update
    void Start()
    {
        
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
