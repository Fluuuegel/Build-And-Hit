using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager sTheGlobalBehavior = null;
    private BlockManager mBlockManager = null;
    private PlatformManager mPlatformManager = null;
    private PlayerManager mPlayerManager = null;
    private TurnManager mTurnManager = null;
    private UIManager mUIManager = null;
    void Start()
    {

        GameManager.sTheGlobalBehavior = this;
        mPlayerManager = gameObject.AddComponent<PlayerManager>();
        mBlockManager = new BlockManager();
        mPlatformManager = new PlatformManager();
        mUIManager = gameObject.AddComponent<UIManager>();
        mTurnManager = gameObject.AddComponent<TurnManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
