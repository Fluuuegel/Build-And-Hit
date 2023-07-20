using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using Unity.VisualScripting;
/*
 * @BlockListManagerSupport
 * all the until function should be in here
 */
public partial class BlockListManager : MonoBehaviour
{
    public int GetCoolDownOfPlayer(int index)
    {
        return mHitCoolDown[index];
    }
    #region prepare for each round
    /*
     * @JudgeVictory
     * called when entering Idle state, go through all wining conditions to see if any player wins
     */
    private bool JudgeVictory() {
        for (int i = 0; i < kPlayerNum; i++) {
            if (mBlockManagers[i].GetHeight() == 0) {
                
                mEndCanvas.SetActive(true);
                mWinImages[i] = GameObject.Find("EndCanvas/Panel/P" + (i + 1) + "Win");
                mWinImages[1 - i] = GameObject.Find("EndCanvas/Panel/P" + (2 - i) + "Win");
                for (int j = 0; j < kPlayerNum; j++) {
                    mHitButtons[j].SetActive(false);
                    mBuildButtons[j].SetActive(false);
                    mSkillButtons[j].SetActive(false);
                }
                mWinImages[i].SetActive(false);
                mWinImages[1 - i].SetActive(true);
                mBlockState = BlockState.eEnd;
                return true;
            } else if (mBlockManagers[i].GetHeight() >= 20) {
                mEndCanvas.SetActive(true);
                mWinImages[i] = GameObject.Find("EndCanvas/Panel/P" + (i + 1) + "Win");
                mWinImages[1 - i] = GameObject.Find("EndCanvas/Panel/P" + (2 - i) + "Win");
                for (int j = 0; j < kPlayerNum; j++) {
                    mHitButtons[j].SetActive(false);
                    mBuildButtons[j].SetActive(false);
                    mSkillButtons[j].SetActive(false);
                }
                mWinImages[i].SetActive(true);
                mWinImages[1 - i].SetActive(false);
                mBlockState = BlockState.eEnd;
                return true;
            }
        }
        return false;
    }
    /*
     * @RoundRefresh
     * update the block manager and player status
     * exp: to decrease the cool down of skill, to reduce the immune round of block manager
     * call when every round starts only once
     */
    private void RoundRefresh()
    {
        Debug.Log("Refresh round");
        for(int i = 0; i < kPlayerNum; i++)
        {
            mBlockManagers[i].RefreshRound();
            PlayerBehaviour script = mPlayers[i].GetComponent<PlayerBehaviour>();
            script.RefreshRound();
        }

        if (mHitCoolDown[mPlayerIndex] > 0)
        {
            mHitCoolDown[mPlayerIndex]--;
        }
    }
    /*
     * @UpdatePlayerKeyBinding
     * update the control key for player1 and player2
     * called once when block list manager is entering the Idle state
     */
    private void UpdatePlayerKeyBinding()
    {
        if (mPlayerIndex == 0)
        {
            mHitKeyCode = KeyCode.D;
            mBuildKeyCode = KeyCode.A;
            mSkill1KeyCode = KeyCode.Q;
            mSkill2KeyCode = KeyCode.E;
            mUpBlockKey = KeyCode.W;
            mDownBlockKey = KeyCode.S;
        }

        if (mPlayerIndex == 1)
        {
            mHitKeyCode = KeyCode.LeftArrow;
            mBuildKeyCode = KeyCode.RightArrow;
            mSkill1KeyCode = KeyCode.Comma;
            mSkill2KeyCode = KeyCode.Period;
            mUpBlockKey = KeyCode.UpArrow;
            mDownBlockKey = KeyCode.DownArrow;
        }
    }
    
    
    #endregion

    #region trigger skills
    private bool TriGettingSkill()
    {
        if (Input.GetKeyDown(mSkill2KeyCode) && (mGettingSkills == GettingSkills.eGetSkills))
        {
            float ChooseSkills = Random.Range(0f, 1f);
            
            if (ChooseSkills < 0f)//Skill 1
            {
                if(mBlockManagers[mPlayerIndex].GetHeight() >= 1)
                {
                    SkillHitFirstBlock();
                }
                else
                {
                    Debug.Log("False to use the skills!");
                }
            }
            else//Skill 2
            {
                SkillBuildFirstBlock();
            }

            mGettingSkills = GettingSkills.eGetNormal;
            mBlockSkills = BlockSkills.eNormal;
            return true;
        }
        return false;
    }
    //fixme: implement the skill cast
    private bool CastGettingSkill()
    {
        return false;
    }
    private bool TriggerSkill()
    {
        if (Input.GetKeyDown(mSkill1KeyCode) && (mBlockSkills == BlockSkills.eSkills))
        {
            CastPlayerSkill(mPlayers[mPlayerIndex]);
            mBlockSkills = BlockSkills.eNormal;
            mGettingSkills = GettingSkills.eGetNormal;
            mSkillButtons[mPlayerIndex].SetActive(false);
            return true;
        }
        return false;
    }
    private void CastPlayerSkill(GameObject player)
    {
        PlayerBehaviour script = player.GetComponent<PlayerBehaviour>();
        SkillInfo skillInfo = WriteCurrentSkillInfo();
        script.SkillCast(skillInfo);
    }
    
    #endregion
    /*
     * @SkillInfo
     * write all information needed for casting a skill
     * called in triggerskill
     */
    private SkillInfo WriteCurrentSkillInfo()
    {
        SkillInfo cur = new SkillInfo();
        cur.PlayerBlockManager = mBlockManagers[mPlayerIndex];
        cur.TargetBlockManager = mBlockManagers[1 - mPlayerIndex];
        cur.CurrentState = mBlockState;
        cur.WillCast = true;
        cur.curPlayerIndex = mPlayerIndex;
        cur.GolbalBlockListManager = this;
        return cur;
    }
    #region CameraEffect
    public void CameraEffect(GameObject player)
    {
        
        if (mCameraControll != null)
        {
            mCameraControll.CameraFocusOnPlayer(player);
        }
        else
        {
            Debug.Log("No CameraControll Found");
        }
    }

    public void CameraEnd(GameObject playerWin, GameObject playerLose)
    {
        Debug.Log("CameraEnd!");
        if(mCameraControll != null)
        {
            Debug.Log($"focus on {playerWin.name}");
            mCameraControll.ModifyTarget(playerLose.name, 3f, 5f);
            mCameraControll.ModifyTarget(playerWin.name, 40f, 0.1f);
        }
    }
    #endregion CameraEffect
    
    
}
