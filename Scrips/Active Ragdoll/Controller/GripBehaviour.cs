using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable] public struct JointMotionsConfig {
    public ConfigurableJointMotion angularXMotion, angularYMotion, angularZMotion;
    public float angularXLimit, angularYLimit, angularZLimit;

    public void ApplyTo(ref ConfigurableJoint joint) {
        joint.angularXMotion = angularXMotion;
        joint.angularYMotion = angularYMotion;
        joint.angularZMotion = angularZMotion;

        var softJointLimit = new SoftJointLimit();

        softJointLimit.limit = angularXLimit / 2;
        joint.highAngularXLimit = softJointLimit;

        softJointLimit.limit = -softJointLimit.limit;
        joint.lowAngularXLimit = softJointLimit;

        softJointLimit.limit = angularYLimit;
        joint.angularYLimit = softJointLimit;

        softJointLimit.limit = angularZLimit;
        joint.angularZLimit = softJointLimit;
    }
}

public class GripBehaviour : MonoBehaviour
{
    public float leftArmWeightThreshold = 0.5f, rightArmWeightThreshold = 0.5f;

    public JointMotionsConfig defaultMotionsConfig;

    public bool onlyUseTriggers = false;
    public bool canGripYourself = false;

    private Gripper _leftGrip, _rightGrip;
    public GameObject _leftHand, _rightHand;

    private void Start()
    {

        (_leftGrip = _leftHand.AddComponent<Gripper>()).GripMod = this;
        (_rightGrip = _rightHand.AddComponent<Gripper>()).GripMod = this;
    }


    public void UseLeftGrip(InputAction.CallbackContext ctx)
    {
        _leftGrip.enabled = ctx.ReadValue<float>() > leftArmWeightThreshold;
    }

    public void UseRightGrip(InputAction.CallbackContext ctx)
    {
        _rightGrip.enabled = ctx.ReadValue<float>() > rightArmWeightThreshold;
    }

    public class Gripper : MonoBehaviour
    {
        public GripBehaviour GripMod { get; set; }
        private Rigidbody _lastCollision;

        private ConfigurableJoint _joint;
        public JointMotionsConfig jointMotionsConfig;

        public void Start()
        {
            // Start disabled is useful to avoid fake gripping something at the start
            enabled = false;
        }

        private void Grip(Rigidbody whatToGrip)
        {
            if (!enabled)
            {
                _lastCollision = whatToGrip;
                return;
            }

            if (_joint != null)
                return;


            _joint = gameObject.AddComponent<ConfigurableJoint>();
            _joint.connectedBody = whatToGrip;
            _joint.xMotion = ConfigurableJointMotion.Locked;
            _joint.yMotion = ConfigurableJointMotion.Locked;
            _joint.zMotion = ConfigurableJointMotion.Locked;

            GripMod.defaultMotionsConfig.ApplyTo(ref _joint);
        }

        private void UnGrip()
        {
            if (_joint == null)
                return;

            Destroy(_joint);
            _joint = null;
        }


//enter就把对应的rigbody记录下来，当enable的时候抓起
        private void OnCollisionEnter(Collision collision)
        {
            if (GripMod.onlyUseTriggers)
                return;

            if (collision.rigidbody != null)
                Grip(collision.rigidbody);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody != null)
                Grip(other.attachedRigidbody);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.rigidbody == _lastCollision)
                _lastCollision = null;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.attachedRigidbody == _lastCollision)
                _lastCollision = null;
        }


        private void OnEnable()
        {
            if (_lastCollision != null)
                Grip(_lastCollision);
        }

        private void OnDisable()
        {
            UnGrip();
        }
    }
}