using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public partial class BlockListManager
{
    public bool AIEnabled { get; private set; } = false;
    private float mBuildPossibility = 0.7f;
    private float mHitPossibility = 0.4f;
    private float mSkillPossibility = 0.5f;
    private int[] mAIPlayerIndex = {1};

    public bool ActiveAI()
    {
        return AIEnabled = true;
    }
    private bool IsAITurn()
    {
        return mAIPlayerIndex.Contains(mPlayerIndex);
    }
    private bool AITriggerBuild()
    {
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
        Debug.Log("Ai Build success!");
        float build = Random.value;
        return build < mBuildPossibility;
        StartAITimer();
    }

    private bool AITriggerHit()
    {
        if (!IsAITurn() || !AICanOperate() || ! AIEnabled)
        {
            if (!IsAITurn() )
                Debug.Log("AI Hit fail! Not his turn!");
            if (!AICanOperate())
                Debug.Log("AI Hit In cool down!");
            return false;
        }

        if (mBlockManagers[1].GetHeight() == 1)
        {
            return true;
        }
        return Random.value < mHitPossibility;
        StartAITimer();
    }

    private bool AIUseSkill()
    {
        if (!IsAITurn() || !AICanOperate())
            return false;
        return Random.value < 1.0f;
        StartAITimer();
    }
    private int AIAttackTarget()
    {
        int height = mBlockManagers[0].GetHeight();
        int vision = curPlayer.VisionRange();
        //return Random.Range(1, vision);
        int BestHit = -1;
        int maxDestroy = 0;
        for(int i = 1; i <= vision; i++)
        {
            if (maxDestroy < CalculateDestroyByHitThisBlock(height - i) && CalculateDestroyByHitThisBlock(height - i) > 2)
            {
                maxDestroy = CalculateDestroyByHitThisBlock(height - i);
                BestHit = i;
            }
            
        }
        if(BestHit != -1)
        {
            Debug.Log("AI Attack Target: " + BestHit + " Destroy: " + maxDestroy + " blocks");
            Debug.Log("NotRandom: ");
            return BestHit;
        }
        else
        {
            BestHit = Random.Range(1, vision);
            Debug.Log("AI Attack Target: " + -1 + " Destroy: " + maxDestroy + " blocks");
            Debug.Log("Vision: " + vision);
            Debug.Log("Random: ");
            return BestHit;
        }
    }
    private float AITimer = 0;
    private void StartAITimer()
    {
        AITimer = Time.time;
    }

    private bool AICanOperate()
    {
        return Time.time - AITimer > 0.5f;
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
