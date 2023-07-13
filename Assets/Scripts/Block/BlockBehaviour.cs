using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    private BlockManager mBlockManager = null;

    public void setBlockManager(BlockManager blockManager)
    {
        mBlockManager = blockManager;
    }
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
