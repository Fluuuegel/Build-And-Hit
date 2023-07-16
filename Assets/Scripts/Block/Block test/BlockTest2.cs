// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEngine;


// public class BlockTest2 : MonoBehaviour
// {
//     public float HitSpeed = 2f;//?˜˜??

//     private bool isHit = false;//???˜˜
//     private bool isHitIniti = true;//??˜˜˜˜?˜˜
//     private bool isCollision = false;//?˜˜?˜˜˜˜˜˜?
//     private int BlockNum;//˜˜?˜˜?˜˜˜˜
//     private int BeHitColor;//˜˜?˜˜(˜˜˜˜?˜˜)˜˜˜˜˜˜?
//     private int HitColor;//˜˜˜˜˜˜ç˜˜˜˜˜?

//     private BlockManager mTest;//˜˜˜Block list
//     private BlockManager mTestEnemy;//˜˜˜˜Block list
//     private PlatformManager mPlatformManager;//??
//     private GameObject HitBlock;//Hit Block
//     private GameObject BeHitBlock;//Be Hit Block
//     private Vector3 HitBlockPos;
//     private Vector3 BeHitBlockPos;
//     private float time;

//     private int seq = 1;

//     // Start is called before the first frame update
//     void Start()
//     {
//         mTest = new BlockManager();
//         mTestEnemy = new BlockManager();
//         mPlatformManager = new PlatformManager();
//         mPlatformManager.setPlayerInitPos(0, 0, 0);
//         mPlatformManager.setEnemyInitPos(2, 2, 0);
//         mPlatformManager.InitializePlatform();
//         Vector2 pos = new Vector2(0, 0);
//         Vector2 enemypos = new Vector2(2, 2);
//         mTest.SetInitPos(pos);
//         mTestEnemy.SetInitPos(enemypos);
//         time = 0;
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         KeyControl();//˜˜˜˜


//         //˜˜˜˜??˜˜
//         if (isHit)
//         {
//             //˜˜?˜˜˜˜?˜˜
//             if (isHitIniti)
//             {
//                 Debug.Log("isNull: " + mTest.GetBlockAt(2));
//                 //GameObject p = mTest.GetBlockAt(0);
//                 //BlockNum = mTest.GetBlockAt(0);//1. ˜˜Player˜˜?˜BlockNum
//                 //BeHitColor = ...;//2. ˜˜˜˜BlockNum˜˜˜˜BlockManager˜˜˜Color=TowerBlock[BlockNum];
//                 //HitColor = ...;//3. ˜˜BlockManager˜˜˜Player Block Color
//                 HitBlock = mTest.GetBlockAt(mTest.GetHeight()-1);//4. ˜˜˜Player˜˜˜å˜GameObject:HitBlock
//                 if (HitBlock)
//                     HitBlockPos = HitBlock.transform.position;//5. ˜˜˜HitBlock˜˜?˜˜
//                 BeHitBlock = mTestEnemy.GetBlockAt(mTestEnemy.GetHeight()-1);//6. ˜˜˜Tower?˜˜˜˜˜˜?˜˜GameObject:BeHitBlock
//                 if (BeHitBlock)
//                     BeHitBlockPos = BeHitBlock.transform.position;//7. ˜˜˜BeHitBlock˜˜?˜˜
//                 isHitIniti = false;//?˜˜˜˜?˜˜??˜˜?˜˜
//             }

//             //˜˜˜˜?˜˜?˜˜?
//             if (!isCollision)
//             {
//                 if(HitBlock && BeHitBlock) 
//                     HitBehaviour(HitBlockPos, BeHitBlockPos, HitBlock, BeHitBlock);//8. ?˜˜Hit˜˜˜é˜˜?
//             }
//         }
//     }

//     //Hit?˜˜é˜˜˜(˜˜˜˜Blocks˜˜???˜˜˜˜˜GameObject)
//     private void HitBehaviour(Vector3 HitBlockPos, Vector3 BeHitBlockPos, GameObject HitBlock, GameObject BeHitBlock)
//     {
//         time += HitSpeed * Time.smoothDeltaTime;
//         Debug.Log("Hit: " + HitBlock.transform.position);
//         //??˜˜
//         float x = Mathf.LerpUnclamped(HitBlockPos.x, BeHitBlockPos.x, time);
//         float y = Mathf.LerpUnclamped(HitBlockPos.y, BeHitBlockPos.y, time);
//         HitBlock.transform.position = new Vector3(x,y,0);
        
//         //˜˜˜HitBlock?˜˜˜˜˜˜?
//         BlockBehaviour HitBlockScript = HitBlock.GetComponent<BlockBehaviour>();
//         HitBlockScript.targetCollisionObject = BeHitBlock;
//         //Collision2D BeHitColli = BeHitBlock.GetComponent<Collision2D>();
//         //HitBlockScript.OnCollisionEnter2D(BeHitColli);
//         bool isDestroy = HitBlockScript.isColli();
//         if (isDestroy) {
//             //mTest.DestroyOneBlock(1);
//             //mTestEnemy.DestroyOneBlock(0);
//             //yield return new WaitForSeconds(0.5f); Bug: Ý˜???˜˜?
//             //Destroy(HitBlock);//˜˜˜˜HitBlock
//             mTest.DestroyOneBlock(mTest.GetHeight() - 1);
//             //Destroy(BeHitBlock);//˜˜˜˜BeHitBlock
//             mTestEnemy.DestroyOneBlock(mTestEnemy.GetHeight() - 1);
            
//             isCollision = false;//?˜˜˜˜˜˜
//             isHit = false;//˜˜?˜˜˜˜˜˜˜˜
//             seq += 1;
//             time = 0;
//         }
//     }

//     //?˜˜Hit˜˜˜˜
//     public void HitEnemyBlock()
//     {
//         isHit = true;
//     }

//     //˜˜˜˜˜˜˜˜?˜˜˜˜˜˜block˜˜˜˜?˜˜˜˜
//     public Array HitColorState()
//     {
//         int[] Colors = new int[2];
//         Colors[1] = BeHitColor;
//         Colors[2] =  HitColor;
//         return Colors;
//     }

//     //˜˜˜˜˜˜˜˜
//     private void KeyControl()
//     {
//         if (Input.GetKeyDown(KeyCode.Q))//Hero˜˜˜˜?˜˜˜˜˜˜?block
//         {
//             mTest.BuildOneBlock();
//         }
//         if (Input.GetKeyDown(KeyCode.W))//Enemy˜˜˜˜?˜˜˜˜˜˜?˜˜block
//         {
//             mTestEnemy.BuildOneBlock();
//         }
//         if (Input.GetKeyDown(KeyCode.A))//˜˜˜˜?˜˜block?˜˜˜˜?
//         {
//             isHit = true;
//             isHitIniti = true;
//             isCollision = false;
//             //?˜˜hit˜˜?˜block
//         }
//         if (Input.GetKeyDown(KeyCode.Z))
//         {
//             mTest.DestroyOneBlock();
//         }
//         if (Input.GetKeyDown(KeyCode.X))
//         {
//             mTestEnemy.DestroyOneBlock();
//         }
//     }

// }
