using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class BlockListManager : MonoBehaviour
{   

    // UI
    private GameObject mEndCanvas = null;
    private GameObject mTextObj = null;
    public Text mEndText = null;
    public GameObject[] mSkillButtons = new GameObject[2];
    
    private enum BlockState {

        eIdle,
        eWait,
        eInitHit,
        eSelectHit,
        eHit,
        eBuild,
        eCombo,
        eEnd
    }

    private enum BlockColor {
        eRed,
        eGreen,
        eBlue
    };

    //For Skills
    private enum BlockSkills {
        eNormal,
        eSkills
    };

    private BlockState mBlockState = BlockState.eIdle;
    private BlockColor mBlockColor = BlockColor.eRed;
    private BlockSkills mBlockSkills = BlockSkills.eNormal;

    // Constants
    private const int kInitBlockIndex = 6;
    private const int kPlayerNum = 2;

    private int mTargetBlockIndex = 0;
    private int mPlayerIndex = 0;
    private bool mIsHitState = false;
    public float mHitSpeed = 2f;
    private float mTime = 0f;
    
    private BlockManager[] mBlockManagers = new BlockManager[2];

    public BlockManager mActiveManager;

    private Animator[] mPlayerAnimators = new Animator[2];

    private Animator mBlockAnimator = new Animator();

    private GameObject[] mPlayers = new GameObject[2];

    private GameObject[] mUIOfPlayers = new GameObject[2];

    // Audio
    private GameObject mAudioObj = null;
    private AudioSource mMusic = null;

    // Hit
    private GameObject mHitBlock;
    private GameObject mTargetBlock;
    private Vector3 mHitBlockPos;
    private Vector3 mTargetBlockPos;
    private CameraControll mCameraControll = null;

    // Start is called before the first frame update
    void Start()
    {
        // UI
        mCameraControll = FindObjectOfType<CameraControll>();
        mEndCanvas = GameObject.Find("EndCanvas");
        mTextObj = GameObject.Find("EndCanvas/Panel/EndText");
        mEndText = mTextObj.GetComponent<Text>();
        mEndCanvas.SetActive(false);

        for (int i = 0; i < kPlayerNum; i++) {
            mSkillButtons[i] = GameObject.Find("Canvas/UIOfPlayer" + (i + 1) + "/Action/SkillButton");
            mSkillButtons[i].SetActive(false);
        }
        
        for (int i = 0; i < kPlayerNum; i++) {
            mBlockManagers[i] = new BlockManager();
            mBlockManagers[i].SetInitPos(GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayerPos(i));
            mPlayers[i] = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer(i);
            Debug.Log("Here");
            if (mPlayers[i] == null) {
                Debug.Log("Player " + i + " is null");
            }
        }

        for (int i = 0; i < kInitBlockIndex; i++) {
            for (int j = 0; j < kPlayerNum; j++) {
                mBlockManagers[j].BuildOneBlock(j, false, -1);
            }
        }

        // Audio
        mAudioObj = GameObject.Find("AudioObject");
        mMusic = mAudioObj.GetComponent<AudioSource>();
    }

    private void UpdateFSM() {
        switch (mBlockState)
        {
            case BlockState.eIdle:
                StartCoroutine(ServiceIdleState());
                break;
            case BlockState.eWait:
                ServiceWaitState();
                break;
            case BlockState.eInitHit:
                ServiceInitHitState();
                break;
            case BlockState.eSelectHit:
                ServiceSelectHitState();
                break;
            case BlockState.eBuild:
                ServiceBuildState();
                break;
            case BlockState.eHit:
                ServiceHitState();
                break;
            case BlockState.eCombo:
                ServiceComboState();
                break;
            case BlockState.eEnd:
                ServiceEndState();
                break;
        }
    }

    private bool JudgeVictory() {
        for (int i = 0; i < kPlayerNum; i++) {
            if (mBlockManagers[i].GetHeight() == 0) {
                
                mEndCanvas.SetActive(true);
                mEndText.text = "Player " + (2 - mPlayerIndex) + " Win!";
                mBlockState = BlockState.eEnd;
                return true;
            }
        }
        return false;
    }


    private IEnumerator ServiceIdleState() {

        yield return new WaitForSeconds(0.3f);

        // Judge vectory in idle state
        if (JudgeVictory())
        {
            CameraEnd(mPlayers[1 - mPlayerIndex], mPlayers[mPlayerIndex]);
        }
        else
        {
            mBlockColor = (BlockColor)Random.Range(0, 3);
            float randomSkill = Random.Range(0f, 1f);

            for (int i = 0; i < kPlayerNum; i++)
            {
                mSkillButtons[i].SetActive(false);
            }

            if (randomSkill > 0.2f)
            {
                // TODO: Add UI
                mBlockSkills = BlockSkills.eNormal;
            }
            else
            {
                mSkillButtons[mPlayerIndex].SetActive(true);
                mBlockSkills = BlockSkills.eSkills;
            }

            //mCameraControll.ModifyTarget("Player1", 10f, 5f);
            //mCameraControll.ModifyTarget("Player2", 3f, 5f);
            //     // BlockColor : 0 - Green, 1 - Red, 2 - Blue
            //     p1Animator.SetInteger("BlockColor", (int)mBlockColor);
            // } else {
            //UIOfPlayer1.SetActive(false);
            //UIOfPlayer2.SetActive(true);
            //     p1Animator.SetBool("IsHolding", false);
            //     p2Animator.SetBool("IsHolding", true);
            //     p2Animator.SetInteger("BlockColor", (int)mBlockColor);

            //mCameraControll.ModifyTarget("Player2", 10f, 5f);
            //mCameraControll.ModifyTarget("Player1", 3f, 5f);
            for (int i = 0; i < kPlayerNum; i++)
            {
                mPlayerAnimators[i] = mPlayers[i].GetComponent<PlayerBehaviour>().animator;
                if (mUIOfPlayers[i] == null)
                {
                    mUIOfPlayers[i] = GameObject.Find("UIOfPlayer" + (i + 1));
                }
            }

            mUIOfPlayers[mPlayerIndex].SetActive(true);
            mPlayerAnimators[mPlayerIndex].SetBool("IsHolding", true);
            mPlayerAnimators[mPlayerIndex].SetInteger("BlockColor", (int)mBlockColor);
            mCameraControll.ModifyTarget("Player" + mPlayerIndex, 10f, 5f);
            for (int i = 0; i < kPlayerNum; i++)
            {
                if (i != mPlayerIndex)
                {
                    mUIOfPlayers[i].SetActive(false);
                    mPlayerAnimators[i].SetBool("IsHolding", false);
                    mCameraControll.ModifyTarget("Player" + i, 3f, 5f);
                }
            }

            mBlockState = BlockState.eWait;
        }
    }
    
    private void ServiceWaitState() {

        if (Input.GetKeyDown(KeyCode.B)) {
            mBlockState = BlockState.eBuild;
            return ;
        } 

        // Only if the player has blocks, can he be hit
        if (Input.GetKeyDown(KeyCode.H) && (((mPlayerIndex == 0) && mBlockManagers[1].GetHeight() > 0) || ((mPlayerIndex == 1) && mBlockManagers[0].GetHeight() > 0))) {
            mTargetBlockIndex = 1;

            // Initialize block blink
            mTargetBlock = mBlockManagers[1 - mPlayerIndex].GetBlockAt(mBlockManagers[1 - mPlayerIndex].GetHeight() - mTargetBlockIndex);
            mBlockAnimator = mTargetBlock.GetComponent<Animator>();
            mBlockAnimator.SetBool("IsSelected", true);

            mBlockState = BlockState.eSelectHit;
            return ;
        }


        //Use skills
        if (Input.GetKeyDown(KeyCode.P) && (mBlockSkills == BlockSkills.eSkills))
        {
            // Destroy the first Block of the enemy
            {
                mTargetBlockIndex = 1;
                mTargetBlock = mBlockManagers[0].GetBlockAt(mBlockManagers[0].GetHeight() - mTargetBlockIndex);
                mTargetBlockPos = mTargetBlock.transform.position;

                if (mPlayerIndex == 0)
                {
                    GameObject bullet = mBlockManagers[0].GetBlockAt(mBlockManagers[0].GetHeight() - 1);
                    mBlockManagers[1].BeingHitBlockDestroy(bullet, mBlockManagers[1].GetHeight() - mTargetBlockIndex);
                }
                else
                {
                    GameObject bullet = mBlockManagers[1].GetBlockAt(mBlockManagers[1].GetHeight() - 1);
                    mBlockManagers[0].BeingHitBlockDestroy(bullet, mBlockManagers[0].GetHeight() - mTargetBlockIndex);
                }
                mMusic.clip = Resources.Load<AudioClip>("music/Audio_Debuff");
                mMusic.Play();
            }
            mBlockSkills = BlockSkills.eNormal;
            return;
        }
    }

    private void ServiceInitHitState() {
        InitializeHit();
        mBlockState = BlockState.eHit;
    }

    private void ServiceSelectHitState() {
        // TODO: Up / Down choose
        if (Input.GetKeyDown(KeyCode.S) && mTargetBlockIndex < mBlockManagers[1 - mPlayerIndex].GetHeight()) {
            // Block blink effect
            mBlockAnimator.SetBool("IsSelected", false);

            mTargetBlockIndex += 1;

            mTargetBlock = mBlockManagers[1 - mPlayerIndex].GetBlockAt(mBlockManagers[1 - mPlayerIndex].GetHeight() - mTargetBlockIndex);
            mBlockAnimator = mTargetBlock.GetComponent<Animator>();
            mBlockAnimator.SetBool("IsSelected", true);

            Debug.Log(mBlockManagers[1 - mPlayerIndex].GetHeight() - mTargetBlockIndex);
            return ;
        }

        if (Input.GetKeyDown(KeyCode.W) && mTargetBlockIndex > 1) {
            mBlockAnimator.SetBool("IsSelected", false);

            mTargetBlockIndex -= 1;

            mTargetBlock = mBlockManagers[1 - mPlayerIndex].GetBlockAt(mBlockManagers[1 - mPlayerIndex].GetHeight() - mTargetBlockIndex);
            mBlockAnimator = mTargetBlock.GetComponent<Animator>();
            mBlockAnimator.SetBool("IsSelected", true);
            
            Debug.Log(mBlockManagers[1 - mPlayerIndex].GetHeight() - mTargetBlockIndex);
            return ;
        }

        if (Input.GetKeyDown(KeyCode.H)) {
            mIsHitState = true;
            mBlockState = BlockState.eBuild;
            return ;
        }

        // if (Input.GetKeyDown(KeyCode.Q) && (((mPlayerIndex == 0) && mBlockManagers[1].GetHeight() >= 1) || ((mPlayerIndex == 1) && mBlockManagers[0].GetHeight() >= 1))) {
        //     mTargetBlockIndex = 1;

        //     mIsHitState = true;
        //     mBlockState = BlockState.eBuild;
        // }
        // if (Input.GetKeyDown(KeyCode.W) && (((mPlayerIndex == 0) && mBlockManagers[1].GetHeight() >= 2) || ((mPlayerIndex == 1) && mBlockManagers[0].GetHeight() >= 2))) {
        //     mTargetBlockIndex = 2;

        //     mIsHitState = true;
        //     mBlockState = BlockState.eBuild;
        // }
        // if (Input.GetKeyDown(KeyCode.E) && (((mPlayerIndex == 0) && mBlockManagers[1].GetHeight() >= 3) || ((mPlayerIndex == 1) && mBlockManagers[0].GetHeight() >= 3))) {
        //     mTargetBlockIndex = 3;
            
        //     mIsHitState = true;
        //     mBlockState = BlockState.eBuild;
        // }

        // You can retract the selection
        if (Input.GetKeyDown(KeyCode.B)) {
            mBlockAnimator.SetBool("IsSelected", false);
            mBlockState = BlockState.eBuild;
        }
    }

    public void InitializeHit() {

        // TODO: Think about what if mPlayerIndex == 2 ?
        if (mPlayerIndex == 0) {
            mHitBlock = mBlockManagers[0].GetBlockAt(mBlockManagers[0].GetHeight() - 1);
            mTargetBlock = mBlockManagers[1].GetBlockAt(mBlockManagers[1].GetHeight() - mTargetBlockIndex);
        } else {
            mHitBlock = mBlockManagers[1].GetBlockAt(mBlockManagers[1].GetHeight() - 1);
            mTargetBlock = mBlockManagers[0].GetBlockAt(mBlockManagers[0].GetHeight() - mTargetBlockIndex);
        }
        mHitBlockPos = mHitBlock.transform.position;
        mTargetBlockPos = mTargetBlock.transform.position;
    }

    public void ServiceHitState() {

        mTime += mHitSpeed * Time.smoothDeltaTime;

        float x = Mathf.LerpUnclamped(mHitBlockPos.x, mTargetBlockPos.x, mTime);
        float y = Mathf.LerpUnclamped(mHitBlockPos.y, mTargetBlockPos.y, mTime);
        mHitBlock.transform.position = new Vector3(x, y, 0);
        
        BlockBehaviour HitBlockScript = mHitBlock.GetComponent<BlockBehaviour>();
        HitBlockScript.targetCollisionObject = mTargetBlock;
        bool isDestroy = HitBlockScript.isColli();
        if (isDestroy) {
            mMusic.clip = Resources.Load<AudioClip>("music/Audio_Hit");
            mMusic.Play();
            if (mPlayerIndex == 0) {
                GameObject bullet = mBlockManagers[0].GetBlockAt(mBlockManagers[0].GetHeight() - 1);
                mBlockManagers[1].BeingHitBlockDestroy(bullet, mBlockManagers[1].GetHeight() - mTargetBlockIndex);//player 2被击打的玩家
                mBlockManagers[0].DestroyOneBlock(mBlockManagers[0].GetHeight() - 1);//player 1: 当前的玩家
                mCameraControll.CameraFocusOnPlayer(mPlayers[1]);
                
            } else {
                GameObject bullet = mBlockManagers[1].GetBlockAt(mBlockManagers[1].GetHeight() - 1);
                mBlockManagers[0].BeingHitBlockDestroy(bullet,mBlockManagers[0].GetHeight() - mTargetBlockIndex);//player 1
                mBlockManagers[1].DestroyOneBlock(mBlockManagers[1].GetHeight() - 1);//player 2: 当前的玩家
                mCameraControll.CameraFocusOnPlayer(mPlayers[0]);
            }
            
            mBlockState = BlockState.eCombo;
            mIsHitState = false;
            mTime = 0;
        }
    }
    #region CameraEffect
    public void CameraEffect(GameObject player)
    {
        
        if (mCameraControll != null)
        {
            mCameraControll.CameraFocusOnPlayer(player);
            //StartCoroutine(DelayToShake(mShakeDelay));
            //StartCoroutine(DelayToFocusBack(mCameraBackDelay,player));
        }
        else
        {
            Debug.Log("No CameraControll Found");
        }
    }

    public void CameraEnd(GameObject playerWin, GameObject playerLose)
    {
        Debug.Log("CameraEnd!");
        if(mCameraControll != null)
        {
            Debug.Log($"focus on {playerWin.name}");
            mCameraControll.ModifyTarget(playerLose.name, 3f, 5f);
            mCameraControll.ModifyTarget(playerWin.name, 40f, 0.1f);
        }
    }
    //private IEnumerator DelayToFocusBack(float delayTime, GameObject player)
    //{
    //    yield return new WaitForSeconds(delayTime);
    //    FocusBack(player);
    //}

    //private IEnumerator DelayToShake(float delayTime)
    //{
    //    yield return new WaitForSeconds(delayTime);
    //    mCameraControll.CameraShake();
    //}
    //private void FocusBack(GameObject player)
    //{
    //    mCameraControll.CameraUnfocusOnPlayer(player);
    //}
    #endregion CameraEffect

    public void ServiceComboState()
    {
        Debug.Log("Combo in block list");
        // TODO: Think about what if mPlayerIndex == 2 ?
        if (mPlayerIndex == 0)
        {
            mActiveManager = mBlockManagers[1];
        }
        else
        {
            mActiveManager = mBlockManagers[0];
        }

        if (mActiveManager.UpdateComboState())
        {
            Debug.Log("Combo done");
            mBlockState = BlockState.eIdle;
            mPlayerIndex = (mPlayerIndex + 1) % 2;
            mIsHitState = false;
            mTime = 0;
        }
    }

    public void ServiceBuildState() {

        mBlockManagers[mPlayerIndex].BuildOneBlock(mPlayerIndex, mIsHitState, (int)mBlockColor);
        mPlayerAnimators[mPlayerIndex].SetBool("IsHolding", false);
        if (mIsHitState) {
            mBlockState = BlockState.eInitHit;
            return ;
        }
        mMusic.clip = Resources.Load<AudioClip>("music/Audio_Build");
        mMusic.Play();
        mBlockState = BlockState.eIdle;
        mPlayerIndex = (mPlayerIndex + 1) % 2;
    }

    public void ServiceEndState() {
        Debug.Log("End!!");
    }

    void Update()
    {
        UpdateFSM();
    }
}
