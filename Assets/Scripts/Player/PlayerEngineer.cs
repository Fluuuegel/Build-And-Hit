using Unity.VisualScripting;
using UnityEngine;

namespace Player
{   
    public class PlayerEngineer : Player
    {
        private const int kCD = 2;
        
        public override void SkillCast(SkillInfo skillInfo)
        {
            if (skillInfo.WillCast)
            {
                //spawn 2 blocks at the top of player's tower without changing the state
                BlockListManager blockListManager = skillInfo.GolbalBlockListManager;
                Debug.Log("Engineer Skill Casted");
                blockListManager.ServiceBuildState(false);
                Debug.Log("Engineer Skill End");
                mTimeUntilNextSkill = kCD;
            }
            else
            {
                return;
            }
        }
    }
}