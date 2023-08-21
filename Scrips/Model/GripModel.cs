using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripModel : MonoBehaviour
{
    private Model model;
    public ConfigurableJoint Joint;
    public float stepThreshold;

    private void Start()
    {
        model = Model.Instance;
    }

    public void Grip(Rigidbody whatToGrip)
    {
        Joint.connectedBody = whatToGrip;
        Joint.xMotion = ConfigurableJointMotion.Locked;
        Joint.yMotion = ConfigurableJointMotion.Locked;
        Joint.zMotion = ConfigurableJointMotion.Locked;
        Joint.angularYMotion = ConfigurableJointMotion.Locked;
    }
    public void UnGrip(Rigidbody whatToGrip)
    {
        if (Joint.connectedBody == null) return;
        Joint.connectedBody = null;
        Joint.xMotion = ConfigurableJointMotion.Free;
        Joint.yMotion = ConfigurableJointMotion.Free;
        Joint.zMotion = ConfigurableJointMotion.Free;
        Joint.angularYMotion = ConfigurableJointMotion.Free;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.CompareTag("CanGrip")) return;
        if(collision.gameObject.layer==gameObject.layer||collision.rigidbody == null)return;
        if(collision.rigidbody.isKinematic) return;
        if (model.m_VirtualSkeletonBehaviour.m_stepReachParameter > stepThreshold)
        {
            Grip(collision.rigidbody);
        }
    }

}
