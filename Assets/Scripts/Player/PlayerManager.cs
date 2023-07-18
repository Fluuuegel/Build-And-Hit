using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerManager : MonoBehaviour
{
    public GameObject[] mPlayerPrefab;
    public Vector2[] mPlayerInitPos;
    public Vector2 mPlayer1InitPos = new Vector2(-3f, 0f);
    public Vector2 mPlayer2InitPos = new Vector2(3f, 0f);
    private GameObject[] mPlayers = null;
    private int mPlayerNum = 2;
    static public CinemachineVirtualCamera mCamera = null;
    static public CinemachineTargetGroup mTargetGroup = null;

    public void InitializePlayer() {

        mPlayerPrefab = new GameObject[mPlayerNum];
        mPlayers = new GameObject[mPlayerNum];
        mPlayerInitPos = new Vector2[] { mPlayer1InitPos, mPlayer2InitPos };

        mCamera = GameObject.Find("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        mTargetGroup = GameObject.Find("TargetGroup").GetComponent<CinemachineTargetGroup>();

        // mPlayerPrefab[0] = Resources.Load<GameObject>($"Prefabs/{Player1Select.player1Select}");
        // mPlayerPrefab[1] = Resources.Load<GameObject>($"Prefabs/{Player2Select.player2Select}");

        mPlayerPrefab[0] = Resources.Load<GameObject>($"Prefabs/Char1");
        mPlayerPrefab[1] = Resources.Load<GameObject>($"Prefabs/Char2R");

        for (int i = 0; i < mPlayerNum; i++) {
            mPlayers[i] = GameObject.Instantiate(mPlayerPrefab[i]) as GameObject;
            mPlayers[i].name = "Player" + i;
            mPlayers[i].transform.position = mPlayerInitPos[i];
            mPlayers[i].GetComponent<PlayerBehaviour>().mPlayerIndex = i;
            mTargetGroup.AddMember(mPlayers[i].transform, 5f, 5f);
        }
    }

    public GameObject getPlayer(int index) {
        return mPlayers[index];
    }

    public Vector3 getPlayerPos(int index) {
        return mPlayers[index].transform.position;
    }

    void Start()
    {
        InitializePlayer();
    }

    void Update()
    {
        
    }

}