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
        private const float mWeightLostPerGen = 8;
        
        /*
         * color pattern
         */
        private int test_GenRandomColour()
        {
            Debug.Log("Gen random color");
            /*if (mRedWeight < mLowestWeight) mRedWeight = mLowestWeight;
            if (mBlueWeight < mLowestWeight) mBlueWeight = mLowestWeight;
            if (mGreenWeight < mLowestWeight) mGreenWeight = mLowestWeight;*/
            BlockColor genBlockColor = BlockColor.eInvalidColour;
            float randomFloat = Random.Range(0f, 1f);
            float normalizeFactor = mRedWeight + mBlueWeight + mGreenWeight;
            float redBound = mRedWeight / normalizeFactor;
            float blueBound = mBlueWeight / normalizeFactor + redBound;
            float greenBound = mGreenWeight / normalizeFactor + blueBound;
            for (int i = 0; i < 99; i++)
            {
                
                if (randomFloat < redBound)
                {
                    genBlockColor = BlockColor.eRed;
                    if (mRedWeight - mWeightLostPerGen > mLowestWeight)
                    {
                        mRedWeight /= 2f;
                        mGreenWeight += mRedWeight/2f;
                        mBlueWeight += mRedWeight/2f;
                    }

                    break;
                }
                else if(randomFloat < blueBound)
                {
                    genBlockColor = BlockColor.eBlue;
                    if (mBlueWeight - mWeightLostPerGen > mLowestWeight)
                    {
                        mBlueWeight /= 2f;
                        mRedWeight += mBlueWeight / 2f;
                        mGreenWeight += mBlueWeight / 2f;
                    }
                    break;
                }
                else 
                {
                    genBlockColor = BlockColor.eGreen;
                    mGreenWeight /= 2f;
                    mRedWeight += mGreenWeight / 2f;
                    mBlueWeight += mGreenWeight /2f;
                    break;
                }

            }
            if(genBlockColor == BlockColor.eInvalidColour)
            {
                Debug.Log("Error: BlockListManager.GenRandomColour() failed to generate a valid color");
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
    }