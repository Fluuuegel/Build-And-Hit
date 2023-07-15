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

    public GameObject[] playerUI;
    


    private void Awake()

    {
        playerBehaviour = new PlayerBehaviour[2];
        player = new GameObject[2];
        playerUI = new GameObject[2];
    }

    private void Start()
    {
        player[0] = GameObject.Find("Player1");
        player[1] = GameObject.Find("Player2");
        playerBehaviour[0] = player[0].GetComponent<PlayerBehaviour>();
        playerBehaviour[1] = player[1].GetComponent<PlayerBehaviour>();

        playerUI[0] = GameObject.Find("UIOfPlayer1");
        playerUI[1] = GameObject.Find("UIOfPlayer2");
        
        
        playerBehaviour[1].Lock();
        playerBehaviour[0].unLock();
        playerUI[0].SetActive(true);
        playerUI[1].SetActive(false);
        currentPlayer = playerBehaviour[0];

        
    }


    private void Update()
    {
        if (currentPlayer.isLocked == true)
        {
            TurnChange();
        }
    }

    private void TurnChange()
    {
        playerUI[playerIndex].SetActive(false);
        playerIndex = (playerIndex + 1) % 2;
        currentPlayer = playerBehaviour[playerIndex];
        currentPlayer.unLock();
        playerUI[playerIndex].SetActive(true);
        Debug.Log($"P{playerIndex + 1}, now it's your turn !");
    }
}

