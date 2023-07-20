using UnityEngine;

namespace Player
{
    public class PlayerDasher : Player
    {
        private bool GodVision;
        private int CoolDown = 0;
        public override bool CanCastSkill()
        {
            return CoolDown == 0;
        }

        public override void SkillCast(SkillInfo skillInfo)
        {
            Debug.Log("Dasher Skill Casted But no Implemented");
            GodVision = true;
            CoolDown = 1;
        }
        public override int VisionRange()
        {
            if (GodVision)
                return 20;
            return 5;
        }
        public override void ExtendedRefreshRound()
        {
            if (GodVision)
                GodVision = false;
            if(CoolDown > 0)
                CoolDown--;
        }
    }
}