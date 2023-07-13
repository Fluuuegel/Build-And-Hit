using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    public GameObject mPlayerPrefab;

    public Vector2 mPlayerInitPos = new Vector2(-2.5f, 10f);
    
    public void InitializePlayer() {
        mPlayerPrefab = Resources.Load<GameObject>("Prefabs/Player");
        GameObject p = GameObject.Instantiate(mPlayerPrefab) as GameObject;
        p.transform.position = mPlayerInitPos;
    }

    public PlayerManager() {
        InitializePlayer();
    }

}