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
                mBuildButtons[j].SetActive(false);
                mHitButtons[j].SetActive(false);
                mSkillButtons[j].SetActive(false);
                mGainedSkillButtons[j].SetActive(false);
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
            mBGM.Stop();
            mMusic.clip = Resources.Load<AudioClip>("music/Audio_Win");
            mMusic.Play();
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
                mMusic.Stop();
                mMusic.clip = Resources.Load<AudioClip>("music/Audio_Win");
                mMusic.Play();
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
            mRefreshKey = KeyCode.R;
        }

        if (mPlayerIndex == 1)
        {
            mHitKeyCode = KeyCode.LeftArrow;
            mBuildKeyCode = KeyCode.RightArrow;
            mSkill1KeyCode = KeyCode.Comma;
            mSkill2KeyCode = KeyCode.Period;
            mUpBlockKey = KeyCode.UpArrow;
            mDownBlockKey = KeyCode.DownArrow;
            mRefreshKey = KeyCode.Slash;
        }
    }
    
    #endregion

    #region Trigger skills
    private void TriggerGainedSkill()
    {
        GainedSkillHintText.text = null;

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
            GainedSkillHintText.text = null;
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
    #endregion

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

    #region UI
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

    private void ModifyCDUI()
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

    private bool CanRefreshTower(int playerIndex)
    {
        return (canRefresh[playerIndex] && (maxTurn - mTurnCnt) <= 2);
    }

    private bool TriggerRefresh(bool haltAnimation = true)
    {
        
        if(Input.GetKeyDown(mRefreshKey) && CanRefreshTower(mPlayerIndex))
        {
            if(haltAnimation)
                mBlockAnimator.SetBool("IsSelected", false);
            DyeOneBlockTowerRandomly(mPlayerIndex); 
            canRefresh[mPlayerIndex] = false;
            return true;
        }
        return false;
    }

    private void RefreshBlockHeight()
    {
        int[] blockHeight = new int[mBlockManagers.Length];
        for (int i = 0; i < mBlockManagers.Length; i++)
        {
            blockHeight[i] = GetPlayerBlockHeight(i);
        }
        if (blockHeight[0] <= 3 && blockHeight[1] > 3)
        {
            mBlockHeight.GetComponent<TextMeshProUGUI>().text = $"<color=red>{blockHeight[0]}</color>       :       {blockHeight[1]}";
        }
        else if (blockHeight[1] <= 3 && blockHeight[0] > 3)
        {
            mBlockHeight.GetComponent<TextMeshProUGUI>().text = $"{blockHeight[0]}       :       <color=red>{blockHeight[1]}</color>";
        }
        else if (blockHeight[0] <= 3 && blockHeight[1] <= 3)
        {
            mBlockHeight.GetComponent<TextMeshProUGUI>().text = $"<color=red>{blockHeight[0]}</color>       :       <color=red>{blockHeight[1]}</color>";
        }
        else
        {
            mBlockHeight.GetComponent<TextMeshProUGUI>().text = $"{blockHeight[0]}       :       {blockHeight[1]}";
        }
    }
    #endregion UI
}
