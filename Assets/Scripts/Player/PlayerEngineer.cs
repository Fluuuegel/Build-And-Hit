namespace Player
{
    public class PlayerEngineer : Player
    {
        public override void SkillCast(SkillInfo skillInfo)
        {
            if (skillInfo.WillCast)
            {
                BlockListManager blockListManager = skillInfo.GolbalBlockListManager;
                blockListManager.ServiceBuildState(false);
                blockListManager.ServiceBuildState(false);
            }
            else
            {
                return;
            }
        }
    }
}  