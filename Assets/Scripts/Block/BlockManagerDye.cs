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
    }