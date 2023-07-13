using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockManager
{
    public const int kInitalBlockCount = 8;

    public GameObject[] mBlockPrefabs = new GameObject[3];
    private List<GameObject> mBlocks = new List<GameObject>();

    private Vector2 mPlayerInitPos = new Vector2(-2.5f, -1f);

    private int mInitialLayerCount = 0;

    public void InitializeBlocks() {
        int randomInt;
        mBlockPrefabs[0] = Resources.Load<GameObject>("Prefabs/RedCube");
        mBlockPrefabs[1] = Resources.Load<GameObject>("Prefabs/GreenCube");
        mBlockPrefabs[2] = Resources.Load<GameObject>("Prefabs/BlueCube");

        for(int i = 0; i < kInitalBlockCount; i++) {
            randomInt = Random.Range(0, 3);
            GameObject p = GameObject.Instantiate(mBlockPrefabs[randomInt]) as GameObject;
            p.transform.position = new Vector3(mPlayerInitPos.x, mPlayerInitPos.y + 0.5f * i, 0f);
            SpriteRenderer spriteRenderer = p.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = i;
            mBlocks.Add(p);
            mInitialLayerCount++;
        }
    }

    public BlockManager() {
        InitializeBlocks();
    }
}
