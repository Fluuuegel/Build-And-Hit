using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Rendering;
using Cinemachine;
using Unity.VisualScripting;

public class BlockManager
{
    public const int kInitalBlockCount = 0;
    private int mComboBound = 3; //more than x blocks in a row will be destroyed, x is mComboBound
    private static GameObject[] mBlockPrefabs = new GameObject[4];
    private List<GameObject> mBlocks = new List<GameObject>();
    //for skills to protect player tower
    public int mImmuneRound = 0;
    //for multiple destroy skill
    public int mBlockDestroyCascade= 0;
    
    const int DefaultCoolDown = 3;
    public void RefreshRound()
    {
        
        if(mImmuneRound > 0)
        {
            mImmuneRound--;
        }
    }
    public GameObject GetBlockAt(int index)
    {
        if (index >= mBlocks.Count || index < 0)
        {
            return null;
        }

        return mBlocks[index];
    }

    public BlockBehaviour.BlockColourType GetBlockColorAt(int index)
    {
        if (index >= mBlocks.Count)
        {
            return BlockBehaviour.BlockColourType.red;
        }

        GameObject p = mBlocks[index];
        BlockBehaviour script = p.GetComponent<BlockBehaviour>();
        return script.GetBlockColour();
    }
    private Vector2 mPlayerInitPos;
    private Vector2 mBlockInitPos;
    private int mCurLayerCount = 1;

    public void SetInitPos(Vector2 pos) {

        mPlayerInitPos = pos;
        mBlockInitPos = pos;
        mBlockInitPos.y = 18f;
    }


    public BlockManager()
    {

        InitializeBlocks();
    }

    public void InitializeBlocks()
    {
        mBlockPrefabs[0] = Resources.Load<GameObject>("Prefabs/RedCube");
        mBlockPrefabs[1] = Resources.Load<GameObject>("Prefabs/GreenCube");
        mBlockPrefabs[2] = Resources.Load<GameObject>("Prefabs/BlueCube");
        mBlockPrefabs[3] = Resources.Load<GameObject>("Prefabs/BlueSlimeCube");
    }

    public void BuildOneBlock(int playerIndex = -1, bool isHit = false, int color = -1, bool init = false)
    {
        if (playerIndex == -1)
        {
            return;
        }
        string msg = "block colour " + color + " ";
        Debug.Log(msg);
        SpawnNewBlock(playerIndex, isHit, GetHeight(), color, init);
    }
    

    private void SpawnNewBlock(int playerIndex, bool isHit, int index, int color = -1, bool init = false)
    {
        if (color == -1)
        {
            color = Random.Range(0, 3);
        }

        GameObject p = GameObject.Instantiate(mBlockPrefabs[color]) as GameObject;
        PlayerManager.mTargetGroup.AddMember(p.transform, 1f, 3f);

        GameObject p1 = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer(0);
        GameObject p2 = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer(1);
        Vector3 p1Pos = p1.transform.position;
        Vector3 p2Pos = p2.transform.position;
        BlockBehaviour script = p.GetComponent<BlockBehaviour>();
        SpriteRenderer spriteRenderer = p.GetComponent<SpriteRenderer>();
        SortingGroup blockSortingGroup = p.GetComponent<SortingGroup>();

        // Set the sorting layer of the block
        // Set the position of the players in game
    
        mBlocks.Add(p);

        // Set block
        if(playerIndex == 0) { // Change the position of the block
            if(isHit) {
                p.transform.position = new Vector3(p1Pos.x, p1Pos.y + 1.0f, 0f);
            } else {
                p.transform.position = new Vector3(p1Pos.x, p1Pos.y - 0.1f, 0f);
                script.mParticle = script.GetComponent<ParticleSystem>();
                script.mParticle.Play();
            }
        } else {
            if (isHit) {
                p.transform.position = new Vector3(p2Pos.x, p2Pos.y + 1.0f, 0f);
            } else {
                p.transform.position = new Vector3(p2Pos.x, p2Pos.y - 0.1f, 0f);
                script.mParticle = script.GetComponent<ParticleSystem>();
                script.mParticle.Play();
            }
        }

        // Set sorting layer of the block
        
        if (playerIndex == 0 && !isHit) {
            blockSortingGroup.sortingLayerName = "PlayerCube";
            blockSortingGroup.sortingOrder = mCurLayerCount;
            p1.transform.position = new Vector3(p1Pos.x, p1Pos.y + 1.0f, 0f);
        }
        else if (playerIndex == 1 && !isHit) {
            blockSortingGroup.sortingLayerName = "EnemyCube";
            blockSortingGroup.sortingOrder = mCurLayerCount;
            p2.transform.position = new Vector3(p2Pos.x, p2Pos.y + 1.0f, 0f);
        }

        spriteRenderer.sortingOrder = mCurLayerCount;
        script.SetBlockManager(this);
        script.SetBlockIndex(index);
        SetBlockColor(color, script);
        mCurLayerCount++;
    }

