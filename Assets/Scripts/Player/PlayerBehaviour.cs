using System.Collections;
using System.Collections.Generic;
using Unity.Play.Publisher.Editor;
using UnityEngine;
using UnityEngine.UI;

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


    private GameObject mPlayerUI = null;
    private Button mBuildButton = null;
    private Button mHitButton = null;
    private bool isBuildButtonClick = false;
    private bool isHitButtonClick = false;


    private Animator animator;


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

            //if (isBuildButtonClick)
            //{
            //    mPlayerState = PlayerState.eBuild;
            //    Debug.Log("Build");
            //    isBuildButtonClick = false;
            //    return;
            //} 
            //if (isHitButtonClick) {
            //    mPlayerState = PlayerState.eHit;
            //    Debug.Log("Hit");
            //    isHitButtonClick=false;
            //    return ;
            //}

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

            Debug.Log("Hit 1");

            mPlayerState = PlayerState.eIdle;
        }
        if(Input.GetKeyDown(KeyCode.W)) {
            mHitNum = 2;
            isLocked = true;

            Debug.Log("Hit 2");

            mPlayerState = PlayerState.eIdle;
        }
        if(Input.GetKeyDown(KeyCode.E)) {
            mHitNum = 3;
            isLocked = true;

            Debug.Log("Hit 3");

            mPlayerState = PlayerState.eIdle;
        }

    }
    void Update()
    {
        if(mPlayerUI == null && !isLocked)
        {
            BindUI();
            BindButton();
        }
        UpdateFSM();
    }

    private void BindUI()
    {
        if(gameObject.name == "Player1")
        {
            mPlayerUI = GameObject.Find("UIOfPlayer1");
        }
        if(gameObject.name == "Player2")
        {
            mPlayerUI = GameObject.Find("UIOfPlayer2");
        }
    }

    private void BindButton()
    {
        mBuildButton = mPlayerUI.transform.Find("Action").transform.Find("BuildButton").GetComponent<Button>();
        mBuildButton.onClick.AddListener(BuildButtonClick);
        mHitButton = mPlayerUI.transform.Find("Action").transform.Find("HitButton").GetComponent<Button>();
        mHitButton.onClick.AddListener(HitButtonClick);
    }

    private void BuildButtonClick()
    {
        isBuildButtonClick = true;
    }

    private void HitButtonClick()
    {
        isHitButtonClick = true;
    }
}
