using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BlockListManager : MonoBehaviour
{
    #region redundant functions
    private void SkillHitFirstBlock()
    {
        mTargetBlockIndex = 1;
        mTargetBlock = mBlockManagers[0].GetBlockAt(mBlockManagers[0].GetHeight() - mTargetBlockIndex);
        //mTargetBlockPos = mTargetBlock.transform.position;

        if (TurnNow())
        {
            GameObject bullet = mBlockManagers[0].GetBlockAt(mBlockManagers[0].GetHeight() - 1);
            mBlockManagers[1].BeingHitBlockDestroy(bullet, mBlockManagers[1].GetHeight() - mTargetBlockIndex);
        }
        else
        {
            GameObject bullet = mBlockManagers[1].GetBlockAt(mBlockManagers[1].GetHeight() - 1);
            mBlockManagers[0].BeingHitBlockDestroy(bullet, mBlockManagers[0].GetHeight() - mTargetBlockIndex);
        }
        mMusic.clip = Resources.Load<AudioClip>("music/Audio_Debuff");
        mMusic.Play();
    }

    private void SkillBuildFirstBlock()
    {
        bool isSameColor = true;
        BlockColor SkillBuildColor = BlockColor.eRed;
        while (isSameColor)
        {
            SkillBuildColor = (BlockColor)Random.Range(0, 3);
            if (SkillBuildColor == mBlockColor)
            {
                isSameColor = true;
                SkillBuildColor = (BlockColor)Random.Range(0, 3);
            }
            else
            {
                isSameColor = false;
            }
        }
        mTargetBlockIndex = 1;
        mBlockManagers[mPlayerIndex].BuildOneBlock(mPlayerIndex, mIsHitState, (int)SkillBuildColor);
        mMusic.clip = Resources.Load<AudioClip>("music/Audio_Buff");
        mMusic.Play();
    }

    private bool TurnNow()
    {
        if (mPlayerIndex == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    #endregion
}