    // Set the color of the block

    public int DestroyOneBlock(int index = 0)
    {
        if (mBlocks.Count == 0 || index >= mBlocks.Count)
        {
            return -1;
        }

        GameObject p = mBlocks[index];
        BlockBehaviour BlockScript = p.GetComponent<BlockBehaviour>();

        //delete from list
        mBlocks.RemoveAt(index);
        //delete the instance
        BlockScript.SelfDestroy();
        //mCurLayerCount--;
        for (int i = 0; i < mBlocks.Count; i++)
        {
            SpriteRenderer spriteRenderer = mBlocks[i].GetComponent<SpriteRenderer>();
            //spriteRenderer.sortingOrder = i + 1;
            BlockScript = mBlocks[i].GetComponent<BlockBehaviour>();
            BlockScript.SetBlockIndex(i);
        }

        return 1;
    }

    public int GetHeight()
    {
        return mBlocks.Count;
    }

    private void SetBlockColor(int color, BlockBehaviour blockBehaviour)
    {
        if (color == 0)
        {
            blockBehaviour.SetBlockColour(BlockBehaviour.BlockColourType.red);
        }
        else if (color == 1)
        {
            blockBehaviour.SetBlockColour(BlockBehaviour.BlockColourType.green);
        }
        else if (color == 2)
        {
            blockBehaviour.SetBlockColour(BlockBehaviour.BlockColourType.blue);
        }
        else if (color == 3) {
            blockBehaviour.SetBlockColour(BlockBehaviour.BlockColourType.slime);
        } else 
        {
            Debug.Log("SetBlockColor: Wrong color!");
        }
    }

    public static bool SameColor(GameObject block1, GameObject block2)
    {
        BlockBehaviour b1 = block1.GetComponent<BlockBehaviour>();
        BlockBehaviour b2 = block2.GetComponent<BlockBehaviour>();
        return b1.GetBlockColour() == b2.GetBlockColour();
    }

    public void BeingHitBlockDestroy(GameObject hitBlock, int index = 0)
    {
        if (index >= mBlocks.Count || !hitBlock)
        {
            return;
        }
        
        int startDeleteIndex = index;
        int blocksToDestroyCnt = 1;
        if (!BlockManager.SameColor(hitBlock, mBlocks[index]))
        {
            DestroyOneBlock(index);
            
        }
        else
        {
            int temp = index;

            while (index > 0 && SameColor(mBlocks[index], mBlocks[index - 1]))
            {
                index--;
                blocksToDestroyCnt++;
            }

            startDeleteIndex = index;
            index = temp;
            while (index <= mBlocks.Count - 2 && SameColor(mBlocks[index], mBlocks[index + 1]))
            {
                index++;
                blocksToDestroyCnt++;
            }
            for (int i = 0; i < blocksToDestroyCnt; i++)
            {
                DestroyOneBlock(startDeleteIndex);
            }
        }

        targetBlock1 = GetBlockAt(startDeleteIndex - 1);
        targetBlock2 = GetBlockAt(startDeleteIndex);
        setComboLowerBound(startDeleteIndex -1);
    }

    #region combo_logic
    
    public bool canCombo = true;                // the next combo is possible to happen
    public bool targetBlockCollided = false;    // the target block is collided
    public bool readyCombo = false;             // the next combo is ready to happen (0.5s after collision)
    public int comboLowerBound;                 // the lower bound of combo
    public GameObject targetBlock1 = null, targetBlock2 = null; // the 2 blocks that are about to collide after last elimination(both hit and combo)
    private float setUpTimeBetweenCollisionAndCombo = 0.3f;     // the time between collision and combo elimination
    private float collisionTimer = 0.0f;                        //fixme: this is not used yet

    public void SetTargetBlock(GameObject block1, GameObject block2)
    {
        targetBlock1 = block1;
        targetBlock2 = block2;
    }
    public bool IsTargetBlock(GameObject block1, GameObject block2){
        return (block1 == targetBlock1 && block2 == targetBlock2) || (block2 == targetBlock1 && block1 == targetBlock2);
    }
    public void setComboLowerBound(int lower_bound)
    {
        comboLowerBound = lower_bound;
    }

