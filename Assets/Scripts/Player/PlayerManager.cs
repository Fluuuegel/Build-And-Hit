using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerManager : MonoBehaviour
{
    public GameObject[] mPlayerPrefab = new GameObject[2];

    public Vector2 mPlayer1InitPos = new Vector2(-3f, 0f);

    public Vector2 mPlayer2InitPos = new Vector2(3f, 0f);

    private GameObject mPlayer1 = null;

    private GameObject mPlayer2 = null;
    public int mHitNum = 0;
    static public CinemachineVirtualCamera mCamera = null;
    static public CinemachineTargetGroup mTargetGroup = null;



    public void InitializePlayer() {

        mCamera = GameObject.Find("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        mTargetGroup = GameObject.Find("TargetGroup").GetComponent<CinemachineTargetGroup>();
        //mPlayerPrefab[0] = Resources.Load<GameObject>($"Prefabs/{Player1Select.player1Select}");
        //mPlayerPrefab[1] = Resources.Load<GameObject>($"Prefabs/{Player2Select.player2Select}");
        //GameObject p1, p2;


        //p1 = GameObject.Instantiate(mPlayerPrefab[0]) as GameObject;
        //p1.name = "Player1";


        //p2 = GameObject.Instantiate(mPlayerPrefab[1]) as GameObject;
        //p2.name = "Player2";

        //mPlayerPrefab[0] = Resources.Load<GameObject>($"Prefabs/{Player1Select.player1Select}");
        //mPlayerPrefab[1] = Resources.Load<GameObject>($"Prefabs/{Player2Select.player2Select}");
        mPlayerPrefab[0] = Resources.Load<GameObject>($"Prefabs/Char1");
        mPlayerPrefab[1] = Resources.Load<GameObject>($"Prefabs/Char1R");
        mPlayer1 = GameObject.Instantiate(mPlayerPrefab[0]) as GameObject;
        mPlayer1.name = "Player1";

        mPlayer1.GetComponent<PlayerBehaviour>().isPlayer1 = true;
        mPlayer2 = GameObject.Instantiate(mPlayerPrefab[1]) as GameObject;
        mPlayer2.GetComponent<PlayerBehaviour>().isPlayer1 = false;
        mPlayer2.name = "Player2";


        mPlayer1.transform.position = mPlayer1InitPos;
        mPlayer2.transform.position = mPlayer2InitPos;

        mTargetGroup.AddMember(mPlayer1.transform, 5f, 5f);
        mTargetGroup.AddMember(mPlayer2.transform, 5f, 5f);
    }

    public void getHitNum(int num) {
        mHitNum = num;
        Debug.Log($"PlayerManager Hit Num: {mHitNum}");
    }

    public GameObject getPlayer1() {
        return mPlayer1;
    }

    public GameObject getPlayer2() {
        return mPlayer2;
    }
    public Vector3 getPlayer1Pos() {
        return mPlayer1InitPos;
    }

    public Vector3 getPlayer2Pos() {
        return mPlayer2InitPos;
    }

    void Start()
    {
        InitializePlayer();
    }

    void Update()
    {
        
    }

}