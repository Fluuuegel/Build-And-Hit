using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Cryptography;
using UnityEngine;
using DG.Tweening;

/* TODO:

    If u want to achieve particle effect when block is destroyed, use this script.
    And u need to change a lot of things in BlockManager.cs. and add 'GreenCubeObj', 'RedCubeObj' prefabs in Resources folder.
    U can refer to 'BlueCubeObj' prefab.
    
*/
public class BlockObjBehaviour : MonoBehaviour
{

    public ParticleSystem mParticle = null;

    public GameObject BlueCube;
    public Material _material = null;
    public GameObject targetCollisionObject;
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

    {   _material = GetComponent<Renderer>().material;
        mParticle = GetComponent<ParticleSystem>();
        _material.DOFloat(10, "_Strength", 0.2f).OnComplete(() => {
            Destroy(this.gameObject);
            mParticle.Play();
        });
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

}
