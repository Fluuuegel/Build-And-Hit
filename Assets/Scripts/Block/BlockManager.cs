using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockManager
{
    public const int kInitalBlockCount = 1;
    
    private static GameObject[] mBlockPrefabs = new GameObject[3];
    private List<GameObject> mBlocks = new List<GameObject>();

    public GameObject GetBlockAt(int index)
    {
        return mBlocks[index];
    }
    
    private Vector2 mPlayerInitPos;
    private Vector2 mBlockInitPos;
    private float mLastTimeBuild = 0f;
    private int mCurLayerCount = 1;
    private bool mCanBuild = true;
    public void SetInitPos(Vector2 pos) {
        mPlayerInitPos = pos;
        mBlockInitPos = pos;
        mBlockInitPos.y = 20f;
    }
    
    public BlockManager() {
        InitializeBlocks();
    }
    public void InitializeBlocks() {
        int randomInt;
        mBlockPrefabs[0] = Resources.Load<GameObject>("Prefabs/RedCube");
        mBlockPrefabs[1] = Resources.Load<GameObject>("Prefabs/GreenCube");
        mBlockPrefabs[2] = Resources.Load<GameObject>("Prefabs/BlueCube");

        for(int i = 0; i < kInitalBlockCount; i++) {
            randomInt = Random.Range(0, 3);
            GameObject p = GameObject.Instantiate(mBlockPrefabs[randomInt]) as GameObject;
            p.transform.position = new Vector3(mPlayerInitPos.x, mPlayerInitPos.y + 0.5f * i, 0f);
            SpriteRenderer spriteRenderer = p.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = mCurLayerCount;
            mBlocks.Add(p);
            //fixme: set p's manager
            BlockBehaviour script = p.GetComponent<BlockBehaviour>();
            script.SetBlockManager(this);
            SetBlockColor(randomInt, script);
            mCurLayerCount++;
        }
    }

    //use color to define the block color, -1 means random
    public int BuildOneBlock(int color = -1)
    {
        TriggerBuild();
        if(mCanBuild == false) {
            return -1;
        }
        GameObject p;
        
        if (color == -1)
        {
            int randonInt = Random.Range(0, 3);
            p = GameObject.Instantiate(mBlockPrefabs[randonInt]) as GameObject;
            color = randonInt;
        }
        else
        {
            p = GameObject.Instantiate(mBlockPrefabs[color]) as GameObject;
        }
        p.transform.position = new Vector3(mBlockInitPos.x,mBlockInitPos.y, 0f);
        SpriteRenderer spriteRenderer = p.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = mCurLayerCount;
        mBlocks.Add(p);
        mCurLayerCount++;
        BlockBehaviour script = p.GetComponent<BlockBehaviour>();
        script.SetBlockManager(this);
        SetBlockColor(color, script);
        mLastTimeBuild = Time.time;
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

    private float mInterval = 0.2f;
    private void TriggerBuild()
    {
        if(Time.time - mLastTimeBuild > mInterval) {
            mCanBuild = true;
            
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
            Debug.Log("SetBlockColor: color error!!!");
        }
    } 
}
