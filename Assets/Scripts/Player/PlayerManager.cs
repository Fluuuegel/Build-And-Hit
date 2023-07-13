using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject[] mPlayerPrefab = new GameObject[2];

    public Vector2 mPlayer1InitPos = new Vector2(-2.5f, 10f);

    public Vector2 mPlayer2InitPos = new Vector2(2.5f, 10f);

    private bool isPlayer1 = true;

    private bool isPlayer2 = true;

    public void InitializePlayer() {
        mPlayerPrefab[0] = Resources.Load<GameObject>("Prefabs/Player1");
        mPlayerPrefab[1] = Resources.Load<GameObject>("Prefabs/Player2");
        GameObject p1, p2;
        if(isPlayer1) {
            p1 = GameObject.Instantiate(mPlayerPrefab[0]) as GameObject;
            p1.name = "Player1";
        }
        else { 
            p1 = GameObject.Instantiate(mPlayerPrefab[1]) as GameObject;
            p1.name = "Player1";
        }
        if(isPlayer2) {
            p2 = GameObject.Instantiate(mPlayerPrefab[0]) as GameObject;
            p2.name = "Player2";
        }
        else {
            p2 = GameObject.Instantiate(mPlayerPrefab[1]) as GameObject;
            p2.name = "Player2";
        }
        p1.transform.position = mPlayer1InitPos;
        p2.transform.position = mPlayer2InitPos;
    }

    void Start()
    {
        InitializePlayer();
    }

    void Update()
    {
        
    }

}