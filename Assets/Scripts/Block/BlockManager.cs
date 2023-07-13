using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockManager
{
    public const int kInitalBlockCount = 1;
    
    private static GameObject[] mBlockPrefabs = new GameObject[3];
    private List<GameObject> mBlocks = new List<GameObject>();

    private Vector2 mPlayerInitPos;
    private Vector2 mBlockInitPos;
    private float mLastTimeBuild = 0f;
    public void SetInitPos(Vector2 pos) {
        mPlayerInitPos = pos;
        mBlockInitPos = pos;
        mBlockInitPos.y = 20f;
    }
    private int mCurLayerCount = 0;
    private bool mCanBuild = true;
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
            spriteRenderer.sortingOrder = i;
            mBlocks.Add(p);
            //fixme: set p's manager
            BlockBehaviour script = p.GetComponent<BlockBehaviour>();
            script.setBlockManager(this);
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
        script.setBlockManager(this);
        mLastTimeBuild = Time.time;
        return 1;
    }

    public int DestroyOneBlock(int index = 0)
    {
        if (mBlocks.Count == 0)
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
        for(int i = index; i < mBlocks.Count; i++) {
            SpriteRenderer spriteRenderer = mBlocks[i].GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = i;
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

}
