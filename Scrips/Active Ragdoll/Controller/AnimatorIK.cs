using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Animator IK
public class AnimatorIK : MonoBehaviour
{
    private Animator _animator;
    public Transform LookPoint;

    // 在场景中创建IK Target
    private Transform _targetsParent;
    public Transform LeftHandTarget;
    public Transform RightHandTarget;
    public Transform LeftHandHint { get; set; }
    public Transform RightHandHint { get; set; }
    public Transform LeftFootTarget { get; set; }
    public Transform RightFootTarget { get; set; }


    public float LookIKWeight { get; set; }
    public float LeftArmIKWeight { get; set; }
    public float RightArmIKWeight { get; set; }
    public float LeftLegIKWeight { get; set; }
    public float RightLegIKWeight { get; set; }


    private float _currentLeftArmIKWeight = 0, _currentRightArmIKWeight = 0;
    private float _currentLeftLegIKWeight = 0, _currentRightLegIKWeight = 0;
    

    void Awake()
    {
        _animator = GetComponent<Animator>();

        // _targetsParent = new GameObject("IKTargets").transform;
        // _targetsParent.parent = transform;
        //
        // LeftHandTarget = new GameObject("LeftHandTarget").transform;
        // RightHandTarget = new GameObject("RightHandTarget").transform;
        // LeftHandTarget.parent = _targetsParent;
        // RightHandTarget.parent = _targetsParent;

        // LeftHandHint = new GameObject("LeftHandHint").transform;
        // RightHandHint = new GameObject("RightHandHint").transform;
        // LeftHandHint.parent = _targetsParent;
        // RightHandHint.parent = _targetsParent;

        // LeftFootTarget = new GameObject("LeftFootTarget").transform;
        // RightFootTarget = new GameObject("RightFootTarget").transform;
        // LeftFootTarget.parent = _targetsParent;
        // RightFootTarget.parent = _targetsParent;
    }

    private void Update()
    {
        _currentLeftArmIKWeight = LeftArmIKWeight;
        _currentLeftLegIKWeight = LeftLegIKWeight;
        _currentRightArmIKWeight = RightArmIKWeight;
        _currentRightLegIKWeight = RightLegIKWeight;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        // Look
        _animator.SetLookAtWeight(LookIKWeight, ((LeftArmIKWeight + RightArmIKWeight) / 2) * 0.5f, 1, 0,0.5f);
        _animator.SetLookAtPosition(LookPoint.position);

        // Left arm
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _currentLeftArmIKWeight);
        _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _currentLeftArmIKWeight);
        // _animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, LeftArmIKWeight);

        _animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.position);
        _animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandTarget.rotation);
        // _animator.SetIKHintPosition(AvatarIKHint.LeftElbow, LeftHandHint.position);

        // Right arm
        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _currentRightArmIKWeight);
        _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _currentRightArmIKWeight);
        // _animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, RightArmIKWeight);

        _animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandTarget.position);
        _animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandTarget.rotation);
        // _animator.SetIKHintPosition(AvatarIKHint.RightElbow, RightHandHint.position);

        // // Left leg
        // _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, _currentLeftLegIKWeight);
        // _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _currentLeftLegIKWeight);
        //
        // _animator.SetIKPosition(AvatarIKGoal.LeftFoot, LeftFootTarget.position);
        // _animator.SetIKRotation(AvatarIKGoal.LeftFoot, LeftFootTarget.rotation);
        //
        // // Right leg
        // _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, _currentRightLegIKWeight);
        // _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, _currentRightLegIKWeight);
        //
        // _animator.SetIKPosition(AvatarIKGoal.RightFoot, RightFootTarget.position);
        // _animator.SetIKRotation(AvatarIKGoal.RightFoot, RightFootTarget.rotation);
    }
    
}