using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager sTheGlobalBehavior = null;
    private PlatformManager mPlatformManager = null;
    private PlayerManager mPlayerManager = null;
    private BlockListManager mBlockListManager = null;
    void Start()
    {

        GameManager.sTheGlobalBehavior = this;
        mPlayerManager = gameObject.AddComponent<PlayerManager>();
        mPlatformManager = new PlatformManager();
        mBlockListManager = gameObject.AddComponent<BlockListManager>();
        
    }

    public PlayerManager GetPlayerManager()
    {
        return this.mPlayerManager;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
