using UnityEngine;

namespace Player
{
    public enum PlayerType
    {   
        eDasher,
        eEngineer,
        eSlime,
    }
    public abstract class Player
    {
        PlayerType mPlayerType;

        public int mTimeUntilNextSkill = 0;

        public abstract void SkillCast(SkillInfo skillInfo);
        public abstract int GetMaxCD();
        public int GetCurrentCD()
        {
            return mTimeUntilNextSkill;
        }
       
        public PlayerType GetPlayerType()
        {
            return mPlayerType;
        }

        public virtual int VisionRange()
        {
            return 4;
        }

        public virtual void ExtendedRefreshRound()
        {
        }

        public virtual bool CanCastSkill() {
            return mTimeUntilNextSkill == 0;
        }

        public virtual void IncreaseTimeUntilNextSkill() {
            if(mTimeUntilNextSkill > 0)
            {
                mTimeUntilNextSkill--;
            }
        }
        
        public static Player MakeNewPlayer(string type)
        {
            bool playerDebug = true;
            Player player = null;
            switch (type)
            {
                case "Char1":
                    case "Char1R":
                        player = new PlayerDasher();
                        player.mPlayerType = PlayerType.eDasher;
                        if(playerDebug)
                            Debug.Log("Make a Dasher");
                    break;
                case "Char2":
                    case "Char2R":
                        player = new PlayerEngineer();
                        player.mPlayerType = PlayerType.eEngineer;
                        if(playerDebug)
                            Debug.Log("Make a Engineer");
                        break;

                case "Slime":
                    case "SlimeR":
                        player = new PlayerSlime();
                        player.mPlayerType = PlayerType.eSlime;
                        if(playerDebug)
                            Debug.Log("Make a Slime");
                        break;

                default:
                    throw new System.Exception("Player type not found");
            }
            return player;
        }
    }
    
}

/*
 * pass this parameter to SkillCast function, then we will decide whether to cast skill or not
 * depending on the current state of the block
 */
public class SkillInfo
{
    public BlockManager PlayerBlockManager, TargetBlockManager;
    public bool WillCast;
    public BlockListManager.BlockState CurrentState;
    public int curPlayerIndex;
    public BlockListManager GolbalBlockListManager;
    public SkillInfo()
    {
        PlayerBlockManager = TargetBlockManager = null;
        WillCast = false;
        CurrentState = BlockListManager.BlockState.eInvalid;
        GolbalBlockListManager = null;
    }
}