using System.Linq;
using UnityEngine;

public partial class BlockListManager
{
    public bool AIEnabled { get; private set; } = false;
    private float mBuildPossibility = 0.5f;
    private float mHitPossibility = 0.5f;
    private float mSkillPossibility = 0.5f;
    private int[] mAIPlayerIndex = {1};

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
        int vision = curPlayer.VisionRange();
        return Random.Range(0, vision);
    }
}
