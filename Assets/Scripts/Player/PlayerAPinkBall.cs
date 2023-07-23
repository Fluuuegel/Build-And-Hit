using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class PlayerAPinkBall : Player
    {
        private int kCD = 2;

        private bool mCanRelease = false;

        private BlockBehaviour.BlockColourType mColour = BlockBehaviour.BlockColourType.eRed;
        public override int GetMaxCD()
        {
            return kCD;
        }
        public override void SetColor(BlockBehaviour.BlockColourType color)
        {
            mColour = color;
        }
        public override void Recover()
        {
            mCanRelease = false;
        }
        public override void SkillCast(SkillInfo skillInfo)
        {
            if (skillInfo.WillCast)
            {
                if (mCanRelease)
                {
                    // TODO: Release
                    Debug.Log("Release");
                    mAnimator.SetBool("Suck", false);
                    mAnimator.SetBool("Released", true);
                    BlockListManager blockListManager = skillInfo.GolbalBlockListManager;
                    blockListManager.ServiceBuildState(false, false, (int)mColour);
                    mCanRelease = false;
                    mTimeUntilNextSkill = kCD;
                }
                else
                {
                    // TODO: Suck
                    Debug.Log("Suck");
                    mAnimator.SetBool("Released", false);
                    mCanRelease = true;
                    mTimeUntilNextSkill = 0;
                }
            }
            else
            {
                return;
            }
        }
    }
}