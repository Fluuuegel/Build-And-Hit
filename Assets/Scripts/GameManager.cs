using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager sTheGlobalBehavior = null;
    private BlockManager mBlockManager = null;
    private PlatformManager mPlatformManager = null;
    private PlayerManager mPlayerManager = null;
    void Start()
    {

        GameManager.sTheGlobalBehavior = this;
        mBlockManager = new BlockManager();
        mPlatformManager = new PlatformManager();
        mPlayerManager = new PlayerManager();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
