using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager 
{
    public GameObject mPlatformPrefab = new GameObject();

    private List<GameObject> mPlatforms = new List<GameObject>();

    private Vector3 mPlayerInitPos = new Vector3(-2.5f, -1.2f, 0f);

    private Vector3 mEnemyInitPos = new Vector3(2.5f, 1.0f, 0f);

    public void InitializePlatform() {
        mPlatformPrefab = Resources.Load<GameObject>("Prefabs/PlatformSmall");
        GameObject p1 = GameObject.Instantiate(mPlatformPrefab) as GameObject;
        GameObject p2 = GameObject.Instantiate(mPlatformPrefab) as GameObject;
        p1.transform.position = mPlayerInitPos;
        mPlatforms.Add(p1);
        p2.transform.position = mEnemyInitPos;
        mPlatforms.Add(p2);
    }

    public PlatformManager() {
        InitializePlatform();
    }
}
