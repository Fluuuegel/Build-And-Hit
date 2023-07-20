using UnityEngine;

namespace Player
{
    public class PlayerDasher : Player
    {
        private bool GodVision;
        private const int kCD = 2;

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
                Debug.Log("GodVision");
                GodVision = false;
                return 20;
            }
            Debug.Log("NormalVision");
            return 5;
        }
    }
}