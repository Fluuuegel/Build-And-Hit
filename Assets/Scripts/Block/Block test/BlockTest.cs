using System;
using UnityEngine;using System.Collections;
using System.Collections.Generic;
namespace Block.Block_test
{
    public class BlockTest : MonoBehaviour
    {
        private BlockManager mTest;
        private BlockManager mTestEnemy;
        private PlatformManager mPlatformManager;
        public void Start()
        {
            mTest = new BlockManager();
            mTestEnemy = new BlockManager();
            mPlatformManager = new PlatformManager();
            mPlatformManager.setPlayerInitPos(0, 0, 0);
            mPlatformManager.setEnemyInitPos(2,2,0);
            mPlatformManager.InitializePlatform();
            Vector2 pos = new Vector2(0, 0);
            Vector2 enemypos = new Vector2(2, 2);
            mTest.SetInitPos(pos);
            mTestEnemy.SetInitPos(enemypos);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                mTest.BuildOneBlock();
            }
            if(Input.GetKeyDown(KeyCode.E))
            {
                mTest.DestroyOneBlock();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                mTestEnemy.BuildOneBlock();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                mTestEnemy.DestroyOneBlock(1);
            }
        }
    }
}