using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public partial class BlockListManager
{
    public bool AIEnabled { get; private set; } = false;
    private float mBuildPossibility = 0.5f;
    private float mHitPossibility = 1f;
    // private float mSkillPossibility = 0.5f;
    private int[] mAIPlayerIndex = {1};
    private float mAISkillPossibility = 0.5f;

    public bool ActiveAI()
    {
        return AIEnabled = true;
    }
    private bool IsAITurn()
    {
        if(!AIEnabled)
            return false;
        return mAIPlayerIndex.Contains(mPlayerIndex);
    }
    private bool AITriggerBuild()
    {
        if(!AIEnabled)
            return false;
        /*if(IsAITurn())
            return true;*/
        if (!IsAITurn() || !AICanOperate() || !AIEnabled)
        {
            if (!IsAITurn() )
                Debug.Log("AI Build fail! Not his turn!");
            if (!AICanOperate())
                Debug.Log("AI In cool down! ");
            Debug.Log("curplayerindex: " + mPlayerIndex);
            return false;
        }
        if (mBlockManagers[1].GetHeight() == 1)
        {
            return true;
        }
        Debug.Log("Ai Build success!");
        float build = Random.value;
        return build < mBuildPossibility;
        
    }

    private bool AITriggerHit()
    {
        if(!AIEnabled)
            return false;
        if (!IsAITurn() || !AICanOperate() || !AIEnabled)
        {
            if (!IsAITurn() )
                Debug.Log("AI Hit fail! Not his turn!");
            if (!AICanOperate())
                Debug.Log("AI Hit In cool down!");
            return false;
        }
        Debug.Log("Ai Hit success!");
        return Random.value < mHitPossibility;
    }

    private bool AIUseSkill()
    {
        if(!AIEnabled)
            return false;
        if (!IsAITurn() || !AICanOperate() || !AIEnabled)
            return false;
        return Random.value < mAISkillPossibility;
        
    }
    private int AIAttackTarget()
    {
        int height = mBlockManagers[0].GetHeight();
        int vision = curPlayer.VisionRange();
        //return Random.Range(1, vision);
        int bestHitIndex = -1;
        int maxDestroy = 0;
        for(int i = 1; i <= vision; i++)
        {
            if (maxDestroy < CalculateDestroyByHitThisBlock(height - i) && CalculateDestroyByHitThisBlock(height - i) > 1)
            {
                maxDestroy = CalculateDestroyByHitThisBlock(height - i);
                bestHitIndex = i;
            }
            
        }
        if(bestHitIndex != -1)
        {
            Debug.Log("AI Attack Target: " + bestHitIndex + " Destroy: " + maxDestroy + " blocks");
            Debug.Log("NotRandom: ");
            return bestHitIndex;
        }
        else
        {
            bestHitIndex = Random.Range(1, vision);
            Debug.Log("AI Attack Target: " + -1 + " Destroy: " + maxDestroy + " blocks");
            Debug.Log("Vision: " + vision);
            Debug.Log("Random: ");
            return bestHitIndex;
        }
    }
    private float AITimer = 0;
    private void StartAITimer()
    {
        AITimer = Time.time;
    }

    private bool AICanOperate()
    {
        return Time.time - AITimer > 1f;
    }
    private int CalculateDestroyByHitThisBlock(int index)
    {
        int total = 0;
        int lower = index - 1;
        List<int> destroyList = new List<int>();
        BlockManager blockManager = mBlockManagers[0];
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
        while(index > 0 && index < destroyList.Count - 1 && destroyList[index] == destroyList[index + 1])
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
