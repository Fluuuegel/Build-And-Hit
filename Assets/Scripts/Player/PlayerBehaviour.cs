using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    
    Player.Player mPlayer = null;
    
    public int VisionRange()
    {
        return mPlayer.VisionRange();
    }
    public bool readyToCast()
    {
        return mPlayer.mCoolDownRound <= 0;
    }
    public void SetActualPlayer(Player.Player type)
    {
        mPlayer = type;
        
    }
    public void SkillCast(SkillInfo skillInfo)
    {
        mPlayer.SkillCast(skillInfo);
    }
    public bool isPlayer1 = true;

    public int mPlayerIndex = 1;
    
    public Animator animator;
    void Start() {
        animator = GetComponent<Animator>();
    }

    public void RefreshRound()
    {
        //update cooldown of skill
        if (mPlayer.mCoolDownRound > 0)
        {
            mPlayer.mCoolDownRound--;
        }
        mPlayer.ExtendedRefreshRound();
    }
    void Update() {
    }

    
}