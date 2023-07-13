using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    GameObject[] player; 
    PlayerBehaviour [] playerBehaviour;
    PlayerBehaviour currentPlayer;

    public static TurnManager turnManager;
    public static int TurnManagerState;
    private int playerIndex = 0;


    private void Awake()

    {
        playerBehaviour = new PlayerBehaviour[2];
        player = new GameObject[2];
    }

    private void Start()//获得两个角色的实例以及PlayerBehaviour组件并锁定角�?
    {
        player[0] = GameObject.Find("Player1");
        player[1] = GameObject.Find("Player2");
        playerBehaviour[0] = player[0].GetComponent<PlayerBehaviour>();
        playerBehaviour[1] = player[1].GetComponent<PlayerBehaviour>();
        playerBehaviour[1].Lock();
        playerBehaviour[0].unLock();
        currentPlayer = playerBehaviour[0];
        
    }


    private void Update()
    {
        if (currentPlayer.isLocked == true) {
            playerIndex = (playerIndex + 1) % 2;
            currentPlayer = playerBehaviour[playerIndex];
            currentPlayer.unLock();
        }
    }
}

