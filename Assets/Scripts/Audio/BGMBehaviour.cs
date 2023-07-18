using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMBehaviour : MonoBehaviour
{
   private static BGMBehaviour instance;

    // Start is called before the first frame update
    public void Start()
    {
        
        if(instance == null )
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex == 4)
        {
            Destroy(gameObject);
        }
    }
}
