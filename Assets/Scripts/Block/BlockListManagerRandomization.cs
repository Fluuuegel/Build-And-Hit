using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.Rendering.Universal;
//use this to unify the color system
using BlockColor = BlockBehaviour.BlockColourType;
using Random = UnityEngine.Random;

public partial class BlockListManager
    {
        //fuck real randomization!
        public float mRedWeight = 33, mBlueWeight = 33, mGreenWeight = 33;
        private const float mLowestWeight = 1;
        //after generate one block of certain color, decrease its weight by this value
        //and add other 2 colors' weight by half this size
        private const float mWeightLostPerGen = 3f;

        private const float mWeightShrinkFactorPerGen = 3.2f;
        
        /*
         * @GenRanDomColour
         * this function can generate a series of blocks with a controlled possibility
         * that once a block is generated, it's very unlikely that the next block will have the same colour
         * call function @ResetRandom ONCE before you want to start generating a new list of block
         */
        private int GenRandomColour()
        {
            const float normalizeFactor = (mWeightShrinkFactorPerGen - 1f) / 2;
//            Debug.Log("Gen random color");
            BlockColor genBlockColor = BlockColor.eInvalidColour;
            float randomFloat = Random.Range(0f, 1f);
            float weightSum = mRedWeight + mBlueWeight + mGreenWeight;
            float redBound = mRedWeight / weightSum;
            float blueBound = mBlueWeight / weightSum + redBound;
            float greenBound = mGreenWeight / weightSum + blueBound;
            if (randomFloat < redBound)
            {
                genBlockColor = BlockColor.eRed;
                {
                    mRedWeight /= mWeightShrinkFactorPerGen;
                    mGreenWeight += mRedWeight/mWeightShrinkFactorPerGen*normalizeFactor;
                    mBlueWeight += mRedWeight/mWeightShrinkFactorPerGen*normalizeFactor;
                }

            }
            else if(randomFloat < blueBound)
            {
                genBlockColor = BlockColor.eBlue;
                {
                    mBlueWeight /= mWeightShrinkFactorPerGen;
                    mRedWeight += mBlueWeight / mWeightShrinkFactorPerGen*normalizeFactor;
                    mGreenWeight += mBlueWeight / mWeightShrinkFactorPerGen*normalizeFactor;
                }
            }
            else 
            {
                genBlockColor = BlockColor.eGreen;
                mGreenWeight /= mWeightShrinkFactorPerGen;
                mRedWeight += mGreenWeight / mWeightShrinkFactorPerGen*normalizeFactor;
                mBlueWeight += mGreenWeight /mWeightShrinkFactorPerGen*normalizeFactor;
            }
            /*string msg = "Color Generated: " + genBlockColor.ToString();
            Debug.Log(msg);
            msg = "Red Weight: " + mRedWeight.ToString() + " Blue Weight: " + mBlueWeight.ToString() + " Green Weight: " + mGreenWeight.ToString();
            Debug.Log(msg);
            msg = "Red bound: " + redBound.ToString() + " Blue bound: " + blueBound.ToString() + " Green bound: " + greenBound.ToString();
            Debug.Log(msg);
            Debug.Log(randomFloat);*/
            return (int)(genBlockColor);
        }

        private void ResetRandom()
        {
            mRedWeight = mBlueWeight = mGreenWeight = 33f;
        }

        private void DyeOneBlockTowerRandomly(int towerIndex)
        {
            ResetRandom();
            BlockManager tower = mBlockManagers[towerIndex];
            int blockCount = tower.GetHeight();
            for (int i = 0; i < blockCount; i++)
            {
                tower.DyeBlock(i, (BlockColor)GenRandomColour());
            }
        }
    }