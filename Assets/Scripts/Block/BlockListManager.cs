using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using Unity.VisualScripting;
//use this to unify the color system
using BlockColor = BlockBehaviour.BlockColourType;
public partial class BlockListManager : MonoBehaviour
{

    // UI
    private GameObject mEndCanvas = null;

    public GameObject[] mSkillButtons = new GameObject[2];

    public GameObject[] mBuildButtons = new GameObject[2];

    public GameObject[] mHitButtons = new GameObject[2];

    public GameObject[] mWinImages = new GameObject[2];

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
        eInvalid
    }

    /*private enum BlockColor {
        eRed,
        eGreen,
        eBlue,
        eSlime
    };*/
    // we should only use one color system in the block behavior


    //For Role Skills
    private enum BlockSkills {
        eNormal,
        eSkills
    };

    //For Getting Skills
    private enum GettingSkills
    {
        eGetNormal,
        eGetSkills
    }

    private BlockState mBlockState = BlockState.eIdle;
    private BlockColor mBlockColor = BlockColor.eRed;
    private BlockSkills mBlockSkills = BlockSkills.eNormal;
    private GettingSkills mGettingSkills = GettingSkills.eGetNormal;

    // Constants
    private const int kInitBlockIndex = 10;
    private const int kPlayerNum = 2;

    private int mTurnCnt = 30;
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
    CinemachineTargetGroup.Target[] targets = null;

    //for hit cool down
    private int[] mHitCoolDown = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
    const int kHitCoolDown = 0;
    private Player.Player curPlayer;
    
    //for user control
    KeyCode mHitKeyCode, mBuildKeyCode, mSkill1KeyCode, mSkill2KeyCode, mUpBlockKey, mDownBlockKey;
    void Start()
    {
        // UI
        
        mCameraControll = FindObjectOfType<CameraControll>();
        mEndCanvas = GameObject.Find("EndCanvas");
        mEndCanvas.SetActive(false);

        for (int i = 0; i < kPlayerNum; i++) {
            mSkillButtons[i] = GameObject.Find("Canvas/UIOfPlayer" + (i + 1) + "/Action/SkillButton");
            mBuildButtons[i] = GameObject.Find("Canvas/UIOfPlayer" + (i + 1) + "/Action/BuildButton");
            mHitButtons[i] = GameObject.Find("Canvas/UIOfPlayer" + (i + 1) + "/Action/HitButton");
            mSkillButtons[i].SetActive(false);
        }
        
        for (int i = 0; i < kPlayerNum; i++) {
            mBlockManagers[i] = new BlockManager();
            mBlockManagers[i].SetInitPos(GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayerPos(i));
            mPlayers[i] = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer(i);
            if (mPlayers[i] == null) {
                Debug.Log("Player " + i + " is null");
            }
        }

        for (int i = 0; i < kInitBlockIndex; i++) {
            for (int j = 0; j < kPlayerNum; j++) {
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
                mBlockManagers[j].BuildOneBlock(j, false, -1, true);
            }
        }

        // Audio
        mAudioObj = GameObject.Find("AudioObject");
        mMusic = mAudioObj.GetComponent<AudioSource>();
    }
    void Update()
    {
        UpdateFSM();
    }
    private void UpdateFSM() {
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
        }
    }
    private void ServiceIdleState() {
        
        mTurnCnt--;
        
        if (JudgeVictory(mTurnCnt))
        {
            CameraEnd(mPlayers[1 - mPlayerIndex], mPlayers[mPlayerIndex]);
        }
        else
        {
            RoundRefresh();
            UpdatePlayerKeyBinding();
            mBlockColor = (BlockColor)Random.Range(0, 3);
            
            //float randomSkill = Random.Range(0f, 1f);

            for (int i = 0; i < kPlayerNum; i++)
            {
                mSkillButtons[i].SetActive(false);
            }
            for (int i = 0; i < kPlayerNum; i++)
            {
                mPlayerAnimators[i] = mPlayers[i].GetComponent<PlayerBehaviour>().animator;
                Debug.Log("load animator");
                if (mUIOfPlayers[i] == null)
                {
                    mUIOfPlayers[i] = GameObject.Find("UIOfPlayer" + (i + 1));
                }
            }

            mUIOfPlayers[mPlayerIndex].SetActive(true);
            mPlayerAnimators[mPlayerIndex].SetBool("IsHolding", true);
            mPlayerAnimators[mPlayerIndex].SetInteger("BlockColor", (int)mBlockColor);

            curPlayer = mPlayers[mPlayerIndex].GetComponent<PlayerBehaviour>().GetPlayer();

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
        }
    }
    private void ServiceSkillState() {
        
        for (int i = 0; i < kPlayerNum; i++) {
            mSkillButtons[i].SetActive(false);
        }

        //For Pernsonal Skills
        float rand = Random.Range(0f, 1f);
        if (rand > 1.0f) {
            mBlockSkills = BlockSkills.eNormal;
        }
        else {
            mSkillButtons[mPlayerIndex].SetActive(true);
            mBlockSkills = BlockSkills.eSkills;
        }

        //For Getting Skills
        float rand2 = Random.Range(0f, 1f);
        if (rand2 > 1.0f)
        {
            mGettingSkills = GettingSkills.eGetNormal;
        }
        else
        {
            Debug.Log("You got a skill!");
            /*Need Button Code*/
            mGettingSkills = GettingSkills.eGetSkills;
        }

        curPlayer.IncreaseTimeUntilNextSkill();
        mBlockState = BlockState.eWait;
    }
    private void ServiceWaitState() {

        if (Input.GetKeyDown(mBuildKeyCode)) {
            mBlockState = BlockState.eBuild;
            return ;
        } 

        // Only if the player has blocks, can he be hit
        if (Input.GetKeyDown(mHitKeyCode) && (((mPlayerIndex == 0) && mBlockManagers[1].GetHeight() > 0) || ((mPlayerIndex == 1) && mBlockManagers[0].GetHeight() > 0))) 
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
                mBlockState = BlockState.eSelectHit;
                return;
            }
            else
            {
                Debug.Log("Hit cool down..");
            }
        }


        //Use Getting skills
        TriGettingSkill();

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
                }
            }
        }
    }
    private void ServiceInitHitState() {
        InitializeHit();
        mBlockState = BlockState.eHit;
        
    }
    private void ServiceSelectHitState() {
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

        if (Input.GetKeyDown(mHitKeyCode)) {
            mCameraControll.CameraFocusOnBlock(mTargetBlock);
            mIsHitState = true;
            mBlockState = BlockState.eBuild;
            return ;
        }

        // You can retract the selection
        if (Input.GetKeyDown(mBuildKeyCode)) {
            mBlockAnimator.SetBool("IsSelected", false);
            mBlockState = BlockState.eBuild;
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
                }
            }
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
            mMusic.volume = 3.0f;
            mMusic.Play();
            Debug.Log("Hit");
            if (mPlayerIndex == 0) {
                GameObject bullet = mBlockManagers[0].GetBlockAt(mBlockManagers[0].GetHeight() - 1);
                mBlockManagers[1].BeingHitBlockDestroy(bullet, mBlockManagers[1].GetHeight() - mTargetBlockIndex);//player 2被击打的玩家
                mBlockManagers[0].DestroyOneBlock(mBlockManagers[0].GetHeight() - 1);//player 1: 当前的玩家

                mCameraControll.CameraFocusOnBlock(mTargetBlock);

                
            } else {
                GameObject bullet = mBlockManagers[1].GetBlockAt(mBlockManagers[1].GetHeight() - 1);
                mBlockManagers[0].BeingHitBlockDestroy(bullet,mBlockManagers[0].GetHeight() - mTargetBlockIndex);//player 1
                mBlockManagers[1].DestroyOneBlock(mBlockManagers[1].GetHeight() - 1);//player 2: 当前的玩家
                mCameraControll.CameraFocusOnBlock(mTargetBlock);
            }
            
            mBlockState = BlockState.eCombo;
            mIsHitState = false;
            mTime = 0;
        }
    }
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
            mHitCoolDown[mPlayerIndex] = kHitCoolDown;
            mBlockState = BlockState.eIdle;
            
            mPlayerIndex = (mPlayerIndex + 1) % 2;
            mIsHitState = false;
            mTime = 0;
        }
    }
    public void ServiceBuildState(bool noSkillCast = true, bool buildSlimeBlock = false) {
        if (noSkillCast)
        {
            string msg = "Build: the newly spawn block color is" + mBlockColor;
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
                mBlockManagers[mPlayerIndex].BuildOneBlock(mPlayerIndex, mIsHitState, -1);
                mMusic.clip = Resources.Load<AudioClip>("music/Audio_Build");
                mMusic.Play();
            }
        }
    }
    public void ServiceEndState() {
    }
}
