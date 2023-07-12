using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : SingletonMonobehaviour<PlayerBehaviour>
{
    public GameObject mPlayerPrefab = new GameObject();

    public Vector2 mPlayerInitPos = new Vector2(-2.5f, 10f);

    public PlayerBehaviour() {
        mPlayerPrefab = Resources.Load<GameObject>("Prefabs/Player");
    }
    
    void Start() {
        mPlayerPrefab = Resources.Load<GameObject>("Prefabs/Player");
        GameObject p = GameObject.Instantiate(mPlayerPrefab) as GameObject;
        p.transform.position = mPlayerInitPos;
    }

    void Update()
    {
        
    }
}
