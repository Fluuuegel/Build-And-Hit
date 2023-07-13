using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CubeManager
{
    public const int kInitalCubeCount = 8;

    public GameObject[] mCubePrefabs = new GameObject[3];
    private List<GameObject> mCubes = new List<GameObject>();

    private Vector2 mPlayerInitPos = new Vector2(-2.5f, -1f);

    private int mInitialLayerCount = 0;

    public void InitializemCubes() {
        int randomInt;
        mCubePrefabs[0] = Resources.Load<GameObject>("Prefabs/RedCube");
        mCubePrefabs[1] = Resources.Load<GameObject>("Prefabs/GreenCube");
        mCubePrefabs[2] = Resources.Load<GameObject>("Prefabs/BlueCube");

        for(int i = 0; i < kInitalCubeCount; i++) {
            randomInt = Random.Range(0, 3);
            GameObject p = GameObject.Instantiate(mCubePrefabs[randomInt]) as GameObject;
            p.transform.position = new Vector3(mPlayerInitPos.x, mPlayerInitPos.y + 0.5f * i, 0f);
            SpriteRenderer spriteRenderer = p.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = i;
            mCubes.Add(p);
            mInitialLayerCount++;
        }
    }

    public CubeManager() {
        InitializemCubes();
    }
}
