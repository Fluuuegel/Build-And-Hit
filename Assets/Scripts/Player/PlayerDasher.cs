using UnityEngine;

namespace Player
{
    public class PlayerDasher : Player
    {
        private int GodVisionLastRound = 0;
        public override void SkillCast(SkillInfo skillInfo)
        {
            Debug.Log("Dasher Skill Casted But no Implemented");
            GodVisionLastRound = 2;
        }
        public override int VisionRange()
        {
            if (GodVisionLastRound > 0)
                return 20;
            return 5;
        }
        public override void ExtendedRefreshRound()
        {
            if (GodVisionLastRound > 0)
                GodVisionLastRound--;
        }
    }
}