using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAIObj : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        BlockListManager mBlockListManager = GameObject.Find("GameManager").GetComponent<BlockListManager>();
        mBlockListManager.ActiveAI();
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
