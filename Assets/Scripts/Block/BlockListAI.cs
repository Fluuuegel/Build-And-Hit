using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public partial class BlockListManager
{
    public bool AIEnabled { get; private set; } = false;
    private float mBuildPossibility = 0.5f;
    private float mHitPossibility = 0.5f;
    private float mSkillPossibility = 0.5f;
    private int[] mAIPlayerIndex = {1};

    private bool ActiveAI()
    {
        return AIEnabled = true;
    }
    private bool IsAITurn()
    {
        return mAIPlayerIndex.Contains(mPlayerIndex);
    }
    private bool AITriggerBuild()
    {
        return Random.value < mBuildPossibility;
    }

    private bool AITriggerHit()
    {
        return Random.value < mHitPossibility;
    }

    private bool AIUseSkill()
    {
        return Random.value < 0.5f;
    }
    private int AIAttackTarget()
    {
        int height = mBlockManagers[1 - mPlayerIndex].GetHeight();
        int vision = curPlayer.VisionRange();
        int BestHit = -1;
        int maxDestroy = 0;
        for(int i = 1; i <= vision; i++)
        {
            if (maxDestroy < CalculateDestroyByHitThisBlock(height - i))
            {
                maxDestroy = CalculateDestroyByHitThisBlock(height - i);
                BestHit = i;
            }
            
        }
        if(BestHit != -1)
        {
            return BestHit;
        }
        else
        {
            return Random.Range(0, vision);
        }
    }

    private int CalculateDestroyByHitThisBlock(int index)
    {
        int total = 1;
        int lower = index - 1;
        List<int> destroyList = new List<int>();
        BlockManager blockManager = mBlockManagers[1 - mPlayerIndex];
        for (int i = 0; i < blockManager.GetHeight(); i++)
        {
            destroyList.Add((int)blockManager.GetBlockColorAt(i));
        }
        //calculate distinct destroy list
        int temp = index;
        while (index > 0 && destroyList[index] == destroyList[index - 1])
        {
            index--;
            total++;
        }
        int LowerBound = index;
        index = temp;
        while(index < destroyList.Count - 1 && destroyList[index] == destroyList[index + 1])
        {
            index++;
            total++;
        }
        for (int i = 0; i < total; i++)
        {
            destroyList.RemoveAt(LowerBound);
        }
        //calculate cobol destroy
        int ComboLower = LowerBound, ComboUpper = LowerBound + 1;
        int ComboTotal = 0;
        while (ComboLower >= 0 && ComboUpper < destroyList.Count && destroyList[ComboLower] == destroyList[ComboUpper])
        {
            int ComboInThisRound = 2;
            while (ComboLower > 0 && destroyList[ComboLower] == destroyList[ComboLower - 1])
            {
                ComboLower--;
                ComboInThisRound++;
            }
            while (ComboUpper <= destroyList.Count - 2 && destroyList[ComboUpper] == destroyList[ComboUpper + 1])
            {
                ComboUpper++;
                ComboInThisRound++;
            }

            if (ComboInThisRound < 3)
            {
                break;// not enough to combo, end of all combo
            }
            ComboTotal += ComboInThisRound;
        }
        if(ComboTotal > 2)
        {
            total += ComboTotal;
        }
        return total;
    }
}
