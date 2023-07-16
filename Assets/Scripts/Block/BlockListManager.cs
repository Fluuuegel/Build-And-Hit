using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class BlockListManager : MonoBehaviour
{
    
    private void ModifyTargetWeight(string targetName, float weight)
    {
        CinemachineTargetGroup.Target[] targets = PlayerManager.mTargetGroup.m_Targets;
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].target != null && targets[i].target.name == targetName)
            {
                targets[i].weight = weight;
            }
        }
    }
    private enum BlockState {
        eIdle,
        eWait,
        eInitHit,
        eSelectHit,
        eHit,
        eBuild,
        eCombo
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
    public bool isLocked = true;
    private int mTargetBlockIndex = 0;
    
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
    private float time;

    private AudioBehaviour AudioBehaviour;
    private GameObject AudioObject = null;
    private AudioSource music = null;

    // Start is called before the first frame update
    void Start()
    {
        
        mP1BlockManager = new BlockManager();
        mP2BlockManager = new BlockManager();
        mP1BlockManager.SetInitPos(GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer1Pos());
        mP2BlockManager.SetInitPos(GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer2Pos());
        p1 = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer1();
        p2 = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer2();
        time = 0;
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
        }
    }

    private void ServiceIdleState() {
        mBlockColor = (BlockColor)Random.Range(0, 3);
        float randomSkill = Random.Range(0f, 1f);
        if (randomSkill > 0.2f)
        {
            mBlockSkills = BlockSkills.Normal;
        }
        else
        {
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

            ModifyTargetWeight("Player1", 10f);
            ModifyTargetWeight("Player2", 3f);
            // BlockColor : 0 - Green, 1 - Red, 2 - Blue
            p1Animator.SetInteger("BlockColor", (int)mBlockColor);
        } else {
            UIOfPlayer1.SetActive(false);
            UIOfPlayer2.SetActive(true);
            p1Animator.SetBool("IsHolding", false);
            p2Animator.SetBool("IsHolding", true);
            p2Animator.SetInteger("BlockColor", (int)mBlockColor);

            ModifyTargetWeight("Player1", 3f);
            ModifyTargetWeight("Player2", 10f);
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
            Debug.Log("SelectHit");
            return ;
        }


        //Use skills
        if (Input.GetKeyDown(KeyCode.P) && (mBlockSkills == BlockSkills.Skills))
        {
            Debug.Log("It's skill time!");
            //temporarily: Destroy the first Block of enemy
            {
                mTargetBlockIndex = 1;
                beHitBlock = mP1BlockManager.GetBlockAt(mP1BlockManager.GetHeight() - mTargetBlockIndex);
                beHitBlockPos = beHitBlock.transform.position;
                if (p1Turn)
                {
                    GameObject bullet = mP1BlockManager.GetBlockAt(mP1BlockManager.GetHeight() - 1);
                    mP2BlockManager.BeingHitBlockDestroy(bullet, mP2BlockManager.GetHeight() - mTargetBlockIndex);//player 2被击打的玩家
                    //mP1BlockManager.DestroyOneBlock(mP1BlockManager.GetHeight() - 1);//player 1: 当前的玩家
                }
                else
                {
                    GameObject bullet = mP2BlockManager.GetBlockAt(mP2BlockManager.GetHeight() - 1);
                    mP1BlockManager.BeingHitBlockDestroy(bullet, mP1BlockManager.GetHeight() - mTargetBlockIndex);//player 1
                    //mP2BlockManager.DestroyOneBlock(mP2BlockManager.GetHeight() - 1);//player 2: 当前的玩家
                }
                music.clip = Resources.Load<AudioClip>("music/Audio_Debuff");
                music.Play();
            }
            mBlockSkills = BlockSkills.Normal;//Skills just use once
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
            //todo: fix here so the next state is combo
            mBlockState = BlockState.eCombo;
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
            //Debug.Log("isHit == true");
            return ;
        }
        music.clip = Resources.Load<AudioClip>("music/Audio_Build");
        music.Play();
        mBlockState = BlockState.eIdle;
        p1Turn = !p1Turn;
    }

    public void ServiceComboState()
    {
        
        mBlockState = BlockState.eIdle;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFSM();
    }
}
