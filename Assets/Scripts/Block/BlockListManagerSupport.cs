using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using Unity.VisualScripting;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using TMPro;

/*
 * @BlockListManagerSupport
 * all the until function should be in here
 */
public partial class BlockListManager : MonoBehaviour
{
    #region Round initialization
    /*
     * @JudgeVictory
     * called when entering Idle state, go through all wining conditions to see if any player wins
     */
    private bool JudgeVictory(int turncnt,ref int winnerIndex) {
        UndisplayLastStandUI();
        if (turncnt == 0) {
            mEndCanvas.SetActive(true);
            for (int j = 0; j < kPlayerNum; j++) {
                mHitButtons[j].SetActive(false);
                mBuildButtons[j].SetActive(false);
                mSkillButtons[j].SetActive(false);
            }
            if (mBlockManagers[0].GetHeight() > mBlockManagers[1].GetHeight()) {
                winnerIndex = 0;
                mWinImages[0] = GameObject.Find("EndCanvas/Panel/P1Win");
                mWinImages[1] = GameObject.Find("EndCanvas/Panel/P2Win");
            } else {
                winnerIndex = 1;
                mWinImages[0] = GameObject.Find("EndCanvas/Panel/P2Win");
                mWinImages[1] = GameObject.Find("EndCanvas/Panel/P1Win");
            }
            mWinImages[0].SetActive(true);
            mWinImages[1].SetActive(false);
            mBlockState = BlockState.eEnd;
            return true;
        }
        for (int i = 0; i < kPlayerNum; i++) {
            if (mBlockManagers[i].GetHeight() == 0) {
                
                mEndCanvas.SetActive(true);
                mWinImages[i] = GameObject.Find("EndCanvas/Panel/P" + (i + 1) + "Win");
                mWinImages[1 - i] = GameObject.Find("EndCanvas/Panel/P" + (2 - i) + "Win");
                winnerIndex = 1 - i;

                for (int j = 0; j < kPlayerNum; j++) {
                    mHitButtons[j].SetActive(false);
                    mBuildButtons[j].SetActive(false);
                    mSkillButtons[j].SetActive(false);
                }
                mWinImages[i].SetActive(false);
                mWinImages[1 - i].SetActive(true);
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
        Debug.Log("Refreshing round");
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
    private void UpdateKeyBinding()
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

    #region Trigger skills
    private bool TriggerGainedSkill()
    {
        if (Input.GetKeyDown(mSkill2KeyCode) && (hasGainedSkill))
        {   
            if (GainedSkillIndex == 1)
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
            else if (GainedSkillIndex == 2)
            {
                SkillBuildFirstBlock();
            }
            else if (GainedSkillIndex == 3)
            {
                SkillChangeFirstBlock();
            }else
            {
                Debug.Log("Getting Skills Index is error!");
            }

            hasGainedSkill = false;
            hasCharacterSkill = false;
            return true;
        }
        return false;
    }

    // TODO: implement the skill cast
    private bool CastGainedSkill()
    {
        return false;
    }

    private bool TriggerSkill()
    {
        if (Input.GetKeyDown(mSkill1KeyCode) && (hasCharacterSkill))
        {
            CastUniqueSkill(mPlayers[mPlayerIndex]);
            hasGainedSkill = false;
            hasCharacterSkill = false;
            mSkillButtons[mPlayerIndex].SetActive(false);
            return true;
        }
        return false;
    }

    private void CastUniqueSkill(GameObject player)
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

    #region Camera
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

    #endregion
    
    #region Utils
    public int GetPlayerBlockHeight(int index)
    {
        return mBlockManagers[index].GetHeight();
    }
    #endregion

    private void DisplayLastStandUI()
    {
        mLastStandUI[mPlayerIndex].SetActive(true);
    }

    private void UndisplayLastStandUI()
    {
        for(int i = 0; i < mBlockManagers.Length; i++)
        {
            mLastStandUI[i].SetActive(false);
        }
    }

    private void ModifyCDValue()
    {
        PlayerBehaviour playerBehaviour = mPlayers[mPlayerIndex].GetComponent<PlayerBehaviour>();
        Player.Player curPlayer = playerBehaviour.GetPlayer();
        float mFillAmount = mSkillCDSlider[mPlayerIndex].GetComponent<Image>().fillAmount;
        int MaxCD = curPlayer.GetMaxCD();
        int curCD = curPlayer.GetCurrentCD();
        mFillAmount = (float)curCD / (float)MaxCD;
        mSkillCDSlider[mPlayerIndex].GetComponent<Image>().fillAmount = mFillAmount;
    }

    private void DisplayCountDown()
    {
        TextMeshProUGUI countdown = GameObject.Find("Canvas/RoundCountdown").GetComponent<TextMeshProUGUI>();
        if(countdown != null )
        {
            //Debug.Log(countdown.text);
        }
        else
        {
            Debug.Log("NO TMPRO");
        }
        countdown.text = $"Round Left: {mTurnCnt}";
    }
}
