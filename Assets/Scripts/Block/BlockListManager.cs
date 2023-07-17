using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class BlockListManager : MonoBehaviour
{
    private GameObject mEndCanvas = null;
    public Text mEndText = null;

    public GameObject mSkillButton1 = null;
    public GameObject mSkillButton2 = null;
    
    private GameObject textObj = null;
    //private float mShakeDelay = 0.3f;
    //private float mCameraBackDelay = 1f;
    
    
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
    private enum BlockSkills
    {
        Normal,
        Skills
    };

    private BlockState mBlockState = BlockState.eIdle;

    private BlockColor mBlockColor = BlockColor.eRed;

    private BlockSkills mBlockSkills = BlockSkills.Normal;

    public float hitSpeed = 2f;

    private int mTargetBlockIndex = 0;

    private int mInitBlockIndex = 6;
    
    private BlockManager mP1BlockManager;
    private BlockManager mP2BlockManager;
    private GameObject hitBlock;
    private GameObject beHitBlock;
    private Vector3 hitBlockPos;
    private Vector3 beHitBlockPos;

    private Animator p1Animator = null;
    private Animator p2Animator = null;

    private GameObject p1;
    private GameObject p2;
    private GameObject UIOfPlayer1, UIOfPlayer2;

    private bool p1Turn = true;
    private bool isHit = false;
    private float time = 0f;

    private AudioBehaviour AudioBehaviour;
    private GameObject AudioObject = null;
    private AudioSource music = null;
    private CameraControll mCameraControll = null;

    // Start is called before the first frame update
    void Start()
    {
        // UI
        mCameraControll = FindObjectOfType<CameraControll>();
        mEndCanvas = GameObject.Find("EndCanvas");
        if (mEndCanvas == null) {
            Debug.LogError("EndCanvas not found");
            return;
        }

        textObj = GameObject.Find("EndCanvas/Panel/EndText");
        if (textObj == null) {
            Debug.LogError("EndText not found");
            return;
        }

        mEndText = textObj.GetComponent<Text>();
        if (mEndText == null) {
            Debug.LogError("Text component not found on EndText");
            return;
        }

        mEndCanvas.SetActive(false);

        mSkillButton1 = GameObject.Find("Canvas/UIOfPlayer1/Action/SkillButton");
        mSkillButton2 = GameObject.Find("Canvas/UIOfPlayer2/Action/SkillButton");
        mSkillButton1.SetActive(false);
        mSkillButton2.SetActive(false);

        mP1BlockManager = new BlockManager();
        mP2BlockManager = new BlockManager();
        mP1BlockManager.SetInitPos(GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer1Pos());
        mP2BlockManager.SetInitPos(GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer2Pos());
        p1 = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer1();
        p2 = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer2();

        for (int i = 0; i < mInitBlockIndex; i++) {
            mP1BlockManager.BuildOneBlock(true, false, -1);
            mP2BlockManager.BuildOneBlock(false, false, -1);
        }
        AudioObject = GameObject.Find("AudioObject");
        music = AudioObject.GetComponent<AudioSource>();
    }

    private void UpdateFSM() {
        switch (mBlockState)
        {
            case BlockState.eIdle:
                ServiceIdleState();
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
        if (p1Turn && mP1BlockManager.GetHeight() == 0) {
            Debug.Log("Player 2 Win!");
            mBlockState = BlockState.eEnd;
            return ;
        } else if (!p1Turn && mP2BlockManager.GetHeight() == 0) {
            Debug.Log("Player 1 Win!");
            mBlockState = BlockState.eEnd;
            return ;
        }

        mBlockColor = (BlockColor)Random.Range(0, 3);
        float randomSkill = Random.Range(0f, 1f);

        mSkillButton1.SetActive(false);
        mSkillButton2.SetActive(false);

        if (randomSkill > 0.2f)
        {
            // TODO: Add UI
            mBlockSkills = BlockSkills.Normal;
        }
        else
        {
            if (p1Turn)
            {
                mSkillButton1.SetActive(true);
            }
            else
            {
                mSkillButton2.SetActive(true);
            }
            mBlockSkills = BlockSkills.Skills;
            Debug.Log("Find Skill Block!");
        }
        p1Animator = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer1().GetComponent<PlayerBehaviour>().animator;
        p2Animator = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer2().GetComponent<PlayerBehaviour>().animator;
        if(UIOfPlayer1 == null || UIOfPlayer2 == null)
        {
            UIOfPlayer1 = GameObject.Find("UIOfPlayer1");
            UIOfPlayer2 = GameObject.Find("UIOfPlayer2");
        }
        if (p1Turn) {
            UIOfPlayer1.SetActive(true);
            UIOfPlayer2.SetActive(false);
            p1Animator.SetBool("IsHolding", true);
            p2Animator.SetBool("IsHolding", false);

            mCameraControll.ModifyTarget("Player1", 10f, 5f);
            mCameraControll.ModifyTarget("Player2", 3f, 5f);
            // BlockColor : 0 - Green, 1 - Red, 2 - Blue
            p1Animator.SetInteger("BlockColor", (int)mBlockColor);
        } else {
            UIOfPlayer1.SetActive(false);
            UIOfPlayer2.SetActive(true);
            p1Animator.SetBool("IsHolding", false);
            p2Animator.SetBool("IsHolding", true);
            p2Animator.SetInteger("BlockColor", (int)mBlockColor);

            mCameraControll.ModifyTarget("Player2", 10f, 5f);
            mCameraControll.ModifyTarget("Player1", 3f, 5f);
        }
        mBlockState = BlockState.eWait;
    }
    
    private void ServiceWaitState() {

        if (Input.GetKeyDown(KeyCode.B)) {
            mBlockState = BlockState.eBuild;
            return ;
        } 

        // Only if the player has blocks, can he be hit
        if (Input.GetKeyDown(KeyCode.H) && ((p1Turn && mP2BlockManager.GetHeight() > 0) || (!p1Turn && mP1BlockManager.GetHeight() > 0))) {
            mBlockState = BlockState.eSelectHit;
            return ;
        }


        //Use skills
        if (Input.GetKeyDown(KeyCode.P) && (mBlockSkills == BlockSkills.Skills))
        {
            // Destroy the first Block of the enemy
            {
                mTargetBlockIndex = 1;
                beHitBlock = mP1BlockManager.GetBlockAt(mP1BlockManager.GetHeight() - mTargetBlockIndex);
                beHitBlockPos = beHitBlock.transform.position;
                if (p1Turn)
                {
                    GameObject bullet = mP1BlockManager.GetBlockAt(mP1BlockManager.GetHeight() - 1);
                    mP2BlockManager.BeingHitBlockDestroy(bullet, mP2BlockManager.GetHeight() - mTargetBlockIndex);
                }
                else
                {
                    GameObject bullet = mP2BlockManager.GetBlockAt(mP2BlockManager.GetHeight() - 1);
                    mP1BlockManager.BeingHitBlockDestroy(bullet, mP1BlockManager.GetHeight() - mTargetBlockIndex);
                }
                music.clip = Resources.Load<AudioClip>("music/Audio_Debuff");
                music.Play();
            }
            mBlockSkills = BlockSkills.Normal;
            return;
        }
    }

    private void ServiceInitHitState() {
        InitializeHit();
        mBlockState = BlockState.eHit;
    }

    private void ServiceSelectHitState() {
        if (Input.GetKeyDown(KeyCode.Q) && ((p1Turn && mP2BlockManager.GetHeight() >= 1) || (!p1Turn && mP1BlockManager.GetHeight() >= 1))) {
            mTargetBlockIndex = 1;

            isHit = true;
            mBlockState = BlockState.eBuild;
        }
        if (Input.GetKeyDown(KeyCode.W) && ((p1Turn && mP2BlockManager.GetHeight() >= 2) || (!p1Turn && mP1BlockManager.GetHeight() >= 2))) {
            mTargetBlockIndex = 2;

            isHit = true;
            mBlockState = BlockState.eBuild;
        }
        if (Input.GetKeyDown(KeyCode.E) && ((p1Turn && mP2BlockManager.GetHeight() >= 3) || (!p1Turn && mP1BlockManager.GetHeight() >= 3))) {
            mTargetBlockIndex = 3;
            
            isHit = true;
            mBlockState = BlockState.eBuild;
        }

        // You can retract the selection
        if (Input.GetKeyDown(KeyCode.B)) {
            mBlockState = BlockState.eBuild;
        }
    }

    public void InitializeHit() {
        if (p1Turn) {
            hitBlock = mP1BlockManager.GetBlockAt(mP1BlockManager.GetHeight() - 1);
            beHitBlock = mP2BlockManager.GetBlockAt(mP2BlockManager.GetHeight() - mTargetBlockIndex);
        } else {
            hitBlock = mP2BlockManager.GetBlockAt(mP2BlockManager.GetHeight() - 1);
            beHitBlock = mP1BlockManager.GetBlockAt(mP1BlockManager.GetHeight() - mTargetBlockIndex);
        }
        hitBlockPos = hitBlock.transform.position;
        beHitBlockPos = beHitBlock.transform.position;
    }

    public void ServiceHitState() {

        time += hitSpeed * Time.smoothDeltaTime;

        float x = Mathf.LerpUnclamped(hitBlockPos.x, beHitBlockPos.x, time);
        float y = Mathf.LerpUnclamped(hitBlockPos.y, beHitBlockPos.y, time);
        hitBlock.transform.position = new Vector3(x, y, 0);
        
        BlockBehaviour HitBlockScript = hitBlock.GetComponent<BlockBehaviour>();
        HitBlockScript.targetCollisionObject = beHitBlock;
        bool isDestroy = HitBlockScript.isColli();
        if (isDestroy) {
            music.clip = Resources.Load<AudioClip>("music/Audio_Hit");
            music.Play();
            if (p1Turn) {
                GameObject bullet = mP1BlockManager.GetBlockAt(mP1BlockManager.GetHeight() - 1);
                mP2BlockManager.BeingHitBlockDestroy(bullet, mP2BlockManager.GetHeight() - mTargetBlockIndex);//player 2被击打的玩家
                mP1BlockManager.DestroyOneBlock(mP1BlockManager.GetHeight() - 1);//player 1: 当前的玩家
                
            } else {
                GameObject bullet = mP2BlockManager.GetBlockAt(mP2BlockManager.GetHeight() - 1);
                mP1BlockManager.BeingHitBlockDestroy(bullet,mP1BlockManager.GetHeight() - mTargetBlockIndex);//player 1
                mP2BlockManager.DestroyOneBlock(mP2BlockManager.GetHeight() - 1);//player 2: 当前的玩家
            }
            
            mBlockState = BlockState.eCombo;
            isHit = false;
            time = 0;
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

    public BlockManager activeManager;
    public void ServiceComboState()
    {
        Debug.Log("Combo in block list");
        if (p1Turn)
        {
            activeManager = mP2BlockManager;
            CameraEffect(p2);
        }
        else
        {
            activeManager = mP1BlockManager;
            CameraEffect(p1);
        }

        if (activeManager.UpdateComboState())
        {
            Debug.Log("Combo done");
            mBlockState = BlockState.eIdle;
            p1Turn = !p1Turn;
            isHit = false;
            time = 0;
        }
    }

    public void ServiceBuildState() {
        // TODO: Spawn a block
        if (p1Turn) {
            mP1BlockManager.BuildOneBlock(p1Turn, isHit, (int)mBlockColor);
            p1Animator.SetBool("IsHolding", false);
        } else {
            mP2BlockManager.BuildOneBlock(p1Turn, isHit, (int)mBlockColor);
            p2Animator.SetBool("IsHolding", false);
        }
        if (isHit) {
            mBlockState = BlockState.eInitHit;
            return ;
        }
        music.clip = Resources.Load<AudioClip>("music/Audio_Build");
        music.Play();
        mBlockState = BlockState.eIdle;
        p1Turn = !p1Turn;
    }

    public void ServiceEndState() {
        mEndCanvas.SetActive(true);
        mEndText.text =  "P " + (p1Turn ? "2" : "1") + " Win!";
    }

    void Update()
    {
        UpdateFSM();
    }
}
