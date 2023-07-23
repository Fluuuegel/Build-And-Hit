using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;
//use this to unify the color system
using BlockColor = BlockBehaviour.BlockColourType;
public partial class BlockListManager : MonoBehaviour
{

    // UI
    private GameObject mEndCanvas = null;

    public GameObject[] mSkillCDSlider = new GameObject[2];

    public GameObject[] mLastStandUI = new GameObject[2];

    public GameObject[] mSkillButtons = new GameObject[2];

    public GameObject[] mBuildButtons = new GameObject[2];

    public GameObject[] mGainedSkillButtons = new GameObject[2];

    public GameObject[] mHitButtons = new GameObject[2];

    public GameObject[] mRefreshButtons = new GameObject[2];

    public GameObject[] mWinImages = new GameObject[2];

    public GameObject mBlockHeight = null;

    public GameObject mGettingSkillHint = null;
    private TextMeshProUGUI GainedSkillHintText;

    public enum BlockState {

        eIdle,
        eSkill,
        eWait,
        eInitHit,
        eSelectHit,
        eHit,
        eBuild,
        eCombo,
        eEnd,
        eSelectSuck,
        eInitSuck,
        eSuck,
        eInvalid
    }

    private BlockState mBlockState = BlockState.eIdle;
    private BlockColor mBlockColor = BlockColor.eRed;

    // Constants
    private const int kInitBlockIndex = 15;
    private const int kPlayerNum = 2;

    private int maxTurn = 21;
    private int mTurnCnt = 21;
    private int mTargetBlockIndex = 0;
    private int mPlayerIndex = 0;
    private bool mIsHitState = false;

    private bool[] mKirbyIsHungry = new bool[2] {true, true};

    private bool hasGainedSkill = false;
    private bool hasCharacterSkill = false;
    public float mHitSpeed = 2f;

    public float mSuckSpeed = 1.5f;
    private float mTime = 0f;
    
    
    private BlockManager[] mBlockManagers = new BlockManager[2];

    public BlockManager mActiveManager;

    private Animator[] mPlayerAnimators = new Animator[2];

    private Animator mBlockAnimator = new Animator();

    private GameObject[] mPlayers = new GameObject[2];

    private GameObject[] mUIOfPlayers = new GameObject[2];

    private GameObject[] mPopUp = new GameObject[2];

    // Audio
    private GameObject mAudioObj = null;
    private AudioSource mMusic = null;
    private GameObject mBGMObj = null;
    private AudioSource mBGM = null;

    // Hit
    private GameObject mHitBlock;
    private GameObject mTargetBlock;
    private Vector3 mHitBlockPos;
    private Vector3 mTargetBlockPos;
    private CameraControll mCameraControll = null;
    CinemachineTargetGroup.Target[] targets = null;

    //for hit cool down
    private int[] mHitCoolDown = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
    const int kHitCoolDown = 0;
    private Player.Player curPlayer;
    private PlayerManager mPlayerManager;

    //for Getting Skills
    private int GainedSkillIndex = 0;
    private float GettingProb = 0f;

    //for user control
    KeyCode mHitKeyCode, mBuildKeyCode, mSkill1KeyCode, mSkill2KeyCode,mUpBlockKey, mDownBlockKey, mRefreshKey;
    
    //for refresh the initial tower
    private bool[] canRefresh = {true, true};
    
