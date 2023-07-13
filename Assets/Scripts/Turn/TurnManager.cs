using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    GameObject[] player; 
    PlayerBehaviour [] playerBehaviour;
    PlayerBehaviour currentPlayer;

    ////体力值系统应该是角色属性
    //private int[] stamina;//体力值
    //private int[] maxStamina;//满体力
    //private int[] addStaminaRate;//体力增长速率

    public static TurnManager turnManager;
    public static int TurnManagerState;
    private int playerIndex = 0;


    private void Awake()
    {
        playerBehaviour = new PlayerBehaviour[2];
        player = new GameObject[2];
        initSingleton();
    }

    private void Start()//获得两个角色的实例以及PlayerBehaviour组件并锁定角色
    {
        player[0] = GameObject.Find("P1");
        player[1] = GameObject.Find("P2");
        playerBehaviour[0] = player[0].GetComponent<PlayerBehaviour>();
        playerBehaviour[1] = player[1].GetComponent<PlayerBehaviour>();
        //playerBehaviour[0].Lock(); 
        //playerBehaviour[1].Lock();
        
        currentPlayer = playerBehaviour[0];
        
    }

    void initSingleton()
    {
        if (turnManager == null)
            turnManager = this;
        else if (turnManager != this)
            Destroy(gameObject);
    }

    private void Update()
    {
        
    }

    public void StartTurn()
    {

    }

    //public void EndTurn()
    //{
    //    currentPlayer.Lock();
    //    playerIndex = (playerIndex + 1) % 2;
    //    currentPlayer = playerBehaviour[playerIndex];
    //    currentPlayer.unLock();
    //}
}
