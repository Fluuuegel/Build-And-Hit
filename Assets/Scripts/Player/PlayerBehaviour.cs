using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    private enum BlockColor {
        eRed,
        eGreen,
        eBlue
    };

    private enum PlayerState {
        eIdle,
        eWait,
        eBuild,
        eHit
    };

    private BlockColor mBlockColor = BlockColor.eRed;
    private PlayerState mPlayerState = PlayerState.eIdle;
    public Vector2 mPlayerInitPos = new Vector2(-2.5f, 10f);
    public bool isLocked = true;

    private bool isBuild = true;
    // private BlockManager mBlockManager = null;
    private int mHitNum = 0;

    private Animator animator;

    private PlayerManager mPlayerManager = null;

    void Start() {
        animator = GetComponent<Animator>();
    }

    public void Lock() {
        this.isLocked = true;
    }

    public void unLock() {
        this.isLocked = false;
    }
    
    private void UpdateFSM()
    {
        switch (mPlayerState)
        {
            case PlayerState.eIdle:
                ServiceIdleState();
                break;
            case PlayerState.eWait:
                ServiceWaitState();
                break;
            case PlayerState.eBuild:
                ServiceBuildState();
                break;
            case PlayerState.eHit:
                ServiceHitState();
                break;
        }
    }


    private void ServiceIdleState() {
        animator.SetBool("player1IsHolding", false);
        if(!isLocked) {
            mPlayerState = PlayerState.eWait;
        }
    }

    private void ServiceWaitState() {
        animator.SetBool("player1IsHolding", true);
        if (Input.GetKeyDown(KeyCode.B)) {
            mPlayerState = PlayerState.eBuild;
            Debug.Log("Build");
            return ;
        } 
        if (Input.GetKeyDown(KeyCode.H)) {
            mPlayerState = PlayerState.eHit;
            Debug.Log("Hit");
            return ;
        }
    }

    public void ServiceBuildState() {
        isBuild = true;
        isLocked = true;
        mPlayerState = PlayerState.eIdle;
    }

    public void ServiceHitState() {
        isBuild = false;

        if (Input.GetKeyDown(KeyCode.Q)) {
            mHitNum = 1;
            isLocked = true;

            mPlayerManager = GameManager.sTheGlobalBehavior.GetPlayerManager();
            mPlayerManager.getHitNum(mHitNum);
            Debug.Log($"Hit the number {mHitNum} block");

            mPlayerState = PlayerState.eIdle;
        }
        if(Input.GetKeyDown(KeyCode.W)) {
            mHitNum = 2;
            isLocked = true;

            mPlayerManager = GameManager.sTheGlobalBehavior.GetPlayerManager();
            mPlayerManager.getHitNum(mHitNum);
            Debug.Log($"Hit the number {mHitNum} block");

            mPlayerState = PlayerState.eIdle;
        }
        if(Input.GetKeyDown(KeyCode.E)) {
            mHitNum = 3;
            isLocked = true;

            mPlayerManager = GameManager.sTheGlobalBehavior.GetPlayerManager();
            mPlayerManager.getHitNum(mHitNum);
            Debug.Log($"Hit the number {mHitNum} block");

            mPlayerState = PlayerState.eIdle;
        }

    }
    void Update()
    {
        UpdateFSM();
    }

}
