using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CameraControll : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;
    [SerializeField] private float mFocusWeight = 20f;
    [SerializeField] private float mFocusRadius = 3f;
    private float mStableWeight = 10f;
    private float mStableRadius = 5f;

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    public void CameraShake()
    {
        impulseSource.GenerateImpulse();
    }

    public void CameraFocusOnPlayer(GameObject p)
    {
        ModifyTarget(p.name, mFocusWeight,mFocusRadius);
    }

    public void CameraUnfocusOnPlayer(GameObject p)
    {
        ModifyTarget(p.name, mStableWeight,mStableRadius);
    }

    public void ModifyTarget(string targetName, float weight, float radius)
    {
        CinemachineTargetGroup.Target[] targets = PlayerManager.mTargetGroup.m_Targets;
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].target != null && targets[i].target.name == targetName)
            {
                targets[i].weight = weight;
                targets[i].radius = radius;
            }
        }
    }

    public void CameraFocusOnBlock(GameObject block)
    {
        ModifyTarget(block, mFocusWeight, mFocusRadius);
    }
    public void ModifyTarget(GameObject target, float weight, float radius)
    {
        CinemachineTargetGroup.Target[] targets = PlayerManager.mTargetGroup.m_Targets;
        for(int i =  0; i < targets.Length; i++)
        {
            if (targets[i].target != null && targets[i].target.gameObject == target)
            {
                targets[i].weight = weight;
                targets[i].radius = radius;
            }
        }
    }
    
}
