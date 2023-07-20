using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Rendering;
using Cinemachine;
using Unity.VisualScripting;

public partial class BlockManager
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
    private Vector2 mPlayerInitPos;
    private Vector2 mBlockInitPos;
    private int mCurLayerCount = 1;
    
    //for last stand
    bool mCanTriggerLastBuild = true;
    private bool mFatalLow = false;
    public BlockManager()
    {

        InitializeBlocks();
    }
    public void RefreshRound()
    {
        if (GetHeight() == 1)
        {
            mFatalLow = true;
        }
        else
        {
            mFatalLow = false;
        }
        if(mImmuneRound > 0)
        {
            mImmuneRound--;
        }
    }

    public bool LastStand()
    {
        return mFatalLow && mCanTriggerLastBuild;
    }

    public void LastStandUI()
    {
        if (LastStand())
        {
            
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

    public bool PowerfulBuildWhenLow()
    {
        return mFatalLow;
    }
    public BlockBehaviour.BlockColourType GetBlockColorAt(int index)
    {
        if (index >= mBlocks.Count)
        {
            return BlockBehaviour.BlockColourType.eRed;
        }

        GameObject p = mBlocks[index];
        BlockBehaviour script = p.GetComponent<BlockBehaviour>();
        return script.GetBlockColour();
    }
    

    public void SetInitPos(Vector2 pos) {

        mPlayerInitPos = pos;
        mBlockInitPos = pos;
        mBlockInitPos.y = 18f;
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
        // string msg = "block colour " + color + " ";
        // Debug.Log(msg);
        SpawnNewBlock(playerIndex, isHit, GetHeight(), color, init);
        if(!isHit && mFatalLow && mCanTriggerLastBuild)
        {
            mCanTriggerLastBuild = false;
            SpawnNewBlock(playerIndex, isHit, GetHeight(), color, init);
        }
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
            blockBehaviour.SetBlockColour(BlockBehaviour.BlockColourType.eRed);
        }
        else if (color == 1)
        {
            blockBehaviour.SetBlockColour(BlockBehaviour.BlockColourType.eGreen);
        }
        else if (color == 2)
        {
            blockBehaviour.SetBlockColour(BlockBehaviour.BlockColourType.eBlue);
        }
        else if (color == 3) {
            blockBehaviour.SetBlockColour(BlockBehaviour.BlockColourType.eSlime);
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
    
    

    #region Block select
    public GameObject SelectBlockTemp(int index ){
        return null;
    }
    #endregion
}