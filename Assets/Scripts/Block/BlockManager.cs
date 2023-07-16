using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Rendering;

public class BlockManager
{
    public const int kInitalBlockCount = 0;
    private int mComboBound = 3;//more than x blocks in a row will be destroyed, x is mComboBound
    private static GameObject[] mBlockPrefabs = new GameObject[3];
    private List<GameObject> mBlocks = new List<GameObject>();

    public GameObject GetBlockAt(int index)
    {
        if (index >= mBlocks.Count || index < 0)
        {
            Debug.Log("BlockManager::GetBlockAt: index out of range");
            return null;
        }

        return mBlocks[index];
    }

    public BlockBehaviour.BlockColourType GetBlockColorAt(int index)
    {
        if (index >= mBlocks.Count)
        {
            Debug.Log("BlockBehaviour.BlockColourType index out of range");
            return BlockBehaviour.BlockColourType.red;
        }

        GameObject p = mBlocks[index];
        BlockBehaviour script = p.GetComponent<BlockBehaviour>();
        return script.GetBlockColour();
    }

    private Vector2 mPlayerInitPos;
    private Vector2 mBlockInitPos;
    private float mLastTimeBuild = 0f;
    private int mCurLayerCount = 1;
    private bool mCanBuild = true;
    private float SpawnYAxis = 15f;

    public void SetInitPos(Vector2 pos)
    {
        mPlayerInitPos = pos;
        mBlockInitPos = pos;
        mBlockInitPos.y = 18f;
    }

    public BlockManager()
    {
        InitializeBlocks();
    }

    private void SpawnNewBlock(bool p1Turn, bool isHit, int index, int color = -1, int init = 0)
        //if init == 0, then spawn at the top of the screen
    {
        if (color == -1)
        {
            color = Random.Range(0, 2);
        }
        GameObject newBlock = GameObject.Instantiate(mBlockPrefabs[color]) as GameObject;
        GameObject p1 = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer1();
        GameObject p2 = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer2();
        Vector3 p1Pos = p1.transform.position;
        Vector3 p2Pos = p2.transform.position;
        BlockBehaviour script = newBlock.GetComponent<BlockBehaviour>();
        SpriteRenderer spriteRenderer = newBlock.GetComponent<SpriteRenderer>();
        SortingGroup blockSortingGroup = newBlock.GetComponent<SortingGroup>();

        // Set the sorting layer of the block
        // Set the position of the players in game
        

        mBlocks.Add(newBlock);
        /*--------------set new block----------------*/
        if (init > 0)
        {//now is in the gameplay
            if (p1Turn)
            {
                // Change the position of the block
                if (isHit)
                {
                    newBlock.transform.position = new Vector3(p1Pos.x, p1Pos.y + 1.0f, 0f);
                }
                else
                {
                    newBlock.transform.position = new Vector3(p1Pos.x, p1Pos.y - 0.4f, 0f);
                }
            }
            else
            {
                if (isHit)
                {
                    newBlock.transform.position = new Vector3(p2Pos.x, p2Pos.y + 1.0f, 0f);
                }
                else
                {
                    newBlock.transform.position = new Vector3(p2Pos.x, p2Pos.y - 0.4f, 0f);
                }
            }
        }
        else//now is the initialization process
        {
            newBlock.transform.position = new Vector3(mBlockInitPos.x, SpawnYAxis + 0.5f * GetHeight(), 0f);
        }
        /*-----------------player change------------------*/
        if (init > 0)
        {//now is in the gameplay
            if (p1Turn && !isHit)
            {
                blockSortingGroup.sortingLayerName = "PlayerCube";
                blockSortingGroup.sortingOrder = mCurLayerCount;
                p1.transform.position = new Vector3(p1Pos.x, p1Pos.y + 1.0f, 0f);
            }
            else if (!p1Turn && !isHit)
            {
                blockSortingGroup.sortingLayerName = "EnemyCube";
                blockSortingGroup.sortingOrder = mCurLayerCount;
                p2.transform.position = new Vector3(p2Pos.x, p2Pos.y + 1.0f, 0f);
            }
        }
        else
        {//now is the initialization process
            if (p1Turn && !isHit)
            {
                blockSortingGroup.sortingLayerName = "PlayerCube";
                blockSortingGroup.sortingOrder = mCurLayerCount;
                p1.transform.position = new Vector3(p1Pos.x, SpawnYAxis , 0f);
            }
            else if (!p1Turn && !isHit)
            {
                blockSortingGroup.sortingLayerName = "EnemyCube";
                blockSortingGroup.sortingOrder = mCurLayerCount;
                p2.transform.position = new Vector3(p2Pos.x, SpawnYAxis + 1.0f, 0f);
            }
        }
        spriteRenderer.sortingOrder = mCurLayerCount;
        script.SetBlockManager(this);
        script.SetBlockIndex(index);
        SetBlockColor(color, script);
        mCurLayerCount++;
    }

    private void SpawnNewBlock(int index, int color = -1, int init = 0)
        //if init == 0, then spawn at the top of the screen
    {
        GameObject p = GameObject.Instantiate(mBlockPrefabs[color]) as GameObject;
        BlockBehaviour script = p.GetComponent<BlockBehaviour>();
        SpriteRenderer spriteRenderer = p.GetComponent<SpriteRenderer>();
        mBlocks.Add(p);
        if (init > 0)
        {
            p.transform.position = new Vector3(mPlayerInitPos.x, mPlayerInitPos.y + 0.7f * index, 0f);
        }
        else
        {
            p.transform.position = new Vector3(mBlockInitPos.x, SpawnYAxis, 0f);
        }

        spriteRenderer.sortingOrder = mCurLayerCount;
        script.SetBlockManager(this);
        script.SetBlockIndex(index);
        SetBlockColor(color, script);
        mCurLayerCount++;
    }

