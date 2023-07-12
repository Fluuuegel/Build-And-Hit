using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager sTheGlobalBehavior = null;
    private CubeManager mCubeManager = null;
    private PlatformManager mPlatformManager = null;
    private PlayerManager mPlayerManager = null;
    void Start()
    {

        GameManager.sTheGlobalBehavior = this;
        mCubeManager = new CubeManager();
        mPlatformManager = new PlatformManager();
        mPlayerManager = new PlayerManager();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
