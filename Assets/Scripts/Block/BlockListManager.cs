using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockListManager : MonoBehaviour
{
    private enum BlockState {
        eIdle,
        eWait,
        eSelectHit,
        eHit,
        eBuild,
    }

    private enum BlockColor {
        eRed,
        eGreen,
        eBlue
    };

    private BlockState mBlockState = BlockState.eIdle;

    private BlockColor mBlockColor = BlockColor.eRed;

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


    private bool p1Turn = true;
    private float time;

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
            case BlockState.eSelectHit:
                ServiceSelectHitState();
                break;
            case BlockState.eBuild:
                ServiceBuildState();
                break;
            case BlockState.eHit:
                ServiceHitState();
                break;
        }
    }

    private void ServiceIdleState() {
        mBlockColor = (BlockColor)Random.Range(0, 3);
        p1Animator = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer1().GetComponent<PlayerBehaviour>().animator;
        p2Animator = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer2().GetComponent<PlayerBehaviour>().animator;
        Debug.Log("P1Turn: " + p1Turn);
        if (p1Turn) {
            p1Animator.SetBool("IsHolding", true);
            p2Animator.SetBool("IsHolding", false);

            // BlockColor : 0 - Green, 1 - Red, 2 - Blue
            p1Animator.SetInteger("BlockColor", (int)mBlockColor);
        } else {
            p1Animator.SetBool("IsHolding", false);
            p2Animator.SetBool("IsHolding", true);
            p2Animator.SetInteger("BlockColor", (int)mBlockColor);
        }
        Debug.Log("Idle");
        mBlockState = BlockState.eWait;
    }
    
    private void ServiceWaitState() {

        if (Input.GetKeyDown(KeyCode.B)) {
            mBlockState = BlockState.eBuild;
            Debug.Log("Build");
            return ;
        } 

        // Only if the player has blocks, can he be hit
        if (Input.GetKeyDown(KeyCode.H) && ((p1Turn && mP2BlockManager.GetHeight() > 0) || (!p1Turn && mP1BlockManager.GetHeight() > 0))) {
            mBlockState = BlockState.eSelectHit;
            Debug.Log("SelectHit");
            return ;
        }
    }

    private void ServiceSelectHitState() {
        if (Input.GetKeyDown(KeyCode.Q) && ((p1Turn && mP1BlockManager.GetHeight() >= 1) || (!p1Turn && mP2BlockManager.GetHeight() >= 1))) {
            mTargetBlockIndex = 1;

            // Hit initialization
            InitializeHit();
            mBlockState = BlockState.eHit;
        }
        if(Input.GetKeyDown(KeyCode.W) && ((p1Turn && mP1BlockManager.GetHeight() >= 2) || (!p1Turn && mP2BlockManager.GetHeight() >= 2))) {
            mTargetBlockIndex = 2;

            InitializeHit();
            mBlockState = BlockState.eHit;
        }
        if(Input.GetKeyDown(KeyCode.E) && ((p1Turn && mP1BlockManager.GetHeight() >= 3) || (!p1Turn && mP2BlockManager.GetHeight() >= 3))) {
            mTargetBlockIndex = 3;
            
            InitializeHit();
            mBlockState = BlockState.eHit;
        }
    }

    public void InitializeHit() {
        if(p1Turn) {
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
            if(p1Turn) {
                mP1BlockManager.DestroyOneBlock(mP1BlockManager.GetHeight() - 1);
                mP2BlockManager.DestroyOneBlock(mP2BlockManager.GetHeight() - mTargetBlockIndex);
            } else {
                mP2BlockManager.DestroyOneBlock(mP2BlockManager.GetHeight() - 1);
                mP1BlockManager.DestroyOneBlock(mP1BlockManager.GetHeight() - mTargetBlockIndex);
            }

            mBlockState = BlockState.eIdle;
            p1Turn = !p1Turn;
            time = 0;
        }
    }

    public void ServiceBuildState() {
        // TODO: Spawn a block
        
        if(p1Turn) {
            mP1BlockManager.BuildOneBlock(p1Turn, (int)mBlockColor);
        } else {
            mP2BlockManager.BuildOneBlock(p1Turn, (int)mBlockColor);
        }
        mBlockState = BlockState.eIdle;
        p1Turn = !p1Turn;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFSM();
    }
}
