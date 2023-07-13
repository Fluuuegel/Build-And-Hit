using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Cryptography;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
    
    public enum BlockColourType
    {
        red,
        green,
        blue
    }
    private BlockManager mBlockManager = null;
    public BlockColourType mMyColour;
    private int mIndex;
    public void SetBlockManager(BlockManager blockManager)
    {
        mBlockManager = blockManager;
    }
    public BlockManager GetBlockManager()
    {
        return mBlockManager;
    }
    public void SetBlockColour(BlockColourType colour)
    {
        mMyColour = colour;
    }
    public BlockColourType GetBlockColour()
    {
        return mMyColour;
    }
    public void SetBlockIndex(int index)
    {
        mIndex = index;
    }
    public int GetBlockIndex()
    {
        return mIndex;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelfDestroy()
    {
        Destroy(this.gameObject);
    }
    
    
}
