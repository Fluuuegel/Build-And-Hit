using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class BlockTest2 : MonoBehaviour
{
    public float HitSpeed = 2f;//ײ���ٶ�

    private bool isHit = false;//�Ƿ�ʼײ��
    private bool isHitIniti = true;//�Ƿ�ײ����ʼ��
    private bool isCollision = false;//�Ƿ���ײ�������
    private int BlockNum;//���ѡ��ķ����
    private int BeHitColor;//���ѡ��(����ײ��)�������ɫ
    private int HitColor;//������з������ɫ

    private BlockManager mTest;//���Block list
    private BlockManager mTestEnemy;//����Block list
    private PlatformManager mPlatformManager;//ƽ̨
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
        KeyControl();//����


        //�����ʼײ��
        if (isHit)
        {
            //���ײ����ʼ��
            if (isHitIniti)
            {
                Debug.Log("isNull: " + mTest.GetBlockAt(2));
                //GameObject p = mTest.GetBlockAt(0);
                //BlockNum = mTest.GetBlockAt(0);//1. ��Player��õ�BlockNum
                //BeHitColor = ...;//2. ����BlockNum����BlockManager���Color=TowerBlock[BlockNum];
                //HitColor = ...;//3. ��BlockManager���Player Block Color
                HitBlock = mTest.GetBlockAt(mTest.GetHeight()-1);//4. ���Player���е�GameObject:HitBlock
                if (HitBlock)
                    HitBlockPos = HitBlock.transform.position;//5. ���HitBlock��λ��
                BeHitBlock = mTestEnemy.GetBlockAt(mTestEnemy.GetHeight()-1);//6. ���Towerָ������ײ��GameObject:BeHitBlock
                if (BeHitBlock)
                    BeHitBlockPos = BeHitBlock.transform.position;//7. ���BeHitBlock��λ��
                isHitIniti = false;//ײ����ʼ��ִֻ��һ��
            }

            //�����δ��ײ��ʱ
            if (!isCollision)
            {
                if(HitBlock && BeHitBlock) 
                    HitBehaviour(HitBlockPos, BeHitBlockPos, HitBlock, BeHitBlock);//8. ִ��Hit���й���
            }
        }
    }

    //Hit�ķ��й���(����Blocks��ʼλ�ã�����GameObject)
    private void HitBehaviour(Vector3 HitBlockPos, Vector3 BeHitBlockPos, GameObject HitBlock, GameObject BeHitBlock)
    {
        time += HitSpeed * Time.smoothDeltaTime;
        Debug.Log("Hit: " + HitBlock.transform.position);
        //�ƶ�λ��
        float x = Mathf.LerpUnclamped(HitBlockPos.x, BeHitBlockPos.x, time);
        float y = Mathf.LerpUnclamped(HitBlockPos.y, BeHitBlockPos.y, time);
        HitBlock.transform.position = new Vector3(x,y,0);
        
        //���HitBlock�ͼ������ײ
        BlockBehaviour HitBlockScript = HitBlock.GetComponent<BlockBehaviour>();
        HitBlockScript.targetCollisionObject = BeHitBlock;
        //Collision2D BeHitColli = BeHitBlock.GetComponent<Collision2D>();
        //HitBlockScript.OnCollisionEnter2D(BeHitColli);
        bool isDestroy = HitBlockScript.isColli();
        if (isDestroy) {
            //mTest.DestroyOneBlock(1);
            //mTestEnemy.DestroyOneBlock(0);
            //yield return new WaitForSeconds(0.5f); Bug: Э�̽ӿڴ���
            //Destroy(HitBlock);//����HitBlock
            mTest.DestroyOneBlock(mTest.GetHeight() - 1);
            //Destroy(BeHitBlock);//����BeHitBlock
            mTestEnemy.DestroyOneBlock(mTestEnemy.GetHeight() - 1);
            
            isCollision = false;//ײ������
            isHit = false;//��ײ��������
            seq += 1;
            time = 0;
        }
    }

    //ִ��Hit����
    public void HitEnemyBlock()
    {
        isHit = true;
    }

    //��������ײ������block����ɫ����
    public Array HitColorState()
    {
        int[] Colors = new int[2];
        Colors[1] = BeHitColor;
        Colors[2] =  HitColor;
        return Colors;
    }

    //��������
    private void KeyControl()
    {
        if (Input.GetKeyDown(KeyCode.Q))//Hero����һ�������ɫblock
        {
            mTest.BuildOneBlock();
        }
        if (Input.GetKeyDown(KeyCode.W))//Enemy����һ�������ɫ��block
        {
            mTestEnemy.BuildOneBlock();
        }
        if (Input.GetKeyDown(KeyCode.A))//����ײ��blockͶ����ȥ
        {
            isHit = true;
            isHitIniti = true;
            isCollision = false;
            //Ĭ��hit��ײ�block
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
