using UnityEngine;

namespace Player
{
    public class PlayerDasher : Player
    {
        private bool GodVision;
        private const int kCD = 2;
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
        public override int VisionRange()
        {
            if (GodVision) {
                GodVision = false;
                return 20;
            }
            return 5;
        }
    }
}