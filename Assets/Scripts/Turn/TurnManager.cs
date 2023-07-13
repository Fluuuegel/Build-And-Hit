using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    GameObject[] player; 
    PlayerBehaviour [] playerBehaviour;
    PlayerBehaviour currentPlayer;

    ////����ֵϵͳӦ���ǽ�ɫ����
    //private int[] stamina;//����ֵ
    //private int[] maxStamina;//������
    //private int[] addStaminaRate;//������������

    public static TurnManager turnManager;
    public static int TurnManagerState;
    private int playerIndex = 0;


    private void Awake()
    {
        playerBehaviour = new PlayerBehaviour[2];
        player = new GameObject[2];
        initSingleton();
    }

    private void Start()//���������ɫ��ʵ���Լ�PlayerBehaviour�����������ɫ
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
