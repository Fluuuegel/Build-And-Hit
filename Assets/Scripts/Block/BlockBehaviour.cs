using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class BlockBehaviour : MonoBehaviour
{
    public ParticleSystem mParticle = null;
    public Material _material = null;
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
    public int mIndex;
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

    void Start()
    {   
    }

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
            //Debug.Log("Name: " + collidedObjectName);
        }

        if (mBlockManager.IsTargetBlock(this.gameObject, collision.gameObject))
        {
            mBlockManager.TargetBlockCollided();
            //Debug.Log("2 target collided");
        }
        
    }

    public bool isColli() { return isCollision; }


}