    void Start()
    {
        //developer key
        BindDeveloperKey();
        // UI
        mBlockHeight = GameObject.Find("Canvas/BlockHeight");
        mCameraControll = FindObjectOfType<CameraControll>();
        mEndCanvas = GameObject.Find("EndCanvas");
        mEndCanvas.SetActive(false);

        for (int i = 0; i < kPlayerNum; i++) {

            mBuildButtons[i] = GameObject.Find("Canvas/UIOfPlayer" + (i + 1) + "/Action/BuildButton");
            mHitButtons[i] = GameObject.Find("Canvas/UIOfPlayer" + (i + 1) + "/Action/HitButton");
            mSkillButtons[i] = GameObject.Find("Canvas/UIOfPlayer" + (i + 1) + "/Action/SkillButton");
            mGainedSkillButtons[i] = GameObject.Find("Canvas/UIOfPlayer" + (i + 1) + "/Action/GainedSkillButton");
            mSkillCDSlider[i] = GameObject.Find("Canvas/UIOfPlayer" + (i + 1) + "/Action/SkillButton/CDBackground");
            mLastStandUI[i] = GameObject.Find("Canvas/UIOfPlayer" + (i + 1) + "/LastStandUI");
            mRefreshButtons[i] = GameObject.Find("Canvas/UIOfPlayer" + (i + 1) + "/Action/RefreshButton");
            mRefreshButtons[i].SetActive(false);
            mLastStandUI[i].SetActive(false);
            mSkillButtons[i].SetActive(false);
            mGainedSkillButtons[i].SetActive(false);
        }
        
        mPlayerManager = GameManager.sTheGlobalBehavior.GetPlayerManager();
        for (int i = 0; i < kPlayerNum; i++) {
            mBlockManagers[i] = new BlockManager();
            mBlockManagers[i].SetInitPos(GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayerPos(i));
            mPlayers[i] = mPlayerManager.getPlayer(i);
            if (mPlayers[i] == null) {
                Debug.Log("Player " + i + " is null");
            }

            mPlayerAnimators[i] = mPlayers[i].GetComponent<PlayerBehaviour>().animator;
            mPlayers[i].GetComponent<PlayerBehaviour>().GetPlayer().GetAnimator(mPlayerAnimators[i]);
            mPlayers[i].GetComponent<PlayerBehaviour>().GetPlayer().GetPlayerIndex(i);

            if (mUIOfPlayers[i] == null)
            {
                mUIOfPlayers[i] = GameObject.Find("UIOfPlayer" + (i + 1));
            }
        }
        for (int j = 0; j < kPlayerNum; j++) {
            for (int i = 0; i < kInitBlockIndex; i++) {
                if (j == 1) {
                    if (i == kInitBlockIndex - 1) {
                        mBlockManagers[j].BuildOneBlock(j, false, 0, true);
                        continue;
                    } else if (i == kInitBlockIndex - 2) {
                        mBlockManagers[j].BuildOneBlock(j, false, 1, true);
                        continue;
                    } else if (i == kInitBlockIndex - 3) {
                        mBlockManagers[j].BuildOneBlock(j, false, 2, true);
                        continue;
                    }
                }
                mBlockManagers[j].BuildOneBlock(j, false, (int)GenRandomColour(), true);
            }
            ResetRandom();
        }

        mGettingSkillHint = GameObject.Find("Canvas/GainedSkillsHint");
        GainedSkillHintText = mGettingSkillHint.GetComponent<TextMeshProUGUI>();

        // Audio
        mAudioObj = GameObject.Find("AudioObject");
        mMusic = mAudioObj.GetComponent<AudioSource>();
        mBGMObj = GameObject.Find("BattleBGM");
        mBGM = mBGMObj.GetComponent<AudioSource>();
    }
    void Update()
    {
        UpdateFSM();
    }
    private void UpdateFSM() {
        //Debug.Log(mBlockState);
        switch (mBlockState)
        {
            case BlockState.eIdle:
                ServiceIdleState();
                break;
            case BlockState.eSkill:
                ServiceSkillState();
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

            case BlockState.eInitSuck:
                ServiceInitSuckState();
                break;
            case BlockState.eSelectSuck:
                ServiceSelectSuckState();
                break;
            case BlockState.eSuck:
                ServiceSuckState();
                break;
        }
    }
    private void ServiceIdleState() {
        StartAITimer();
        GainedSkillHintText.text = null;
        mTurnCnt--;
        DisplayCountDown();
        int winnerIdnex = mPlayerIndex;
        if (JudgeVictory(mTurnCnt,ref winnerIdnex))
        {
            RefreshBlockHeight();
            CameraEnd(mPlayers[winnerIdnex], mPlayers[1 - winnerIdnex]);
            mTurnCnt = 0;
        }
        else
        {
            curPlayer = mPlayers[mPlayerIndex].GetComponent<PlayerBehaviour>().GetPlayer();
            RoundRefresh();
            curPlayer.IncreaseTimeUntilNextSkill();
            ModifyCDUI();
            
            curPlayer.GetPlayerPosition(mPlayerManager.getPlayerPos(mPlayerIndex)); // Not used yet
            for (int i = 0; i < kPlayerNum; i++)
            {
                if (mPopUp[i] != null)
                {
                    if (i == 0) {
                        Vector3 pos = mPlayerManager.getPlayerPos(i);
                        mPopUp[i].transform.position = new Vector3(pos.x - 2.5f, pos.y + 0.5f, 0);
                    } else {
                        Vector3 pos = mPlayerManager.getPlayerPos(i);
                        mPopUp[i].transform.position = new Vector3(pos.x + 2.5f, pos.y + 0.5f, 0);
                    }
                }
            }

            if (mBlockManagers[mPlayerIndex].LastStand())
            {
                DisplayLastStandUI();
            }
            UpdateKeyBinding();
            mBlockColor = (BlockColor)Random.Range(0, 3);
            mUIOfPlayers[mPlayerIndex].SetActive(true);
            mPlayerAnimators[mPlayerIndex].SetBool("IsHolding", true);
            mPlayerAnimators[mPlayerIndex].SetInteger("BlockColor", (int)mBlockColor);

            targets = PlayerManager.mTargetGroup.m_Targets;
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].weight = 1f;
                targets[i].radius = 3f;
            }
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

