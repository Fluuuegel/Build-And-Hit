using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerDasher : Player
    {
        private bool GodVision;
        private const int kCD = 4;
        public override int GetMaxCD()
        {
            return kCD;
        }
        public override void SkillCast(SkillInfo skillInfo)
        {
            if (skillInfo.WillCast) {
                GodVision = true;
                mTimeUntilNextSkill = kCD;
            } 
        }
        public override void ExtendedRefreshRound()
        {
            if(GodVision)
            {
                GodVision = false;
            }
        }
        public override int VisionRange()
        {
            if (GodVision) {
                return 8;
            }
            return 4;
        }
    }
}