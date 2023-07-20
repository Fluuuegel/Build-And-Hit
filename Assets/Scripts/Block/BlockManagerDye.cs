using UnityEngine;
using UnityEngine.Rendering;
using BlockColor = BlockBehaviour.BlockColourType;
public partial class BlockManager
    {
        public BlockColor DyeBlock(GameObject Block, BlockColor color)
        {
            GameObject newBlock = GameObject.Instantiate(mBlockPrefabs[(int)color]) as GameObject;
            BlockBehaviour newScript = newBlock.GetComponent<BlockBehaviour>();
            BlockBehaviour oldScript = Block.GetComponent<BlockBehaviour>();
            newBlock.transform.position = Block.transform.position;
            newScript.SetBlockColour(color);
            newScript.SetBlockIndex(oldScript.GetBlockIndex());
            newScript.SetBlockManager(this);
            mBlocks[oldScript.GetBlockIndex()] = newBlock;
            
            PlayerManager.mTargetGroup.AddMember(newBlock.transform, 1f, 3f);
            
            //particles 
            BlockBehaviour script = newBlock.GetComponent<BlockBehaviour>();
            script.mParticle = script.GetComponent<ParticleSystem>();
            script.mParticle.Play();
            //sorting layers
            SpriteRenderer spriteRenderer = newBlock.GetComponent<SpriteRenderer>();
            SortingGroup newBlockSortingGroup = newBlock.GetComponent<SortingGroup>();
            SortingGroup oldBlockSortingGroup = Block.GetComponent<SortingGroup>();
            newBlockSortingGroup.sortingLayerName = oldBlockSortingGroup.sortingLayerName;
            newBlockSortingGroup.sortingOrder = oldBlockSortingGroup.sortingOrder;
            
            GameObject.Destroy(Block);
            mBlocks[oldScript.GetBlockIndex()] = newBlock;
            return color;
        }

        public BlockColor DyeBlock(int index, BlockColor color)
        {
            if (index < 0 || index >= mBlocks.Count)
            {
                return BlockColor.eInvalidColour;
            }
            return DyeBlock(mBlocks[index], color);
        }

        public void test_Dye()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                int color = Random.Range(0, 3);
                DyeBlock(3, (BlockColor)color);
            }
                
        }
    }
/*
 * ref:
 * private void SpawnNewBlock(int playerIndex, bool isHit, int index, int color = -1, bool init = false)
    {

        GameObject p = GameObject.Instantiate(mBlockPrefabs[color]) as GameObject;
        PlayerManager.mTargetGroup.AddMember(p.transform, 1f, 3f);

        GameObject p1 = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer(0);
        GameObject p2 = GameManager.sTheGlobalBehavior.GetPlayerManager().getPlayer(1);
        Vector3 p1Pos = p1.transform.position;
        Vector3 p2Pos = p2.transform.position;
        BlockBehaviour script = p.GetComponent<BlockBehaviour>();
        SpriteRenderer spriteRenderer = p.GetComponent<SpriteRenderer>();
        SortingGroup blockSortingGroup = p.GetComponent<SortingGroup>();

        // Set the sorting layer of the block
        // Set the position of the players in game
    
        mBlocks.Add(p);

        // Set block
        if(playerIndex == 0) { // Change the position of the block
            if(isHit) {
                p.transform.position = new Vector3(p1Pos.x, p1Pos.y + 1.0f, 0f);
            } else {
                p.transform.position = new Vector3(p1Pos.x, p1Pos.y - 0.1f, 0f);
                script.mParticle = script.GetComponent<ParticleSystem>();
                script.mParticle.Play();
            }
        } else {
            if (isHit) {
                p.transform.position = new Vector3(p2Pos.x, p2Pos.y + 1.0f, 0f);
            } else {
                p.transform.position = new Vector3(p2Pos.x, p2Pos.y - 0.1f, 0f);
                script.mParticle = script.GetComponent<ParticleSystem>();
                script.mParticle.Play();
            }
        }

        // Set sorting layer of the block
        
        if (playerIndex == 0 && !isHit) {
            blockSortingGroup.sortingLayerName = "PlayerCube";
            blockSortingGroup.sortingOrder = mCurLayerCount;
            p1.transform.position = new Vector3(p1Pos.x, p1Pos.y + 1.0f, 0f);
        }
        else if (playerIndex == 1 && !isHit) {
            blockSortingGroup.sortingLayerName = "EnemyCube";
            blockSortingGroup.sortingOrder = mCurLayerCount;
            p2.transform.position = new Vector3(p2Pos.x, p2Pos.y + 1.0f, 0f);
        }

        spriteRenderer.sortingOrder = mCurLayerCount;
        script.SetBlockManager(this);
        script.SetBlockIndex(index);
        SetBlockColor(color, script);
        mCurLayerCount++;
    }
 */