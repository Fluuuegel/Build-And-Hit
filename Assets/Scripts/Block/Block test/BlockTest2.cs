using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class BlockTest2 : MonoBehaviour
{
    public float HitSpeed = 2f;//撞击速度

    private bool isHit = false;//是否开始撞击
    private bool isHitIniti = true;//是否撞击初始化
    private bool isCollision = false;//是否碰撞到检测体
    private int BlockNum;//玩家选择的方块号
    private int BeHitColor;//玩家选择(即被撞击)方块的颜色
    private int HitColor;//玩家手中方块的颜色

    private BlockManager mTest;//玩家Block list
    private BlockManager mTestEnemy;//敌人Block list
    private PlatformManager mPlatformManager;//平台
    private GameObject HitBlock;//Hit Block
    private GameObject BeHitBlock;//Be Hit Block
    private Vector3 HitBlockPos;
    private Vector3 BeHitBlockPos;
    private float time;

    private int seq = 1;

    // Start is called before the first frame update
    void Start()
    {
        mTest = new BlockManager();
        mTestEnemy = new BlockManager();
        mPlatformManager = new PlatformManager();
        mPlatformManager.setPlayerInitPos(0, 0, 0);
        mPlatformManager.setEnemyInitPos(2, 2, 0);
        mPlatformManager.InitializePlatform();
        Vector2 pos = new Vector2(0, 0);
        Vector2 enemypos = new Vector2(2, 2);
        mTest.SetInitPos(pos);
        mTestEnemy.SetInitPos(enemypos);
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        KeyControl();//按键


        //如果开始撞击
        if (isHit)
        {
            //如果撞击初始化
            if (isHitIniti)
            {
                Debug.Log("isNull: " + mTest.GetBlockAt(2));
                //GameObject p = mTest.GetBlockAt(0);
                //BlockNum = mTest.GetBlockAt(0);//1. 从Player获得的BlockNum
                //BeHitColor = ...;//2. 根据BlockNum，从BlockManager获得Color=TowerBlock[BlockNum];
                //HitColor = ...;//3. 从BlockManager获得Player Block Color
                HitBlock = mTest.GetBlockAt(mTest.GetHeight()-1);//4. 获得Player手中的GameObject:HitBlock
                if (HitBlock)
                    HitBlockPos = HitBlock.transform.position;//5. 获得HitBlock的位置
                BeHitBlock = mTestEnemy.GetBlockAt(mTestEnemy.GetHeight()-1);//6. 获得Tower指定被碰撞的GameObject:BeHitBlock
                if (BeHitBlock)
                    BeHitBlockPos = BeHitBlock.transform.position;//7. 获得BeHitBlock的位置
                isHitIniti = false;//撞击初始化只执行一次
            }

            //如果仍未碰撞到时
            if (!isCollision)
            {
                if(HitBlock && BeHitBlock) 
                    HitBehaviour(HitBlockPos, BeHitBlockPos, HitBlock, BeHitBlock);//8. 执行Hit飞行过程
            }
        }
    }

    //Hit的飞行过程(两个Blocks初始位置，两个GameObject)
    private void HitBehaviour(Vector3 HitBlockPos, Vector3 BeHitBlockPos, GameObject HitBlock, GameObject BeHitBlock)
    {
        time += HitSpeed * Time.smoothDeltaTime;
        Debug.Log("Hit: " + HitBlock.transform.position);
        //移动位置
        float x = Mathf.LerpUnclamped(HitBlockPos.x, BeHitBlockPos.x, time);
        float y = Mathf.LerpUnclamped(HitBlockPos.y, BeHitBlockPos.y, time);
        HitBlock.transform.position = new Vector3(x,y,0);
        
        //如果HitBlock和检测体碰撞
        BlockBehaviour HitBlockScript = HitBlock.GetComponent<BlockBehaviour>();
        HitBlockScript.targetCollisionObject = BeHitBlock;
        //Collision2D BeHitColli = BeHitBlock.GetComponent<Collision2D>();
        //HitBlockScript.OnCollisionEnter2D(BeHitColli);
        bool isDestroy = HitBlockScript.isColli();
        if (isDestroy) {
            //mTest.DestroyOneBlock(1);
            //mTestEnemy.DestroyOneBlock(0);
            //yield return new WaitForSeconds(0.5f); Bug: 协程接口错误
            //Destroy(HitBlock);//销毁HitBlock
            mTest.DestroyOneBlock(mTest.GetHeight() - 1);
            //Destroy(BeHitBlock);//销毁BeHitBlock
            mTestEnemy.DestroyOneBlock(mTestEnemy.GetHeight() - 1);
            
            isCollision = false;//撞击结束
            isHit = false;//碰撞操作结束
            seq += 1;
            time = 0;
        }
    }

    //执行Hit流程
    public void HitEnemyBlock()
    {
        isHit = true;
    }

    //给出参与撞击两个block的颜色代码
    public Array HitColorState()
    {
        int[] Colors = new int[2];
        Colors[1] = BeHitColor;
        Colors[2] =  HitColor;
        return Colors;
    }

    //按键控制
    private void KeyControl()
    {
        if (Input.GetKeyDown(KeyCode.Q))//Hero生成一个随机颜色block
        {
            mTest.BuildOneBlock();
        }
        if (Input.GetKeyDown(KeyCode.W))//Enemy生成一个随机颜色的block
        {
            mTestEnemy.BuildOneBlock();
        }
        if (Input.GetKeyDown(KeyCode.A))//将最底层的block投掷出去
        {
            isHit = true;
            isHitIniti = true;
            isCollision = false;
            //默认hit最底层block
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            mTest.DestroyOneBlock();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            mTestEnemy.DestroyOneBlock();
        }
    }

}