    public void InitializeBlocks()
    {
        mBlockPrefabs[0] = Resources.Load<GameObject>("Prefabs/RedCube");
        mBlockPrefabs[1] = Resources.Load<GameObject>("Prefabs/GreenCube");
        mBlockPrefabs[2] = Resources.Load<GameObject>("Prefabs/BlueCube");

    }

    // Set the color of the block
    private int buildcnt = 0;
    public int BuildOneBlock(bool p1Turn, bool isHit, int color = -1)
    {
        TriggerBuild();
        if (mCanBuild == false)
        {
            return -1;
        }
        /*cheat
         buildcnt++;
        SpawnNewBlock(p1Turn, isHit, GetHeight(), (buildcnt/2)%2, 1);
        */
        SpawnNewBlock(p1Turn, isHit, GetHeight(), color, 1);
        mCanBuild = false;
        return 1;
    }

    public int BuildOneBlock(int color = -1)
    {
        TriggerBuild();
        if (mCanBuild == false)
        {
            return -1;
        }

        SpawnNewBlock(GetHeight(), color, 0);
        mCanBuild = false;
        return 1;
    }

    public int DestroyOneBlock(int index = 0)
    {
        if (mBlocks.Count == 0 || index >= mBlocks.Count)
        {
            Debug.Log("DestroyOneBlock::No block to destroy!");
            return -1;
        }

        GameObject p = mBlocks[index];
        BlockBehaviour BlockScript = p.GetComponent<BlockBehaviour>();
        /*Debug.Log("正在被摧毁的方块颜色：");
        Debug.Log(GetBlockColorAt(index));*/
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
        //Debug.Log("GetHeight: " + mBlocks.Count);
        return mBlocks.Count;
    }

    private float mInterval = 0.3f;

    private void TriggerBuild()
    {
        if (Time.time - mLastTimeBuild > mInterval)
        {
            mCanBuild = true;
            mLastTimeBuild = Time.time;
        }
        else
        {
            mCanBuild = false;
        }
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
        else
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
        
        Debug.Log(hitBlock.GetComponent<BlockBehaviour>().GetBlockColour());
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
        
        Debug.Log("Entering combo -----------------------------------------------------");
        //CombolFrom(startDeleteIndex - 1);
        targetBlock1 = GetBlockAt(startDeleteIndex - 1);
        targetBlock2 = GetBlockAt(startDeleteIndex);
        setComboLowerBound(startDeleteIndex -1);
    }

    public void test_shoot(int hitBlock_index = 0)
    {
        DestroyOneBlock(hitBlock_index);
    }

    public BlockBehaviour.BlockColourType test_GetBlockColor(GameObject block)
    {
        BlockBehaviour script = block.GetComponent<BlockBehaviour>();
        return script.GetBlockColour();
    }

    public static BlockBehaviour.BlockColourType GetBlockColor(GameObject block)
    {
        BlockBehaviour script = block.GetComponent<BlockBehaviour>();
        return script.GetBlockColour();
    }

    #region combo_logic
    
    public bool canCombo = true;                // the next combo is possible to happen
    public bool targetBlockCollided = false;    // the target block is collided
    public bool readyCombo = false;             // the next combo is ready to happen (0.5s after collision)
    public int comboLowerBound;                 // the lower bound of combo
    public GameObject targetBlock1 = null, targetBlock2 = null; // the 2 blocks that are about to collide after last elimination(both hit and combo)
    private float setUpTimeBetweenCollisionAndCombo = 0.5f;     // the time between collision and combo elimination
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
        Debug.Log("Entering UpdateComboState");
        if(canCombo == false || (!targetBlock1) || (!targetBlock2))// no more combo from now!
        {
            Debug.Log("Combo is not possible anymore!");
            resetCombo();
            return true;
        }
        //fixme: block collision ignore first
        if (targetBlockCollided)
        {
            Debug.Log("Target Block Collided");
            TriggerReadyForCombo();
            if (readyCombo)
            {
                Debug.Log("Ready for combo");
                ComboInfo comboInfo = CombolFrom(comboLowerBound);
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

    /*
     * @TargetBlockCollided
     * for blocks to call when they collided
     */
    public bool TargetBlockCollided()
    {
        Debug.Log("TargetBlockCollided");
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
    public ComboInfo CombolFrom(int lower_index)
    {
        ComboInfo comboInfo = new ComboInfo();
        Debug.Log("combo start");
        if (lower_index < 0)
        {
            comboInfo.combo_achieved = false;
            return comboInfo;
        }

        int combo_cnt = 0;
        if (GetBlockColorAt(lower_index) != GetBlockColorAt(lower_index + 1))
        {
            Debug.Log("2 edge block has different color");
            Debug.Log(GetBlockColorAt(lower_index));
            Debug.Log(GetBlockColorAt(lower_index + 1));
            Debug.Log("lower_index: " + lower_index);
            Debug.Log("combo fail");
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
        Debug.Log("conbo_cnt: " + combo_cnt + " lower_bound: " + low_bound );
        if (combo_cnt >= mComboBound)
        {
            CombolBlockInRange(low_bound, combo_cnt);
            comboInfo.combo_achieved = true;
            comboInfo.lower_bound = low_bound;
        }
        else
        {
            comboInfo.combo_achieved = false;
        }
        
        
        return comboInfo;

    }
    
    
    #endregion
}