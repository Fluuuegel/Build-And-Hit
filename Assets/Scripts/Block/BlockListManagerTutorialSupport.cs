
using System;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

public partial class BlockListManager
{
    private const int R = 0, G = 1, B = 2, S = 3;

    private int[] eBuildTutorialList = { B, B, B, B, B, B, B, B, B, B, B, B, B, B, B, B, B, B, B, B };

    // hit slime block to get combo
    private int[] eComboTutorialList = { R, R, B, B, G, G, B, B, B, B, B, G, G, B, B, R, R };

    // use engineer skill to win first
    private int[]
        eEngineerSkillTutorialList =
            { B, B, B, B, B, B, B, B, B, B, B, B, B, B, B, B, B }; //height of 17 use skill to win immediately
    public enum TutotialType
    {
        eBuild,
        eHitAndCombo,
        eEngineerSkill,
        eNotTutorial,

    }
    TutotialType mTutorialType = TutotialType.eNotTutorial;
    public bool mIsTutorial = false;

    public void ActivateTutorial()
    {
        mIsTutorial = true;
    }

    /*
     * @ResetTutorial
     * if the player is stupid enough to fail the tutorial, reset the tutorial
     */
    public void ResetTutorial()
    {
        switch (mTutorialType)
        {
            case TutotialType.eBuild:
                ResetBuildTutorial();
                break;
            default:
                Debug.Log("Not a tutorial!");
                break;
        }

    }

    private void ResetBuildTutorial()
    {
        int playerIndex = 0;
        for (int i = 0; i < eBuildTutorialList.Length; i++)
        {
            mBlockManagers[playerIndex].BuildOneBlock(playerIndex, false, eBuildTutorialList[i], true);
        }

        int playerIndex2 = 1;
        for (int i = 0; i < kInitBlockIndex; i++)
        {
            ResetRandom();
            mBlockManagers[playerIndex2].BuildOneBlock(playerIndex2, false, GenRandomColour(), true);
            ResetRandom();
        }
    }
}