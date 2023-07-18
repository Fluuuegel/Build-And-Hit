using UnityEngine;

namespace Player
{
    public enum PlayerType
    {
        engineer,
        gunSlinger,
        ranger
    }
    public abstract class Player
    {
        PlayerType mPlayerType;
        public abstract void SkillCast(SkillInfo skillInfo);
        public PlayerType GetPlayerType()
        {
            return mPlayerType;
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
                        if(playerDebug)
                            Debug.Log("Make a Dasher");
                    break;
                case "Char2":
                    case "Char2R":
                        player = new PlayerEngineer();
                        if(playerDebug)
                            Debug.Log("Make a Engineer");
                        break;
                /*case PlayerType.gunSlinger:
                    //player = new PlayerGunSlinger();
                    break;
                case PlayerType.ranger:
                    //player = new PlayerRanger();
                    break;*/
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
    public SkillInfo()
    {
        Debug.Log("Empty skill info");
        PlayerBlockManager = TargetBlockManager = null;
        WillCast = false;
        CurrentState = BlockListManager.BlockState.eInvalid;
        GolbalBlockListManager = null;
    }
    public BlockManager PlayerBlockManager, TargetBlockManager;
    public bool WillCast;
    public BlockListManager.BlockState CurrentState;
    public int curPlayerIndex;
    public BlockListManager GolbalBlockListManager;
}