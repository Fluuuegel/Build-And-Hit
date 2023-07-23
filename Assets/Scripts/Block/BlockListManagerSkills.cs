using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockColor = BlockBehaviour.BlockColourType;
public partial class BlockListManager : MonoBehaviour
{
    #region redundant functions
    private void SkillHitFirstBlock()
    {
        if (TurnNow())
        {
            GameObject bullet = mBlockManagers[1].GetBlockAt(mBlockManagers[1].GetHeight() - 1);
            mBlockManagers[1].DestroyOneBlock(mBlockManagers[1].GetHeight() - 1);
        }
        else
        {
            GameObject bullet = mBlockManagers[0].GetBlockAt(mBlockManagers[1].GetHeight() - 1);
            mBlockManagers[0].DestroyOneBlock(mBlockManagers[0].GetHeight() - 1);
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

    private void SkillChangeFirstBlock()
    {
        bool isSameColor = true;
        BlockColor SkillChangeColor = BlockColor.eRed;
        BlockColor ChangeColor = BlockColor.eRed;
        mTargetBlockIndex = 1;
        if (TurnNow())
        {
            GameObject bullet = mBlockManagers[0].GetBlockAt(mBlockManagers[0].GetHeight() - 1);
            SkillChangeColor = mBlockManagers[0].GetBlockColorAt(mBlockManagers[0].GetHeight() - 1);
            while(isSameColor)
            {
                ChangeColor = (BlockColor)Random.Range(0, 3);
                if (ChangeColor == SkillChangeColor)
                { isSameColor = true; }
                else { isSameColor = false; }
            }
            mBlockManagers[0].DestroyOneBlock(mBlockManagers[0].GetHeight() - 1);
        }
        else
        {
            GameObject bullet = mBlockManagers[1].GetBlockAt(mBlockManagers[1].GetHeight() - 1);
            SkillChangeColor = mBlockManagers[1].GetBlockColorAt(mBlockManagers[1].GetHeight() - 1);
            while(isSameColor)
            {
                ChangeColor = (BlockColor)Random.Range(0, 3);
                if (ChangeColor == SkillChangeColor)
                { isSameColor = true; }
                else { isSameColor = false; }
            }
            mBlockManagers[1].DestroyOneBlock(mBlockManagers[1].GetHeight() - 1);
        }

        mTargetBlockIndex = 1;
        mBlockManagers[mPlayerIndex].BuildOneBlock(mPlayerIndex, mIsHitState, (int)ChangeColor);
    }
    #endregion

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

    public static string[] SkillDes =
    {
        "You got a skills in this turn!\n <b><size=22><color=green>Destroy your opponent's first block!</size></color></b>",
        "You got a skills in this turn!\n <b><size=22><color=green>Build a block with different color!</size></color></b>",
        "You got a skills in this turn!\n <b><size=22><color=green>Change the first block's color!</size></color></b>"
    };

    private void BalanceProb()
    {
        if (TurnNow())
        {
            if (mBlockManagers[0].GetHeight() < mBlockManagers[1].GetHeight())
            {
                GettingProb = 0.3f + (mBlockManagers[1].GetHeight() - mBlockManagers[0].GetHeight()) * 0.05f;
                if (GettingProb > 1f)
                { GettingProb = 1f; }
            }
            else { GettingProb = 0.3f; }
        }
        else
        {
            if (mBlockManagers[0].GetHeight() > mBlockManagers[1].GetHeight())
            {
                GettingProb = 0.3f + (mBlockManagers[0].GetHeight() - mBlockManagers[1].GetHeight()) * 0.05f;
                if (GettingProb > 1f)
                { GettingProb = 1f; }
            }
            else { GettingProb = 0.3f; }
        }
    }


}
