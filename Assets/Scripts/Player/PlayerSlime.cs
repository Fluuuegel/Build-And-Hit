using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class PlayerSlime : Player
    {
        private const int kCD = 2;
        public override int GetMaxCD()
        {
            return kCD;
        }
        public override int VisionRange()
        {
            return 5;
        }
        public override void SkillCast(SkillInfo skillInfo)
        {
            if (skillInfo.WillCast)
            {
                //spawn 2 blocks at the top of player's tower without changing the state
                BlockListManager blockListManager = skillInfo.GolbalBlockListManager;
                Debug.Log("Slime Skill Casted");
                blockListManager.ServiceBuildState(false, true);
                Debug.Log("Slime Skill End");
                mTimeUntilNextSkill = kCD;
            }
            else
            {
                return;
            }
        }
    }
}