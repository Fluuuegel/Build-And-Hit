using System;
using UnityEngine;using System.Collections;
using System.Collections.Generic;
namespace Block.Block_test
{
    public class BlockTest : MonoBehaviour
    {
        private BlockManager mTest;
        private PlatformManager mPlatformManager;
        public void Start()
        {
            mTest = new BlockManager();
            mPlatformManager = new PlatformManager();
            mPlatformManager.setPlayerInitPos(0, 0, 0);
            mPlatformManager.setEnemyInitPos(10,10,0);
            mPlatformManager.InitializePlatform();
            Vector2 pos = new Vector2(0, 0);
            mTest.SetInitPos(pos);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                mTest.BuildOneBlock();
            }
            if(Input.GetKeyDown(KeyCode.A))
            {
                mTest.DestroyOneBlock();
            }
        }
    }
}