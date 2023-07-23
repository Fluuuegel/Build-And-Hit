using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices.ComTypes;
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
        eRed,
        eGreen,
        eBlue,
        eSlime,
        eInvalidColour,
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
        //summon a empty object with particle system and play it
        GameObject[] s = new GameObject[4];
        s[0] = Resources.Load<GameObject>("Prefabs/ParticleRed");
        s[1] = Resources.Load<GameObject>("Prefabs/ParticleGreen");
        s[2] = Resources.Load<GameObject>("Prefabs/ParticleBlue");
        s[3] = Resources.Load<GameObject>("Prefabs/ParticleBlue");
        GameObject particle = Instantiate(s[(int)mMyColour], this.transform.position, Quaternion.identity);
        particle.transform.position = this.transform.position;
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        ps.Play();
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

    public void OnCollisionStay2D(Collision2D collision) {
         if (mBlockManager.IsTargetBlock(this.gameObject, collision.gameObject))
        {
            mBlockManager.TargetBlockCollided();
           // Debug.Log("2 target stay collided");
        }
    }

    public bool isColli() { return isCollision; }


}
