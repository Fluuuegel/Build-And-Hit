using UnityEngine;

namespace Player
{
    public class PlayerEngineer : Player
    {
        public override void SkillCast(SkillInfo skillInfo)
        {
            if (skillInfo.WillCast)
            {
                //spawn 2 blocks at the top of player's tower without changing the state
                BlockListManager blockListManager = skillInfo.GolbalBlockListManager;
                Debug.Log("Engineer Skill Casted");
                blockListManager.ServiceBuildState(false);
                blockListManager.ServiceBuildState(false);
                Debug.Log("Engineer Skill End");
            }
            else
            {
                return;
            }
        }
    }
}  