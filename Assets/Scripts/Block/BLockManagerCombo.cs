using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Rendering;
using Cinemachine;
using Unity.VisualScripting;
/*
 * @BlockManagerCombo
 * This file is used to implement the combo logic
 */
public partial class BlockManager
{
    #region combo_logic
    public bool canCombo = true;                // the next combo is possible to happen
    public bool targetBlockCollided = false;    // the target block is collided
    public bool readyCombo = false;             // the next combo is ready to happen (0.5s after collision)
    public int comboLowerBound;                 // the lower bound of combo
    public GameObject targetBlock1 = null, targetBlock2 = null; // the 2 blocks that are about to collide after last elimination(both hit and combo)
    private float setUpTimeBetweenCollisionAndCombo = 0.3f;     // the time between collision and combo elimination
    private float collisionTimer = 0.0f;                        //fixme: this is not used yet

    public void SetTargetBlock(GameObject block1, GameObject block2)
    {
        targetBlock1 = block1;
        targetBlock2 = block2;
    }
    public bool IsTargetBlock(GameObject block1, GameObject block2){
        return (block1 == targetBlock1 && block2 == targetBlock2) || (block2 == targetBlock1 && block1 == targetBlock2);
    }
    public void setComboLowerBound(int lower_bound)
    {
        comboLowerBound = lower_bound;
    }

    /* @UpdateComboState
     * return true means that combo is achieved!
     */
    private void resetCombo()
    {
        canCombo = true;
        targetBlockCollided = false;
        readyCombo = false;
        targetBlock1 = targetBlock2 = null;
    }
    public bool UpdateComboState()
    {
        if(canCombo == false || (!targetBlock1) || (!targetBlock2))// no more combo from now!
        {
            resetCombo();
            return true;
        }
        //fixme: block collision ignore first
        if (targetBlockCollided)
        {
            MarkBlockForDestroy(comboLowerBound);
            TriggerReadyForCombo();
            if (readyCombo)
            {
                ComboInfo comboInfo = ComboFrom(comboLowerBound);
                if (!comboInfo.combo_achieved)//combo already fail, end of all
                {
                    canCombo = false;
                }
                else // combo is success, prepare for next combo
                {
                    AudioSource Sound = GameObject.Find("AudioObject").GetComponent<AudioSource>();
                    Sound.clip = Resources.Load<AudioClip>("music/Audio_Hit");
                    Sound.volume = 3.0f;
                    Sound.Play();
                    comboLowerBound = comboInfo.lower_bound - 1;
                    if (comboLowerBound < 0)
                    {
                        canCombo = false;
                    }
                    targetBlock1 = GetBlockAt(comboLowerBound);
                    targetBlock2 = GetBlockAt(comboLowerBound + 1);
                    if(!targetBlock1 || !targetBlock2)
                    {
                        canCombo = false;
                    }
                    targetBlockCollided = false;
                    readyCombo = false;
                }
            }
        }
        return false;
    }

    private void MarkBlockForDestroy(int lower_index)
    {
        if (lower_index < 0)
        {
            return;
        }

        int combo_cnt = 0;
        if (GetBlockColorAt(lower_index) != GetBlockColorAt(lower_index + 1))
        {
            return;
        }
        else
        {
            combo_cnt = 1;
        }
        
        int up_bound = lower_index, low_bound = lower_index;
        while (up_bound < mBlocks.Count - 1 && GetBlockColorAt(up_bound) == GetBlockColorAt(up_bound + 1))
        {
            up_bound++;
            combo_cnt++;
        }

        while (low_bound > 0 && GetBlockColorAt(low_bound) == GetBlockColorAt(low_bound - 1))
        {
            low_bound--;
            combo_cnt++;
        }

        if (combo_cnt < mComboBound)//not enough combo
            return;
        for(int i = low_bound; i <= up_bound; i++)
        {
            SetTransparent(mBlocks[i],0.3f);
        }
        
    }

    private void CameraShake()
    {
        CinemachineImpulseSource mShakeSource = GameObject.Find("VirtualCamera").GetComponent<CinemachineImpulseSource>();
        mShakeSource.GenerateImpulse();
    }
    /*
     * @TargetBlockCollided
     * for blocks to call when they collided
     */
    public bool TargetBlockCollided()
    {
        if (targetBlockCollided == false)
        {
            collisionTimer = Time.time;//record the collision time
        }
        targetBlockCollided = true;
        return targetBlockCollided;
    }
    
    /*
     * 
     */
    private bool TriggerReadyForCombo()
    {
        //todo: add count down
        if(Time.time - collisionTimer > setUpTimeBetweenCollisionAndCombo)
        {
            readyCombo = true;
        }
        return readyCombo;
    }
    /*
     * @CombolBlockInRange
     * destroy blocks in range [lower_bound, lower_bound + num)
     */
    private void CombolBlockInRange(int lower_bound, int num)
    {
        for(int i = 0; i < num; i++)
        {
            DestroyOneBlock(lower_bound);
        }
    }

    //parameter is the index of starting index of two block at edge
    public struct ComboInfo
    {
        public bool combo_achieved;
        public int lower_bound;
    }

    /*
     * @CombolFrom
     * return true if combo is achieved
     */
    public ComboInfo ComboFrom(int lower_index)
    {
        ComboInfo comboInfo = new ComboInfo();
        if (lower_index < 0)
        {
            comboInfo.combo_achieved = false;
            return comboInfo;
        }

        int combo_cnt = 0;
        if (GetBlockColorAt(lower_index) != GetBlockColorAt(lower_index + 1))
        {
            comboInfo.combo_achieved = false;
            return comboInfo;
        }
        else
        {
            combo_cnt = 1;
        }
        
        int up_bound = lower_index, low_bound = lower_index;
        while (up_bound < mBlocks.Count - 1 && GetBlockColorAt(up_bound) == GetBlockColorAt(up_bound + 1))
        {
            up_bound++;
            combo_cnt++;
        }

        while (low_bound > 0 && GetBlockColorAt(low_bound) == GetBlockColorAt(low_bound - 1))
        {
            low_bound--;
            combo_cnt++;
        }
        if (combo_cnt >= mComboBound)
        {
            CombolBlockInRange(low_bound, combo_cnt);
            CameraShake();
            comboInfo.combo_achieved = true;
            comboInfo.lower_bound = low_bound;
        }
        else
        {
            comboInfo.combo_achieved = false;
        }
        
        
        return comboInfo;

    }
    
    private void SetTransparent(GameObject block, float alpha)
    {
        SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
        Color color = sr.color;
        color.a = alpha;
        sr.color = color;
    }
    #endregion
}