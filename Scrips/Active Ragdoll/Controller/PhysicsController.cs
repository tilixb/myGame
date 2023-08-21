
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


    public class PhysicsController : MonoBehaviour {

        public bool isBalanced=true ;
        [SerializeField] private float customTorsoAngularDrag = 0.05f;
        [SerializeField] private float freezeRotationSpeed = 5;
        
        public Rigidbody _physicalSpine;

        private ConfigurableJoint[] _joints;
        private Quaternion[] _initialJointsRotation;
        public Transform[] _animationbones;
        
        //rotate
        public Vector2 DirMovement { get; set; }
        private Quaternion _targetRotation;
        //jump
        private bool _isJump;
        public Vector3 jumpVelocity=new Vector3(0,4,0);
        private bool _isOnGround;
        //walk back..
        private int walkDir=0;
        public Vector3 WalkVelocity;
        private void Start()
        {
            _joints = GetComponentsInChildren<ConfigurableJoint>();
            _initialJointsRotation = new Quaternion[_joints.Length];
            int i = 0;
            foreach (var VARIABLE in _joints)
            {
                _initialJointsRotation[i]=VARIABLE.gameObject.transform.localRotation;
                i++;
            }
        }

        public void getTargetRotation(InputAction.CallbackContext ctx)
        {
            if (ctx.phase != InputActionPhase.Performed)
            {
                walkDir = 0;
                return;
            }
            DirMovement=ctx.ReadValue<Vector2>();
            walkDir = 1;
            if (DirMovement.y < 0)
            {
                DirMovement = new Vector2(DirMovement.x, -DirMovement.y);
                walkDir = -1;
            }
        }
        public void getJumpInput(InputAction.CallbackContext ctx)
        {
            _isJump = ctx.ReadValueAsButton();
        }

        private void UpdateTargetRotation()
        {
            // 前进方向 (相机 forward 在 XZ 平面上的投影)
            var aimedDir =
                Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
            var angleOffset = Vector2.SignedAngle(DirMovement, Vector2.up);
            // 根据 movement(输入) 和 aimedDirection(相机朝向) 的目标方向
            var targetRotation = Quaternion.AngleAxis(angleOffset, Vector3.up) * aimedDir;
            _targetRotation = Quaternion.LookRotation(targetRotation, Vector3.up);
        }

        private float _lastSpeedY;
        private void FixedUpdate()
        {
            //狀態
            UpdateOnFloor();
            
            UpdateTargetRotation();
            var smoothedRot = Quaternion.Lerp(_physicalSpine.rotation,
                _targetRotation, Time.fixedDeltaTime * freezeRotationSpeed);
            _physicalSpine.MoveRotation(smoothedRot);
            
            UpdateJointTargets();
            if(_isJump&&_isOnGround)PhysicsJump();
            if (walkDir != 0)
            {
                WalkBackVelocity();
                _physicalSpine.velocity =
                    new Vector3(_physicalSpine.velocity.x, _lastSpeedY + 0.01f, _physicalSpine.velocity.z);
            }
            //记录z的速度
            _lastSpeedY = _physicalSpine.velocity.y;
            
            ApplyCustomDrag();
        }   

        void PhysicsJump()
        {
            _physicalSpine.AddForce(jumpVelocity,ForceMode.VelocityChange);
        }        
        void WalkBackVelocity()
        {
            //_targetRotation * scale*..
            _physicalSpine.AddForce(_targetRotation *new Vector3(0,0,0.5f)*walkDir,ForceMode.VelocityChange);
        }
        
        //物理去匹配动画的rotation
        private void UpdateJointTargets() {
            for (int i = 1; i < _joints.Length; i++) {
                ConfigurableJointExtensions.SetTargetRotationLocal(_joints[i], _animationbones[i+1].localRotation, _initialJointsRotation[i]);
            }
        }
        private void ApplyCustomDrag() {
            var angVel = _physicalSpine.angularVelocity;
            angVel -= (Mathf.Pow(angVel.magnitude, 2) * customTorsoAngularDrag) * angVel.normalized;
            _physicalSpine.angularVelocity = angVel;
        }

        [Header("--- FLOOR ---")] public float floorDetectionDistance = 0.3f;
        public float maxFloorSlope = 60;

        public Rigidbody _rightPhysicalFoot, _leftPhysicalFoot;
    
        private void UpdateOnFloor()
        {
            _isOnGround = CheckRigidbodyOnFloor(_rightPhysicalFoot, out Vector3 foo)
                          || CheckRigidbodyOnFloor(_leftPhysicalFoot, out foo);
        }

        public bool CheckRigidbodyOnFloor(Rigidbody bodyPart, out Vector3 normal)
        {
            Ray ray = new Ray(bodyPart.position, Vector3.down);
            bool onFloor = Physics.Raycast(ray, out RaycastHit info, floorDetectionDistance,
                ~(1 << bodyPart.gameObject.layer));

            onFloor = onFloor && Vector3.Angle(info.normal, Vector3.up) <= maxFloorSlope;
            normal = info.normal;
            return onFloor;
        }
    }