    /* @UpdateComboState
     * return true means that combo is achieved!
     */
    private void resetCombo()
    {
        canCombo = true;
        targetBlockCollided = false;
        readyCombo = false;
        targetBlock1 = targetBlock2 = null;
    }
    public bool UpdateComboState()
    {
        if(canCombo == false || (!targetBlock1) || (!targetBlock2))// no more combo from now!
        {
            resetCombo();
            return true;
        }
        //fixme: block collision ignore first
        if (targetBlockCollided)
        {
            MarkBlockForDestroy(comboLowerBound);
            TriggerReadyForCombo();
            if (readyCombo)
            {
                ComboInfo comboInfo = ComboFrom(comboLowerBound);
                if (!comboInfo.combo_achieved)//combo already fail, end of all
                {
                    canCombo = false;
                }
                else // combo is success, prepare for next combo
                {
                    comboLowerBound = comboInfo.lower_bound - 1;
                    if (comboLowerBound < 0)
                    {
                        canCombo = false;
                    }
                    targetBlock1 = GetBlockAt(comboLowerBound);
                    targetBlock2 = GetBlockAt(comboLowerBound + 1);
                    if(!targetBlock1 || !targetBlock2)
                    {
                        canCombo = false;
                    }
                    targetBlockCollided = false;
                    readyCombo = false;
                }
            }
        }
        return false;
    }

    private void MarkBlockForDestroy(int lower_index)
    {
        if (lower_index < 0)
        {
            return;
        }

        int combo_cnt = 0;
        if (GetBlockColorAt(lower_index) != GetBlockColorAt(lower_index + 1))
        {
            return;
        }
        else
        {
            combo_cnt = 1;
        }
        
        int up_bound = lower_index, low_bound = lower_index;
        while (up_bound < mBlocks.Count - 1 && GetBlockColorAt(up_bound) == GetBlockColorAt(up_bound + 1))
        {
            up_bound++;
            combo_cnt++;
        }

        while (low_bound > 0 && GetBlockColorAt(low_bound) == GetBlockColorAt(low_bound - 1))
        {
            low_bound--;
            combo_cnt++;
        }

        if (combo_cnt < mComboBound)//not enough combo
            return;
        for(int i = low_bound; i <= up_bound; i++)
        {
            SetTransparent(mBlocks[i],0.3f);
        }
        
    }

    private void CameraShake()
    {
        CinemachineImpulseSource mShakeSource = GameObject.Find("VirtualCamera").GetComponent<CinemachineImpulseSource>();
        mShakeSource.GenerateImpulse();
    }
    /*
     * @TargetBlockCollided
     * for blocks to call when they collided
     */
    public bool TargetBlockCollided()
    {
        if (targetBlockCollided == false)
        {
            collisionTimer = Time.time;//record the collision time
        }
        targetBlockCollided = true;
        return targetBlockCollided;
    }
    
    /*
     * 
     */
    private bool TriggerReadyForCombo()
    {
        //todo: add count down
        if(Time.time - collisionTimer > setUpTimeBetweenCollisionAndCombo)
        {
            readyCombo = true;
        }
        return readyCombo;
    }
    /*
     * @CombolBlockInRange
     * destroy blocks in range [lower_bound, lower_bound + num)
     */
    private void CombolBlockInRange(int lower_bound, int num)
    {
        for(int i = 0; i < num; i++)
        {
            DestroyOneBlock(lower_bound);
        }
    }

    //parameter is the index of starting index of two block at edge
    public struct ComboInfo
    {
        public bool combo_achieved;
        public int lower_bound;
    }

    /*
     * @CombolFrom
     * return true if combo is achieved
     */
    public ComboInfo ComboFrom(int lower_index)
    {
        ComboInfo comboInfo = new ComboInfo();
        if (lower_index < 0)
        {
            comboInfo.combo_achieved = false;
            return comboInfo;
        }

        int combo_cnt = 0;
        if (GetBlockColorAt(lower_index) != GetBlockColorAt(lower_index + 1))
        {
            comboInfo.combo_achieved = false;
            return comboInfo;
        }
        else
        {
            combo_cnt = 1;
        }
        
        int up_bound = lower_index, low_bound = lower_index;
        while (up_bound < mBlocks.Count - 1 && GetBlockColorAt(up_bound) == GetBlockColorAt(up_bound + 1))
        {
            up_bound++;
            combo_cnt++;
        }

        while (low_bound > 0 && GetBlockColorAt(low_bound) == GetBlockColorAt(low_bound - 1))
        {
            low_bound--;
            combo_cnt++;
        }
        if (combo_cnt >= mComboBound)
        {
            CombolBlockInRange(low_bound, combo_cnt);
            CameraShake();
            comboInfo.combo_achieved = true;
            comboInfo.lower_bound = low_bound;
        }
        else
        {
            comboInfo.combo_achieved = false;
        }
        
        
        return comboInfo;

    }
    
    private void SetTransparent(GameObject block, float alpha)
    {
        SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
        Color color = sr.color;
        color.a = alpha;
        sr.color = color;
    }
    #endregion

    #region Block select
    public GameObject SelectBlockTemp(int index ){
        return null;
    }
    #endregion
}