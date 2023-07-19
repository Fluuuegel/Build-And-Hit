using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class PlayerSlime : Player
    {
        public override void SkillCast(SkillInfo skillInfo)
        {
            if (skillInfo.WillCast)
            {
                //spawn 2 blocks at the top of player's tower without changing the state
                BlockListManager blockListManager = skillInfo.GolbalBlockListManager;
                Debug.Log("Slime Skill Casted");
                blockListManager.ServiceBuildState(false, true);
                Debug.Log("Slime Skill End");
            }
            else
            {
                return;
            }
        }
    }
}