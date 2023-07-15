using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Rendering;

public class BlockManager
{
    public const int kInitalBlockCount = 0;
    
    private static GameObject[] mBlockPrefabs = new GameObject[3];
    private List<GameObject> mBlocks = new List<GameObject>();

    public GameObject GetBlockAt(int index)
    {
        if(index >= mBlocks.Count || index < 0)
        {
            Debug.Log("BlockManager::GetBlockAt: index out of range");
            return null;
        }
        return mBlocks[index];
    }
    public BlockBehaviour.BlockColourType GetBlockColorAt(int index)
    {
        if(index >= mBlocks.Count)
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
    public void SetInitPos(Vector2 pos) {
        mPlayerInitPos = pos;
        mBlockInitPos = pos;
        mBlockInitPos.y = 18f;
    }
    
    public BlockManager() {
        InitializeBlocks();
    }

    private void SpawnNewBlock(bool p1Turn, bool isHit, int index, int color = -1, int init = 0)
    //if init == 0, then spawn at the top of the screen
    {   
        GameObject p = GameObject.Instantiate(mBlockPrefabs[color]) as GameObject;
        GameObject p1 = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer1();
        GameObject p2 = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer2();
        Vector3 p1Pos = p1.transform.position;
        Vector3 p2Pos = p2.transform.position;

        BlockBehaviour script = p.GetComponent<BlockBehaviour>();
        SpriteRenderer spriteRenderer = p.GetComponent<SpriteRenderer>();
        SortingGroup sortingGroup = p.GetComponent<SortingGroup>();

        // Set the sorting layer of the block
        if (p1Turn && !isHit) {
            sortingGroup.sortingLayerName = "PlayerCube";
            sortingGroup.sortingOrder = mCurLayerCount;
            p1.transform.position = new Vector3(p1Pos.x, p1Pos.y + 1.0f, 0f);
        }
        else if(!p1Turn && !isHit){
            sortingGroup.sortingLayerName = "EnemyCube";
            sortingGroup.sortingOrder = mCurLayerCount;
            p2.transform.position = new Vector3(p2Pos.x, p2Pos.y + 1.0f, 0f);
        }

        mBlocks.Add(p);

        if (init > 0)
        {   if(p1Turn) { // Change the position of the block
                if(isHit) {
                    p.transform.position = new Vector3(p1Pos.x, p1Pos.y + 1.0f, 0f);
                } else {
                    p.transform.position = new Vector3(p1Pos.x, p1Pos.y - 0.4f, 0f);
                }
            } else {
                if (isHit) {
                    p.transform.position = new Vector3(p2Pos.x, p2Pos.y + 1.0f, 0f);
                } else {
                    p.transform.position = new Vector3(p2Pos.x, p2Pos.y - 0.4f, 0f);
                }
            }
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

    public void InitializeBlocks() {
        mBlockPrefabs[0] = Resources.Load<GameObject>("Prefabs/RedCube");
        mBlockPrefabs[1] = Resources.Load<GameObject>("Prefabs/GreenCube");
        mBlockPrefabs[2] = Resources.Load<GameObject>("Prefabs/BlueCube");
    }

    // Set the color of the block
    public int BuildOneBlock(bool p1Turn, bool isHit, int color = -1)
    {
        TriggerBuild();
        if(mCanBuild == false) {
            return -1;
        }
        SpawnNewBlock(p1Turn, isHit, GetHeight(), color, 1);
        mCanBuild = false;
        return 1;
    }

    public int BuildOneBlock(int color = -1)
    {
        TriggerBuild();
        if(mCanBuild == false) {
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
            return -1;
        }
        GameObject p = mBlocks[index];
        BlockBehaviour BlockScript = p.GetComponent<BlockBehaviour>();
        Debug.Log(GetBlockColorAt(index));
        //delete from list
        mBlocks.RemoveAt(index);
        //delete the instance
        BlockScript.SelfDestroy();
        mCurLayerCount--;
        for(int i = 0; i < mBlocks.Count; i++) {
            SpriteRenderer spriteRenderer = mBlocks[i].GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = i + 1;
            BlockScript = mBlocks[i].GetComponent<BlockBehaviour>();
            BlockScript.SetBlockIndex(i);
        }
        return 1;
    }
    public int GetHeight()
    {
        return mBlocks.Count;
    }

    private float mInterval = 0.3f;
    private void TriggerBuild()
    {
        if(Time.time - mLastTimeBuild > mInterval) {
            mCanBuild = true;
            mLastTimeBuild = Time.time;
        }
        else {
            mCanBuild = false;
        }
    }

    private void SetBlockColor(int color, BlockBehaviour blockBehaviour)
    {
        if (color == 0)
        {
            blockBehaviour.SetBlockColour(BlockBehaviour.BlockColourType.red);
        }
        else if(color == 1)
        {
            blockBehaviour.SetBlockColour(BlockBehaviour.BlockColourType.green);
        }
        else if(color == 2)
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
    
    public void CheckColor(GameObject hitBlock, int index = 0)
    {
        Debug.Log("Check color");
        if(index >= mBlocks.Count || !hitBlock ) {
            return;
        }
        // List<int> toDestroy = new List<int>();
        // toDestroy.Add(index);
        int startDeleteIndex = index;
        int blocksToDestroyCnt = 1;
        if(!BlockManager.SameColor(hitBlock, mBlocks[index]))
        {
            DestroyOneBlock(index);
            return;
        }
        else {
            int temp = index;
            while(index > 0 && SameColor(mBlocks[index], mBlocks[index - 1]))
            {
                index--;
                blocksToDestroyCnt++;
            }

            startDeleteIndex = index;
            index = temp;
            while(index <= mBlocks.Count - 2 && SameColor(mBlocks[index], mBlocks[index + 1]))
            {
                index++;
                blocksToDestroyCnt++;
            }
        }
        for(int i = 0; i < blocksToDestroyCnt; i++)
        {
            DestroyOneBlock(startDeleteIndex);
        }
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
}
