using UnityEngine;

namespace Player
{   
    public enum PlayerType
    {   
        eDasher,
        eEngineer,
        eSlime,
        eAPinkBall,
    }
    public abstract class Player
    {
        PlayerType mPlayerType;

        public Animator mAnimator;
        
        public Vector2 mPlayerPosition = new Vector2(0, 0);
        public int mPlayerIndex = 0;

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
            return 5;
        }

        public virtual void ExtendedRefreshRound()
        {
        }

        public virtual bool CanCastSkill() {
            return mTimeUntilNextSkill == 0;
        }

        public virtual void GetAnimator(Animator animator)
        {
            mAnimator = animator;
        }

        public virtual void GetPlayerIndex(int index)
        {
            mPlayerIndex = index;
        }

        public virtual void GetPlayerPosition(Vector2 position)
        {
            mPlayerPosition = position;
        }

        public virtual void SetColor(BlockBehaviour.BlockColourType color)
        {
            return;
        }
        public virtual void IncreaseTimeUntilNextSkill() {
            if(mTimeUntilNextSkill > 0)
            {
                mTimeUntilNextSkill--;
            }
        }
        public virtual void Recover() {
            return;
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

                case "APinkBall":
                    case "APinkBallR":
                        player = new PlayerAPinkBall();
                        player.mPlayerType = PlayerType.eAPinkBall;
                        if(playerDebug)
                            Debug.Log("Make A Pink Ball");
                        break;

                default:
                    throw new System.Exception("Player type not found");
            }
            //all skills are in cooldown when game starts
            //player.mTimeUntilNextSkill = player.GetMaxCD();
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