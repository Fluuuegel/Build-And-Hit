using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager 
{
    public GameObject mPlatformPrefab;

    private List<GameObject> mPlatforms = new List<GameObject>();

    private Vector3 mPlayerInitPos = new Vector3(-2.5f, -1.2f, 0f);
    public void setPlayerInitPos(Vector3 pos) {
        mPlayerInitPos = pos;
    }
    public void setPlayerInitPos(int x, int y, int z) {
        mPlayerInitPos = new Vector3(x, y, z);
    }
    private Vector3 mEnemyInitPos = new Vector3(2.5f, 1.0f, 0f);
    public void setEnemyInitPos(Vector3 pos) {
        mEnemyInitPos = pos;
    }
    public void setEnemyInitPos(int x, int y, int z) {
        mEnemyInitPos = new Vector3(x, y, z);
    }
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
