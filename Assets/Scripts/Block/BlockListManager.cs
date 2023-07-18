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
    CinemachineTargetGroup.Target[] targets = null;

    //for hit cool down
    private int[] mHitCoolDown = {0,0,0,0,0,0,0,0,0,0};
    const int kHitCoolDown = 0;
    public int GetCoolDownOfPlayer(int index)
    {
        return mHitCoolDown[index];
    }
    
    //for user control
    KeyCode mHitKeyCode, mBuildKeyCode, mSkill1KeyCode, mSkill2KeyCode,mUpBlockKey, mDownBlockKey;

    private void UpdatePlayerKeyBinding()
    {
        if (mPlayerIndex == 0)
        {
            mHitKeyCode = KeyCode.A;
            mBuildKeyCode = KeyCode.D;
            mSkill1KeyCode = KeyCode.Q;
            mSkill2KeyCode = KeyCode.E;
            mUpBlockKey = KeyCode.W;
            mDownBlockKey = KeyCode.S;
        }

        if (mPlayerIndex == 1)
        {
            mHitKeyCode = KeyCode.LeftArrow;
            mBuildKeyCode = KeyCode.RightArrow;
            mSkill1KeyCode = KeyCode.Comma;
            mSkill2KeyCode = KeyCode.Period;
            mUpBlockKey = KeyCode.UpArrow;
            mDownBlockKey = KeyCode.DownArrow;
        }
    }
    
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
                StartCoroutine(TurnInterval());
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
/*
     * @RoundRefresh
     * update the block manager and player status
     * exp: to decrease the cool down of skill, to reduce the immune round of block manager
     * call when every round starts only once
     */
    private void RoundRefresh()
    {
        Debug.Log("Refresh round");
        for(int i = 0; i < kPlayerNum; i++)
        {
            mBlockManagers[i].RefreshRound();
            PlayerBehaviour script = mPlayers[i].GetComponent<PlayerBehaviour>();
            script.RefreshRound();
        }

        if (mHitCoolDown[mPlayerIndex] > 0)
        {
            mHitCoolDown[mPlayerIndex]--;
        }
    }
    private bool JudgeVictory() {
        for (int i = 0; i < kPlayerNum; i++) {
            if (mBlockManagers[i].GetHeight() == 0) {
                
                mEndCanvas.SetActive(true);
                mWinImages[i] = GameObject.Find("EndCanvas/Panel/P" + (i + 1) + "Win");
                mWinImages[1 - i] = GameObject.Find("EndCanvas/Panel/P" + (2 - i) + "Win");
                Debug.Log("P" + (i + 1) + "Win");
                for (int j = 0; j < kPlayerNum; j++) {
                    mHitButtons[j].SetActive(false);
                    mBuildButtons[j].SetActive(false);
                    mSkillButtons[j].SetActive(false);
                }
                mWinImages[i].SetActive(false);
                mWinImages[1 - i].SetActive(true);
                mBlockState = BlockState.eEnd;
                return true;
            }
        }
        return false;
    }

    private IEnumerator TurnInterval() {
        yield return new WaitForSeconds(0.5f);
        Debug.Log("turn interval");
    }

    private void ServiceIdleState() {

        // Judge vectory in idle state
        if (JudgeVictory())
        {
            CameraEnd(mPlayers[1 - mPlayerIndex], mPlayers[mPlayerIndex]);
        }
        else
        {
            RoundRefresh();
            UpdatePlayerKeyBinding();
            mBlockColor = (BlockColor)Random.Range(0, 3);
            
            float randomSkill = Random.Range(0f, 1f);

            for (int i = 0; i < kPlayerNum; i++)
            {
                mSkillButtons[i].SetActive(false);
            }
            const float SkillRate = 1.0f;
            if (randomSkill > SkillRate)
            {
                // TODO: Add UI
                mBlockSkills = BlockSkills.eNormal;
            }
            else
            {
                mSkillButtons[mPlayerIndex].SetActive(true);
                mBlockSkills = BlockSkills.eSkills;
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
        float rand = Random.Range(0f, 1f);

        for (int i = 0; i < kPlayerNum; i++) {
            mSkillButtons[i].SetActive(false);
        }

        if (rand > 1.0f) {
            mBlockSkills = BlockSkills.eNormal;
        }
        else {
            mSkillButtons[mPlayerIndex].SetActive(true);
            mBlockSkills = BlockSkills.eSkills;
        }

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
                mBlockAnimator = mTargetBlock.GetComponent<Animator>();
                mBlockAnimator.SetBool("IsSelected", true);
                //change status to hit
                mBlockState = BlockState.eSelectHit;
                return;
            }
            else
            {
                Debug.Log("!!!Hit is on cooldown!!!!");
            }
        }


        //Use skills
        
        TriggerSkill();
    }
    private bool TriggerSkill()
    {
        if (Input.GetKeyDown(mSkill1KeyCode) && (mBlockSkills == BlockSkills.eSkills))
        {
            //Debug.Log(mPlayerIndex);
            CastPlayerSkill(mPlayers[mPlayerIndex]);
            mBlockSkills = BlockSkills.eNormal;
            mSkillButtons[mPlayerIndex].SetActive(false);
            return true;
        }
        return false;
    }
    private void CastPlayerSkill(GameObject player)
    {
        PlayerBehaviour script = player.GetComponent<PlayerBehaviour>();
        SkillInfo skillInfo = WriteCurrentSkillInfo();
        script.SkillCast(skillInfo);
    }

    private SkillInfo WriteCurrentSkillInfo()
    {
        SkillInfo cur = new SkillInfo();
        cur.PlayerBlockManager = mBlockManagers[mPlayerIndex];
        cur.TargetBlockManager = mBlockManagers[1 - mPlayerIndex];
        cur.CurrentState = mBlockState;
        cur.WillCast = true;
        cur.curPlayerIndex = mPlayerIndex;
        cur.GolbalBlockListManager = this;
        return cur;
    }
    private void ServiceInitHitState() {
        InitializeHit();
        mBlockState = BlockState.eHit;
        
    }

    private void ServiceSelectHitState() {
        // TODO: Up / Down choose
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
            mIsHitState = true;
            mBlockState = BlockState.eBuild;
            return ;
        }

        // You can retract the selection
        if (Input.GetKeyDown(mBuildKeyCode)) {
            mBlockAnimator.SetBool("IsSelected", false);
            mBlockState = BlockState.eBuild;
        }

        TriggerSkill();
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
            mHitCoolDown[mPlayerIndex] = kHitCoolDown;
            mBlockState = BlockState.eIdle;
            mPlayerIndex = (mPlayerIndex + 1) % 2;
            mIsHitState = false;
            mTime = 0;
        }
    }

    public void ServiceBuildState(bool InFSM = true) {
        if (InFSM)
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
        {
            mBlockManagers[mPlayerIndex].BuildOneBlock(mPlayerIndex, mIsHitState, -1);
            mMusic.clip = Resources.Load<AudioClip>("music/Audio_Build");
            mMusic.Play();
        }
    }

    public void ServiceEndState() {
    }

    void Update()
    {
        UpdateFSM();
    }
}
