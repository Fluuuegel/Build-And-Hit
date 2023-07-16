using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Rendering;
using Cinemachine;
using Unity.VisualScripting;

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


    public CinemachineImpulseSource impulseSource;

    private void Start()
    {
        impulseSource = GameObject.Find("VirtualCamera").GetComponent<CinemachineImpulseSource>();
    }
    public void SetInitPos(Vector2 pos) {

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

        GameObject p = GameObject.Instantiate(mBlockPrefabs[color]) as GameObject;
        PlayerManager.mTargetGroup.AddMember(p.transform, 1f, 3f);

        GameObject p1 = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer1();
        GameObject p2 = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer2();
        Vector3 p1Pos = p1.transform.position;
        Vector3 p2Pos = p2.transform.position;
        BlockBehaviour script = p.GetComponent<BlockBehaviour>();
        SpriteRenderer spriteRenderer = p.GetComponent<SpriteRenderer>();
        SortingGroup blockSortingGroup = p.GetComponent<SortingGroup>();

        // Set the sorting layer of the block
        // Set the position of the players in game
        

        mBlocks.Add(p);
        /*--------------set new block----------------*/
        if (init > 0)
        {   if(p1Turn) { // Change the position of the block
                if(isHit) {
                    p.transform.position = new Vector3(p1Pos.x, p1Pos.y + 1.0f, 0f);
                } else {
                    p.transform.position = new Vector3(p1Pos.x, p1Pos.y - 0.4f, 0f);
                    script.mParticle = script.GetComponent<ParticleSystem>();
                    script.mParticle.Play();
                }
            } else {
                if (isHit) {
                    p.transform.position = new Vector3(p2Pos.x, p2Pos.y + 1.0f, 0f);
                } else {
                    p.transform.position = new Vector3(p2Pos.x, p2Pos.y - 0.4f, 0f);
                    script.mParticle = script.GetComponent<ParticleSystem>();
                    script.mParticle.Play();
                }
            }
        }
        else//now is the initialization process
        {
            p.transform.position = new Vector3(mBlockInitPos.x, SpawnYAxis + 0.5f * GetHeight(), 0f);
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
        PlayerManager.mTargetGroup.AddMember(p.transform, 1f, 3f);
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
        test_ExecuteCombol(startDeleteIndex - 1);
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
        public int combo_cnt;
    }

    public bool test_ExecuteCombol(int lower_index)
    {
        Debug.Log("combo start");
        if (lower_index < 0)
        {
            return false;
        }

        int combo_cnt = 0;
        if (GetBlockColorAt(lower_index) != GetBlockColorAt(lower_index + 1))
        {
            Debug.Log("2 edge block has different color");
            return false;
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
            test_ExecuteCombol(low_bound);
        }
        string combo_info = "combo_cnt: " + combo_cnt + " lower_bound: " + low_bound + "color: " + GetBlockColorAt(low_bound);
        Debug.Log(combo_info);
        return true;

    }
    
    
    #endregion
}