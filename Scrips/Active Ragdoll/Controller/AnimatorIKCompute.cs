using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

//Animator Controlller
public class AnimatorIKCompute : MonoBehaviour
{
    //set Ik for animator
    private AnimatorIK animatorIK;
    
    // [Tooltip("Those values define the rotation range in which the target direction influences arm movement.")]
    // public float minTargetDirAngle = -30,
    //     maxTargetDirAngle = 60;
    //
    // [Space(10)] [Tooltip("The limits of the arms direction. How far down/up should they be able to point?")]
    // public float minArmsAngle = -70;
    //
    // public float maxArmsAngle = 100;
    //
    // [Tooltip("The limits of the look direction. How far down/up should the character be able to look?")]
    // public float minLookAngle = -50, maxLookAngle = 60;
    //
    // [Space(10)] [Tooltip("The vertical offset of the look direction in reference to the target direction.")]
    // public float lookAngleOffset;
    //
    // [Tooltip("The vertical offset of the arms direction in reference to the target direction.")]
    // public float armsAngleOffset;
    //
    // [Tooltip("Defines the orientation of the hands")]
    // public float handsRotationOffset = 0;
    //
    // [Space(10)] [Tooltip("How far apart to place the arms")]
    // public float armsHorizontalSeparation = 0.75f;
    //
    // [Tooltip("The distance from the body to the hands in relation to how high/low they are. " +
    //          "Allows to create more realistic movement patterns.")]
    // public AnimationCurve armsDistance;

    private Vector3 _aimDirection;
    private Vector3 _armsDir, _lookDir, _aimedDir2d;
    //public Transform _animTorso, _chest, _head;
    private float _targetDirVerticalPercent;
    private Vector3 lookAt, LHand, RHand;
    private void Start()
    {
        animatorIK = GetComponent<AnimatorIK>();
        lookAt = animatorIK.LookPoint.transform.position;
        LHand = animatorIK.LeftHandTarget.transform.position;
        RHand = animatorIK.RightHandTarget.transform.position;
    }

    void FixedUpdate()
    {
        _aimDirection = Camera.main.transform.forward;
        UpdateLocalIk();
        //UpdateIK();
    }


    private void UpdateLocalIk()
    {
        animatorIK.LookIKWeight = 1;
        Vector3 temp = _aimDirection;
        temp.x = Mathf.Clamp(_aimDirection.x, -0.5f, 0.5f);
        temp.z = Mathf.Clamp(_aimDirection.z, 0, 0.5f);
        if (temp.y > 0) temp.y *= 1000000;
        temp.y = Mathf.Clamp(_aimDirection.y, -0.5f, 15.0f);
        animatorIK.LookPoint.transform.position =lookAt+ temp;
        animatorIK.LeftHandTarget.transform.position =LHand+ new Vector3(0,temp.y,0);
        animatorIK.RightHandTarget.transform.position=RHand+ new Vector3(0,temp.y,0);
    }
    
    // private void UpdateIK()
    // {
    //     
    //     animatorIK.LookIKWeight = 1;
    //     _aimedDir2d =Vector3.ProjectOnPlane(_aimDirection, Vector3.up).normalized;
    //     CalculateVerticalPercent();
    //
    //     UpdateLookIK();
    //     UpdateArmsIK();
    // }
    //
    //
    // private void CalculateVerticalPercent()
    // {
    //     float directionAngle = Vector3.Angle(_aimDirection, Vector3.up);
    //     directionAngle -= 90;
    //     _targetDirVerticalPercent =
    //         1 - Mathf.Clamp01((directionAngle - minTargetDirAngle) / Mathf.Abs(maxTargetDirAngle - minTargetDirAngle));
    // }
    //
    // //交给AnimatorIK处理,物理追隨即可
    // private void UpdateLookIK()
    // {
    //     float lookVerticalAngle = _targetDirVerticalPercent * Mathf.Abs(maxLookAngle - minLookAngle) + minLookAngle;
    //     lookVerticalAngle += lookAngleOffset;
    //     _lookDir = Quaternion.AngleAxis(-lookVerticalAngle, _animTorso.right) * _aimedDir2d;
    //
    //     Vector3 lookPoint = _head.position + _lookDir;
    //
    //     //animatorIK.LookPoint = lookPoint;
    // }
    //
    // private void UpdateArmsIK()
    // {
    //     float armsVerticalAngle = _targetDirVerticalPercent * Mathf.Abs(maxArmsAngle - minArmsAngle) + minArmsAngle;
    //     armsVerticalAngle += armsAngleOffset;
    //     _armsDir = Quaternion.AngleAxis(-armsVerticalAngle, _animTorso.right) * _aimedDir2d;
    //
    //     float currentArmsDistance = armsDistance.Evaluate(_targetDirVerticalPercent);
    //
    //     Vector3 armsMiddleTarget = _chest.position + _armsDir * currentArmsDistance;
    //     Vector3 upRef = Vector3.Cross(_armsDir, _animTorso.right).normalized;
    //     Vector3 armsHorizontalVec = Vector3.Cross(_armsDir, upRef).normalized;
    //     Quaternion handsRot = _armsDir != Vector3.zero
    //         ? Quaternion.LookRotation(_armsDir, upRef)
    //         : Quaternion.identity;
    //
    //     animatorIK.LeftHandTarget.position = armsMiddleTarget + armsHorizontalVec * armsHorizontalSeparation / 2;
    //     animatorIK.RightHandTarget.position = armsMiddleTarget - armsHorizontalVec * armsHorizontalSeparation / 2;
    //     animatorIK.LeftHandTarget.rotation = handsRot * Quaternion.Euler(0, 0, 90 - handsRotationOffset);
    //     animatorIK.RightHandTarget.rotation = handsRot * Quaternion.Euler(0, 0, -90 + handsRotationOffset);
    //
    // }

    public void UseLeftArm(InputAction.CallbackContext ctx)
    {
        animatorIK.LeftArmIKWeight = ctx.ReadValue<float>();
    }

    public void UseRightArm(InputAction.CallbackContext ctx)
    {
        animatorIK.RightArmIKWeight = ctx.ReadValue<float>();
    }
}