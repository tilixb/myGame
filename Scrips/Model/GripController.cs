using System;
using Cinemachine.Utility;
using UnityEngine;

public class GripController : MonoBehaviour
{
    private Model model;
    [SerializeField] private GripModel leftGrip;
    [SerializeField] private GripModel rightGrip;

    private void Start()
    {
        model = Model.Instance;
    }
    private void resetLeft()
    {
        if (leftGrip.Joint.connectedBody == null) return;
        leftGrip.Joint.connectedBody = null;
        leftGrip.Joint.xMotion = ConfigurableJointMotion.Free;
        leftGrip.Joint.yMotion = ConfigurableJointMotion.Free;
        leftGrip.Joint.zMotion = ConfigurableJointMotion.Free;
        leftGrip.Joint.angularYMotion = ConfigurableJointMotion.Free;
    }

    private void resetRight()
    {
        if (rightGrip.Joint.connectedBody == null) return;
        rightGrip.Joint.connectedBody = null;
        rightGrip.Joint.xMotion = ConfigurableJointMotion.Free;
        rightGrip.Joint.yMotion = ConfigurableJointMotion.Free;
        rightGrip.Joint.zMotion = ConfigurableJointMotion.Free;
        rightGrip.Joint.angularYMotion = ConfigurableJointMotion.Free;
    }

    private void FixedUpdate()
    {
        switch (model.m_VirtualSkeletonBehaviour.BipedState)
        {
            case VirtualSkeletonBehaviour.BipedAction.Idle:
                resetLeft();
                resetRight();
                break;
            case VirtualSkeletonBehaviour.BipedAction.Squart:
                break;
            case VirtualSkeletonBehaviour.BipedAction.LeftStep:
                resetRight();
                break;
            case VirtualSkeletonBehaviour.BipedAction.RightStep:
                resetLeft();
                break;

        }
    }

}
