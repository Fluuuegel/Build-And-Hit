using UnityEngine;

public partial class BlockListManager
{
    KeyCode mTotallyDestroyKey, mDyeTowerSameColorKey /*red for now*/, mSpawnRedBlockKey, mSpawnBlueBlockKey, mSpawnGreenBlockKey, mSpawnSlimeBlockKey;
    private int DyeColor = 0;
    public bool DeveloperMode { get; private set; } = false;       

    private void BindDeveloperKey()
    {
        if (DeveloperMode)
        {
            mTotallyDestroyKey = KeyCode.Space;
            mDyeTowerSameColorKey = KeyCode.RightAlt;
            mSpawnRedBlockKey = KeyCode.Z;
            mSpawnBlueBlockKey = KeyCode.X;
            mSpawnGreenBlockKey = KeyCode.C;
            mSpawnSlimeBlockKey = KeyCode.V;
        }
    }

    private void DeveloperModeUpdate()
    {
        if (DeveloperMode)
        {
            if (Input.GetKeyDown(mTotallyDestroyKey))
            {
                int height = mBlockManagers[mPlayerIndex].GetHeight();
                for (int i = 0; i < height; i++)
                {
                    mBlockManagers[mPlayerIndex].DestroyOneBlock(0);
                }
            }

            if (Input.GetKeyDown(mDyeTowerSameColorKey))
            {//turn tower to blue
                int height = mBlockManagers[mPlayerIndex].GetHeight();
                for (int i = 0; i < height; i++)
                {
                    mBlockManagers[mPlayerIndex].DyeBlock(i, (BlockBehaviour.BlockColourType)DyeColor);
                }
                DyeColor++;
                DyeColor %= 3;
            }

            if (Input.GetKeyDown(mSpawnRedBlockKey))
            {
                mBlockManagers[mPlayerIndex].BuildOneBlock(mPlayerIndex, false, 0, false);
            }
            if (Input.GetKeyDown(mSpawnBlueBlockKey))
            {
                mBlockManagers[mPlayerIndex].BuildOneBlock(mPlayerIndex, false, 1, false);
            }
            if (Input.GetKeyDown(mSpawnGreenBlockKey))
            {
                mBlockManagers[mPlayerIndex].BuildOneBlock(mPlayerIndex, false, 2, false);
            }
            if (Input.GetKeyDown(mSpawnSlimeBlockKey))
            {
                
            }
        }
    }
}