            mBlockState = BlockState.eSkill;
            RefreshBlockHeight();
        }
    }
    private void ServiceSkillState() {
        
        for (int i = 0; i < kPlayerNum; i++) {
            mSkillButtons[i].SetActive(false);
            mGainedSkillButtons[i].SetActive(false);
        }

        // Unique Skill
        float rand = Random.Range(0f, 1f);
        if (rand > 1.0f) {
            hasCharacterSkill = false;
        }
        else {
            mSkillButtons[mPlayerIndex].SetActive(true);
            hasCharacterSkill = true;
        }

        // Gained Skill
        float rand2 = Random.Range(0f, 1f);
        BalanceProb();
        if (rand2 > GettingProb)
        {
            hasGainedSkill = false;
        }
        else
        {
            hasGainedSkill = true;
            mGainedSkillButtons[mPlayerIndex].SetActive(true);
            rand = Random.Range(0f, 1f);
            if(rand < 0.5f)
            {
                GainedSkillIndex = 1;
            }
            else if(rand < 0.7f)
            {
                GainedSkillIndex = 2;
            }
            else
            {
                GainedSkillIndex = 3;
            }
            /*if (mGettingSkillHint != null)
            {*/
                GainedSkillHintText.text = SkillDes[GainedSkillIndex - 1];
            /*}
            else
            {
                Debug.Log("Cannot find Text Object!");
            }*/
            //Clean the text after build, hit, using role&getting skills(Finished)
        }

        mBlockState = BlockState.eWait;
    }
    private void ServiceWaitState()
    {
        TriggerRefresh();
        DeveloperModeUpdate();
        
        if (Input.GetKeyDown(mBuildKeyCode) || AITriggerBuild()) {
            Debug.Log("Wait state to Build state");
            mBlockState = BlockState.eBuild;
            return ;
        } 

        // Only if the player has blocks, can he be hit
        /*if ((Input.GetKeyDown(mHitKeyCode))) /*&& 
             (
                 (mPlayerIndex == 0 && mBlockManagers[1].GetHeight() > 0)) || (mPlayerIndex == 1 && mBlockManagers[0].GetHeight() > 0))
            )#1#*/
        if(Input.GetKeyDown(mHitKeyCode) || AITriggerBuild())
        {
            
            if (mHitCoolDown[mPlayerIndex] <= 0)
            {
                
                mTargetBlockIndex = 1;
                // Initialize block blink
                mTargetBlock = mBlockManagers[1 - mPlayerIndex]
                    .GetBlockAt(mBlockManagers[1 - mPlayerIndex].GetHeight() - mTargetBlockIndex);
                mCameraControll.ModifyTarget(mTargetBlock, 20f, 7f);
                mBlockAnimator = mTargetBlock.GetComponent<Animator>();
                mBlockAnimator.SetBool("IsSelected", true);
                //change status to hit
                Debug.Log("Wait state to Hit state");
                mBlockState = BlockState.eSelectHit;
                return;
            }
            else
            {
                Debug.Log("Hit cool down..");
            }
        }


        //Use Getting skills
        if (Input.GetKeyDown(mSkill2KeyCode) && (hasGainedSkill == true))
        {
            TriggerGainedSkill();
        }
        

        //Use Role Skills
        Player.Player curPlayer = mPlayers[mPlayerIndex].GetComponent<PlayerBehaviour>().GetPlayer();
        if (curPlayer.CanCastSkill()) {
            if (TriggerSkill()) {
                Player.PlayerType type = curPlayer.GetPlayerType();
                if (type == Player.PlayerType.eEngineer) {
                    mBlockState = BlockState.eBuild;
                }
                else if (type == Player.PlayerType.eSlime) {
                    mBlockState = BlockState.eIdle;
                    mPlayerIndex = (mPlayerIndex + 1) % 2;
                } else if (type == Player.PlayerType.eAPinkBall) {
                    if (mKirbyIsHungry[mPlayerIndex]) {
                        mTargetBlockIndex = 1;
                        // Initialize block blink
                        mTargetBlock = mBlockManagers[1 - mPlayerIndex]
                            .GetBlockAt(mBlockManagers[1 - mPlayerIndex].GetHeight() - mTargetBlockIndex);
                        mCameraControll.ModifyTarget(mTargetBlock, 20f, 7f);
                        mBlockAnimator = mTargetBlock.GetComponent<Animator>();
                        mBlockAnimator.SetBool("IsSelected", true);

                        mBlockState = BlockState.eSelectSuck;
                    } else {
                        mKirbyIsHungry[mPlayerIndex] = true;
                        GameObject.Destroy(mPopUp[mPlayerIndex]);
                    }
                }
            }
        }
    }
    private void ServiceInitHitState() {
        InitializeHit();
        mBlockState = BlockState.eHit;
        
    }
    private void ServiceSelectHitState() {
        TriggerRefresh(false);
        PlayerBehaviour script = mPlayers[mPlayerIndex].GetComponent<PlayerBehaviour>();
        int VisionZone = script.VisionRange();
        if (Input.GetKeyDown(mDownBlockKey) && mTargetBlockIndex < mBlockManagers[1 - mPlayerIndex].GetHeight()) {
            // Block blink effect
            mBlockAnimator.SetBool("IsSelected", false);

            mTargetBlockIndex += 1;
            if(mTargetBlockIndex > VisionZone)
            {
                mTargetBlockIndex = VisionZone;
            }
            mCameraControll.ModifyTarget(mTargetBlock, 1f, 3f);
            mTargetBlock = mBlockManagers[1 - mPlayerIndex].GetBlockAt(mBlockManagers[1 - mPlayerIndex].GetHeight() - mTargetBlockIndex); 
            mBlockAnimator = mTargetBlock.GetComponent<Animator>();
            mBlockAnimator.SetBool("IsSelected", true);

            mCameraControll.ModifyTarget(mTargetBlock, 20f, 7f);
//            Debug.Log(mBlockManagers[1 - mPlayerIndex].GetHeight() - mTargetBlockIndex);
            return ;
        }

        if (Input.GetKeyDown(mUpBlockKey) && mTargetBlockIndex > 1) {
            mBlockAnimator.SetBool("IsSelected", false);

            mTargetBlockIndex -= 1;

            mCameraControll.ModifyTarget(mTargetBlock, 1f, 3f);
            mTargetBlock = mBlockManagers[1 - mPlayerIndex].GetBlockAt(mBlockManagers[1 - mPlayerIndex].GetHeight() - mTargetBlockIndex);
            mBlockAnimator = mTargetBlock.GetComponent<Animator>();
            mBlockAnimator.SetBool("IsSelected", true);

            mCameraControll.ModifyTarget(mTargetBlock, 20f, 7f);
//            Debug.Log(mBlockManagers[1 - mPlayerIndex].GetHeight() - mTargetBlockIndex);
            return ;
        }
        
        if (Input.GetKeyDown(mHitKeyCode))
        {
            mCameraControll.CameraFocusOnBlock(mTargetBlock);
            mIsHitState = true;
            mBlockState = BlockState.eBuild;
            return ;
        }

        // You can retract the selection
        if (Input.GetKeyDown(mBuildKeyCode) ) {
            mBlockAnimator.SetBool("IsSelected", false);
            mBlockState = BlockState.eBuild;
        }
        
        if (AITriggerHit())
        {
            mBlockAnimator.SetBool("IsSelected", false);
            int AITarget = AIAttackTarget();
            mTargetBlockIndex =  AITarget;
            //mTargetBlock = mBlockManagers[0].GetBlockAt(mTargetBlockIndex);
            Debug.Log("In select state AI target block: " + mTargetBlockIndex);
            mCameraControll.CameraFocusOnBlock(mTargetBlock);
            mIsHitState = true;
            mBlockState = BlockState.eBuild;
            return ;
        }
        
        

        //Use Gained Skills
        if (Input.GetKeyDown(mSkill2KeyCode) && (hasGainedSkill == true))
        {   
            mBlockAnimator.SetBool("IsSelected", false);
            TriggerGainedSkill();
            mSkillButtons[mPlayerIndex].SetActive(false);
            mBlockState = BlockState.eWait;
        }

        if (curPlayer.CanCastSkill()) {
            if (TriggerSkill()) {
                mBlockAnimator.SetBool("IsSelected", false);
                Player.PlayerType type = curPlayer.GetPlayerType();
                if (type == Player.PlayerType.eEngineer) {
                    mBlockState = BlockState.eBuild;
                }
                else if (type == Player.PlayerType.eSlime) {
                    mBlockState = BlockState.eIdle;
                    mPlayerIndex = (mPlayerIndex + 1) % 2;

                    // TODO: Fix this
                } else if (type == Player.PlayerType.eAPinkBall) {
                    if (mKirbyIsHungry[mPlayerIndex]) {
                        mTargetBlockIndex = 1;
                        // Initialize block blink
                        mTargetBlock = mBlockManagers[1 - mPlayerIndex]
                            .GetBlockAt(mBlockManagers[1 - mPlayerIndex].GetHeight() - mTargetBlockIndex);
                        mCameraControll.ModifyTarget(mTargetBlock, 20f, 7f);
                        mBlockAnimator = mTargetBlock.GetComponent<Animator>();
                        mBlockAnimator.SetBool("IsSelected", true);

                        mBlockState = BlockState.eSelectSuck;
                    } else {
                        mKirbyIsHungry[mPlayerIndex] = true;
                        GameObject.Destroy(mPopUp[mPlayerIndex]);
                        mBlockState = BlockState.eWait;
                    }
                }
            }
        }
    }

    private void ServiceSelectSuckState() {
        TriggerRefresh(false);
        PlayerBehaviour script = mPlayers[mPlayerIndex].GetComponent<PlayerBehaviour>();
        int VisionZone = script.VisionRange();
        if (Input.GetKeyDown(mDownBlockKey) && mTargetBlockIndex < mBlockManagers[1 - mPlayerIndex].GetHeight()) {
            // Block blink effect
            mBlockAnimator.SetBool("IsSelected", false);

            mTargetBlockIndex += 1;
            if(mTargetBlockIndex > VisionZone)
            {
                mTargetBlockIndex = VisionZone;
            }
            mCameraControll.ModifyTarget(mTargetBlock, 1f, 3f);
            mTargetBlock = mBlockManagers[1 - mPlayerIndex].GetBlockAt(mBlockManagers[1 - mPlayerIndex].GetHeight() - mTargetBlockIndex); 
            mBlockAnimator = mTargetBlock.GetComponent<Animator>();
            mBlockAnimator.SetBool("IsSelected", true);

            mCameraControll.ModifyTarget(mTargetBlock, 20f, 7f);
            Debug.Log(mBlockManagers[1 - mPlayerIndex].GetHeight() - mTargetBlockIndex);
            return ;
        }

        if (Input.GetKeyDown(mUpBlockKey) && mTargetBlockIndex > 1) {
            mBlockAnimator.SetBool("IsSelected", false);

            mTargetBlockIndex -= 1;

            mCameraControll.ModifyTarget(mTargetBlock, 1f, 3f);
            mTargetBlock = mBlockManagers[1 - mPlayerIndex].GetBlockAt(mBlockManagers[1 - mPlayerIndex].GetHeight() - mTargetBlockIndex);
            mBlockAnimator = mTargetBlock.GetComponent<Animator>();
            mBlockAnimator.SetBool("IsSelected", true);

            mCameraControll.ModifyTarget(mTargetBlock, 20f, 7f);
            Debug.Log(mBlockManagers[1 - mPlayerIndex].GetHeight() - mTargetBlockIndex);
            return ;
        }

        
        // You can retract the selection
        if (Input.GetKeyDown(mBuildKeyCode)) {
            curPlayer.Recover();
            mBlockAnimator.SetBool("IsSelected", false);
            mPlayerAnimators[mPlayerIndex].SetBool("Suck", false);
            mBlockState = BlockState.eBuild;
        }
        

        //Use Gained Skills
        if (Input.GetKeyDown(mSkill2KeyCode) && (hasGainedSkill == true))
        {   
            curPlayer.Recover();
            mBlockAnimator.SetBool("IsSelected", false);
            mPlayerAnimators[mPlayerIndex].SetBool("Suck", false);
            TriggerGainedSkill();
            mSkillButtons[mPlayerIndex].SetActive(false);
            mBlockState = BlockState.eWait;
        }

        if (Input.GetKeyDown(mSkill1KeyCode)) {
            mBlockAnimator.SetBool("IsSelected", false);
            // TODO: Suck opponent's block
            mBlockState = BlockState.eInitSuck;
        }
        if (AIUseSkill())
        {
            mBlockAnimator.SetBool("IsSelected", false);
            int AITarget = AIAttackTarget();
            mTargetBlockIndex =  AITarget;
            //mTargetBlock = mBlockManagers[0].GetBlockAt(mTargetBlockIndex);
            Debug.Log("In select state AI target block: " + mTargetBlockIndex);
            mCameraControll.CameraFocusOnBlock(mTargetBlock);
            mIsHitState = true;
            mBlockState = BlockState.eBuild;
            return ;
        }
    }

    private void ServiceInitSuckState() {
        InitializeHit();
        mBlockState = BlockState.eSuck;
    }
    private void ServiceSuckState() {

        BlockColor colour = mTargetBlock.GetComponent<BlockBehaviour>().GetBlockColour();
        Vector3 pos = mTargetBlock.transform.position;
        curPlayer.SetColor(colour);

        mPlayerAnimators[mPlayerIndex].SetBool("Suck", true);

        mTime += mSuckSpeed * Time.smoothDeltaTime;

        float x = Mathf.LerpUnclamped(mTargetBlockPos.x, mPlayerManager.getPlayerPos(mPlayerIndex).x, mTime);
        float y = Mathf.LerpUnclamped(mTargetBlockPos.y, mPlayerManager.getPlayerPos(mPlayerIndex).y + 0.3f, mTime);
        mTargetBlock.transform.position = new Vector3(x, y, 0);
        
        BlockBehaviour TargetBlockScript = mTargetBlock.GetComponent<BlockBehaviour>();
        TargetBlockScript.targetCollisionObject = mPlayers[mPlayerIndex];
        bool isDestroy = TargetBlockScript.isColli();
        if (isDestroy) {
            if (mPlayerIndex == 0) {
                mBlockManagers[1].BeingHitBlockDestroy(null, mBlockManagers[1].GetHeight() - mTargetBlockIndex, true);
            } else {
                mBlockManagers[0].BeingHitBlockDestroy(null, mBlockManagers[0].GetHeight() - mTargetBlockIndex, true);
            }
            // mMusic.clip = Resources.Load<AudioClip>("music/Audio_Hit");
            // mMusic.volume = 3.0f;
            // mMusic.Play();
            Debug.Log("Suck");
            mCameraControll.CameraFocusOnBlock(mTargetBlock);
            mBlockState = BlockState.eCombo;
            mKirbyIsHungry[mPlayerIndex] = false;
            mTime = 0;

            // Pop up
            if (mPlayerIndex == 0) {
                        switch (colour)
                        {
                            case BlockBehaviour.BlockColourType.eRed:
                                mPopUp[mPlayerIndex] = GameObject.Instantiate(Resources.Load("Prefabs/EatRedL1")) as GameObject;
                                break;
                            case BlockBehaviour.BlockColourType.eGreen:
                                mPopUp[mPlayerIndex] = GameObject.Instantiate(Resources.Load("Prefabs/EatGreenL1")) as GameObject;
                                break;
                            case BlockBehaviour.BlockColourType.eBlue:
                                mPopUp[mPlayerIndex] = GameObject.Instantiate(Resources.Load("Prefabs/EatBlueL1")) as GameObject;
                                break;
                            case BlockBehaviour.BlockColourType.eSlime:
                                mPopUp[mPlayerIndex] = GameObject.Instantiate(Resources.Load("Prefabs/EatRedL1")) as GameObject;
                                break;
                        }
                        mPopUp[mPlayerIndex].transform.position = new Vector3(pos.x - 2.5f, pos.y + 0.5f, 0);
                    } else {
                        switch (colour)
                        {
                            case BlockBehaviour.BlockColourType.eRed:
                                mPopUp[mPlayerIndex] = GameObject.Instantiate(Resources.Load("Prefabs/EatRedR1")) as GameObject;
                                break;
                            case BlockBehaviour.BlockColourType.eGreen:
                                mPopUp[mPlayerIndex] = GameObject.Instantiate(Resources.Load("Prefabs/EatGreenR1")) as GameObject;
                                break;
                            case BlockBehaviour.BlockColourType.eBlue:
                                mPopUp[mPlayerIndex] = GameObject.Instantiate(Resources.Load("Prefabs/EatBlueR1")) as GameObject;
                                break;
                            case BlockBehaviour.BlockColourType.eSlime:
                                mPopUp[mPlayerIndex] = GameObject.Instantiate(Resources.Load("Prefabs/EatRedR1")) as GameObject;
                                break;
                        }
                        mPopUp[mPlayerIndex].transform.position = new Vector3(pos.x + 2.5f, pos.y + 0.5f, 0);
                    }
        }
    }

    public void InitializeHit() {

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
            mMusic.volume = 3.0f;
            mMusic.Play();
            Debug.Log("Hit");
            if (mPlayerIndex == 0) {
                GameObject bullet = mBlockManagers[0].GetBlockAt(mBlockManagers[0].GetHeight() - 1);
                mBlockManagers[1].BeingHitBlockDestroy(bullet, mBlockManagers[1].GetHeight() - mTargetBlockIndex);
                mBlockManagers[0].DestroyOneBlock(mBlockManagers[0].GetHeight() - 1); // P1: Cur player

                mCameraControll.CameraFocusOnBlock(mTargetBlock);

                
            } else {
                GameObject bullet = mBlockManagers[1].GetBlockAt(mBlockManagers[1].GetHeight() - 1);
                mBlockManagers[0].BeingHitBlockDestroy(bullet,mBlockManagers[0].GetHeight() - mTargetBlockIndex);
                mBlockManagers[1].DestroyOneBlock(mBlockManagers[1].GetHeight() - 1); // P2: Cur player
                mCameraControll.CameraFocusOnBlock(mTargetBlock);
            }
            
            mBlockState = BlockState.eCombo;
            mIsHitState = false;
            mTime = 0;
        }
    }

    public void ServiceComboState()
    {
//        Debug.Log("Combo");
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
//            Debug.Log("Combo done");
            mHitCoolDown[mPlayerIndex] = kHitCoolDown;
            mBlockState = BlockState.eIdle;
            
            mPlayerIndex = (mPlayerIndex + 1) % 2;
            mIsHitState = false;
            mTime = 0;
        }
    }
    public void ServiceBuildState(bool noSkillCast = true, bool buildSlimeBlock = false, int color = -1) {
        Debug.Log("Build");
        TriggerRefresh();
        if (noSkillCast)
        {
            string msg = "Build block color: " + mBlockColor;
            Debug.Log(msg);
            mBlockManagers[mPlayerIndex].BuildOneBlock(mPlayerIndex, mIsHitState, (int)mBlockColor);
            mPlayerAnimators[mPlayerIndex].SetBool("IsHolding", false);
            if (mIsHitState)
            {
                mBlockState = BlockState.eInitHit;
                return;
            }

            mMusic.clip = Resources.Load<AudioClip>("music/Audio_Build");
            mMusic.Play();
            mBlockState = BlockState.eIdle;
            
            mPlayerIndex = (mPlayerIndex + 1) % 2;
        }
        else
        {   if (buildSlimeBlock) {
                mBlockManagers[mPlayerIndex].BuildOneBlock(mPlayerIndex, mIsHitState, 3);
            } else {
                Debug.Log("Build block color (SkillCast): " + mBlockColor);
                mBlockManagers[mPlayerIndex].BuildOneBlock(mPlayerIndex, mIsHitState, color);
                mMusic.clip = Resources.Load<AudioClip>("music/Audio_Build");
                mMusic.Play();
            }
        }
        RefreshBlockHeight();
    }
    public void ServiceEndState()
    {
    }
    
}
