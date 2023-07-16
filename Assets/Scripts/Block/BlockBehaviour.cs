using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Cryptography;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{

    public GameObject targetCollisionObject;
    public enum BlockColourType
    {
        red,
        green,
        blue,
        invalid_colour,
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
    private bool isCollision = false;
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

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject == targetCollisionObject)
        {
            string collidedObjectName = collision.gameObject.name;
            isCollision = true;
            Debug.Log("Name: " + collidedObjectName);
        }
    }

    public bool isColli() { return isCollision; }
/*    {
        if (isCollision)
        {
            return isCollision;
        }else
        {
            return false;
        }
    }*/

}
