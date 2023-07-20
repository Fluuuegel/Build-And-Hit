using Unity.VisualScripting;
using UnityEngine;

namespace Player
{   
    public class PlayerEngineer : Player
    {
        private const int kCoolDown = 2;

        private int mTimeUntilNextSkill = 0;

        public override bool CanCastSkill()
        {
            return (mTimeUntilNextSkill == 0);
        }

        public override void IncreaseTimeUntilNextSkill()
        {
            Debug.Log("Engineer IncreaseTimeUntilNextSkill: " + mTimeUntilNextSkill);
            if(mTimeUntilNextSkill > 0)
            {
                mTimeUntilNextSkill--;
            }
        }

        public override void SkillCast(SkillInfo skillInfo)
        {
            if (skillInfo.WillCast)
            {
                //spawn 2 blocks at the top of player's tower without changing the state
                BlockListManager blockListManager = skillInfo.GolbalBlockListManager;
                Debug.Log("Engineer Skill Casted");
                blockListManager.ServiceBuildState(false);
                Debug.Log("Engineer Skill End");
                mTimeUntilNextSkill = kCoolDown;
            }
            else
            {
                return;
            }
        }
    }
}