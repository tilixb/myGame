using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSkeletonBehaviour : MonoBehaviour
{
    public Transform[] physicsSkeleton;
    public Rigidbody[] physicsRigidbodies;
    private VirtualSkeletonBehaviour m_virtualBehaviour;
    private int m_leftFootIndex=0;
    private int m_leftLegIndex=1;
    private int m_bodyIndex=2;
    private int m_rightLegIndex=3;
    private int m_rightFootIndex=4;

    [Space(10)] [SerializeField] private float idleForceCoreUp;
    //[SerializeField] private Vector3 idleFootForce; //half of coreForce
    [Space(10)] [SerializeField] private float stepForceSupportLegUp;
    [SerializeField] private float stepForceCoreUp;
    [SerializeField] private float stepForceStepLegUp;
    [SerializeField] private float stepForceStepFootUp;
    [SerializeField] private float additionalForce;
    private float initialAdditionalForce;
    [SerializeField] private float airOnFootForce = 75;
    
    [Space(10)] [SerializeField] private float accelerate;
    public bool notInTheAir { get; set; } = false;
    private float squartTimer = 0f;
    [SerializeField]private float squartDelayTime = 0.1f;
    public bool isSquartAccelerateDelayed { get; private set; } = false;

    [Space(10)]
    [SerializeField] private float spring;
    [SerializeField] private float airSpring;
    private float initialSpring;
    [SerializeField] private float torqueSpring;
    [SerializeField] private float predictTime;
    [SerializeField] private float stopAnglerThreshold;
    [SerializeField] private float[] stableProduct;
    
    [Space(10)]
    private bool isRespawning = false;
    private WaitForSeconds isRespawningTime;
    public float respawningTime=2.0f;
    [Space(10)]
    public float speedForParticle = 5.0f;
    public GameObject particlePrefab;
    public Transform particleTransform;
    private GameObject particleInstance;
    [Space(10)] public float forceMultiplier = 5;


    //balance
    private void Start()
    {
        initialAdditionalForce = additionalForce;
        initialSpring = spring;
        m_virtualBehaviour = GetComponent<VirtualSkeletonBehaviour>();
        isRespawningTime = new WaitForSeconds(respawningTime);
        EventManager.Instance.OnPlayerRespawned += HandlePlayerRespawned;
    }

    public static Vector3 CalculateAlignmentForce(Vector3 position, Vector3 velocity, Vector3 targetPosition, float spring, float predictTime)
    {
        Vector3 curPredictPosition = position + velocity * predictTime;
        Vector3 curPredictGap = targetPosition - curPredictPosition;
        Vector3 curPredictForce = spring * curPredictGap;
        return curPredictForce;
    }
    public static Vector3 CalculateAlignmentDirection(Vector3 direction, Vector3 angularVelocity, Vector3 targetDirection, float torqueSpring, float torqueDampingCoefficient)
    {
        Vector3 curPredictOriginDirection = Quaternion.AngleAxis(angularVelocity.magnitude * Mathf.Rad2Deg * torqueDampingCoefficient, angularVelocity) * direction.normalized;
        Vector3 curPredictRotationAxis = Vector3.Cross(curPredictOriginDirection.normalized, targetDirection.normalized);
        Vector3 curPredictTorque = curPredictRotationAxis.normalized * torqueSpring * Vector3.Angle(curPredictOriginDirection.normalized, targetDirection.normalized) * Mathf.Deg2Rad;
        return curPredictTorque;
    }

    private Vector3 platV;
    public void AddForceBasedOnPlatformVelocity(Vector3 platVelocity)
    {
        platV = platVelocity;
        foreach (var rigidbody in physicsRigidbodies)
        {
            // 计算人物与平台之间的速度差
            Vector3 velocityDifference = platVelocity - rigidbody.velocity;

            // 根据速度差施加适当的力
            rigidbody.AddForce(velocityDifference * forceMultiplier, ForceMode.Force);
        }
    } 
    public void ResetPlatformVelocity()
    {
        platV = Vector3.zero;
    }
    private void Alignment()
    {
        float maxDistance = 5f;

        for (int i = 0; i < 5; i++)
        {
            Vector3 curPredictPosition = physicsSkeleton[i].position 
                                         + (physicsRigidbodies[i].velocity-platV) * predictTime;
            Vector3 curPredictGap = m_virtualBehaviour.targetPositions[i] - curPredictPosition;


            Vector3 curPredictOriginDirection =
                Quaternion.AngleAxis(physicsRigidbodies[i].angularVelocity.magnitude * Mathf.Rad2Deg * predictTime,
                    physicsRigidbodies[i].angularVelocity)
                * (i == m_bodyIndex ? physicsSkeleton[i].up : physicsSkeleton[i].right);
            Vector3 curPredictRotationAxis = Vector3.Cross(curPredictOriginDirection.normalized,
                m_virtualBehaviour.targetDirections[i].normalized);
            float curPredictAngle = Vector3.Angle(curPredictOriginDirection.normalized,
                m_virtualBehaviour.targetDirections[i].normalized) * Mathf.Deg2Rad;

            var dynamicSpring = spring * curPredictAngle;
            dynamicSpring = Mathf.Abs(dynamicSpring);
            
            Vector3 curPredictForce = dynamicSpring * curPredictGap;
            Vector3 forSquartForce = spring * curPredictGap;

            if (curPredictAngle < stopAnglerThreshold) curPredictAngle = 0;
            var dynamicTorqueSpring =
                Mathf.Clamp(physicsRigidbodies[i].velocity.magnitude, 1.0f, Mathf.Infinity) * torqueSpring;
            Vector3 curPredictTorque =
                curPredictRotationAxis.normalized * curPredictAngle * dynamicTorqueSpring; //Ydir上的偏差，比如脚的，腿的，头的，主要修正旋转

            switch (m_virtualBehaviour.BipedState)
            {
                case VirtualSkeletonBehaviour.BipedAction.Squart:
                    physicsRigidbodies[i].AddForce(forSquartForce, ForceMode.Force);
                    break;
                case VirtualSkeletonBehaviour.BipedAction.LeftStep:
                    physicsRigidbodies[i].AddForce(curPredictForce*stableProduct[4-i], ForceMode.Force);
                    break;
                case VirtualSkeletonBehaviour.BipedAction.RightStep:
                    physicsRigidbodies[i].AddForce(curPredictForce*stableProduct[i], ForceMode.Force);
                    break;
                default:
                    physicsRigidbodies[i].AddForce(curPredictForce, ForceMode.Force);
                    break;
            }
            physicsRigidbodies[i].AddTorque(curPredictTorque, ForceMode.Force);
        }
    }
    

    private void FixedUpdate()
    {

        if (!notInTheAir)
        {
            additionalForce = airOnFootForce;
            spring = airSpring;
            for (int i = 0; i < 5; i++)
            {
                m_virtualBehaviour.targetPositions[i] = m_virtualBehaviour.virtualSkeletonPositions[i];
            }
        }else
        {
            spring = initialSpring;
            additionalForce = initialAdditionalForce;
        }
        if(isRespawning)
        {
            physicsRigidbodies[m_bodyIndex].AddForce(new Vector3(0,-idleForceCoreUp/5,0),ForceMode.Force);
            physicsRigidbodies[m_leftFootIndex].AddForce(new Vector3(0,idleForceCoreUp/10,0),ForceMode.Force);
            physicsRigidbodies[m_rightFootIndex].AddForce(new Vector3(0,idleForceCoreUp/10,0),ForceMode.Force);
            return;
        }

        //平衡，抵消重力对姿势的影响
        PhysicsBalance();
        //对动画的拟合,牵引力和力矩
        Alignment();

        SetSquartBehaviour();


    }

    void SetSquartBehaviour()
    {
        //驱动向前
        if (m_virtualBehaviour.BipedState == VirtualSkeletonBehaviour.BipedAction.Squart)
        {
            if (!isSquartAccelerateDelayed)
            {
                squartTimer += Time.deltaTime;
                if (squartTimer >= squartDelayTime)
                {
                    isSquartAccelerateDelayed = true;
                }
            }
            else
            {
                if(notInTheAir)
                //方向由动画提供
                {
                    var direction = m_virtualBehaviour.DragRootToMouseDir;
                    for (int i = 0; i < 5; i++)
                    {
                        physicsRigidbodies[i].AddForce(direction * accelerate, ForceMode.Acceleration);
                    }

                    //驱动的动画
                    float speed =new Vector3(physicsRigidbodies[m_bodyIndex].velocity.x,0,physicsRigidbodies[m_bodyIndex].velocity.z).magnitude ;
                    if (speed > speedForParticle)
                    {
                        if (particleInstance == null)
                        {
                            // Instantiate the particle system at the specified position
                            particleInstance = Instantiate(particlePrefab, particleTransform.position,
                                Quaternion.identity);
                        }
                    }
                    else
                    {
                        StartCoroutine(DestroyParticleInstance());
                    }
                }
                else
                {
                    StartCoroutine(DestroyParticleInstance());
                }
            }
        }
        else
        {
            squartTimer = 0f;
            isSquartAccelerateDelayed = false;
            StartCoroutine(DestroyParticleInstance());
        }
        
        if (particleInstance != null)
        {
            // Update the particle system position
            particleInstance.transform.position = particleTransform.position;
        }
    }
    
    private IEnumerator DestroyParticleInstance()
    {
        if (particleInstance != null)
        {
            // Get the ParticleSystem component
            ParticleSystem particleSystem = particleInstance.GetComponent<ParticleSystem>();

            // Set the looping property to false
            var mainModule = particleSystem.main;
            mainModule.loop = false;

            // Wait until the particle system is no longer alive
            yield return new WaitWhile(() => particleSystem.IsAlive(true));

            // Destroy the particle system
            Destroy(particleInstance);
        }
    }
    void PhysicsBalance()
    {
        if (m_virtualBehaviour.BipedState == VirtualSkeletonBehaviour.BipedAction.Idle)
        {
            IdleBanlance();
        }
        else if(m_virtualBehaviour.BipedState==VirtualSkeletonBehaviour.BipedAction.LeftStep)
        {
            LeftStepBalance();
        }else if (m_virtualBehaviour.BipedState == VirtualSkeletonBehaviour.BipedAction.RightStep)
        {
            RightStepBalance();
        }else if (m_virtualBehaviour.BipedState == VirtualSkeletonBehaviour.BipedAction.Squart)
        {
            IdleBanlance();
        }
    }
    void IdleBanlance()
    {
        physicsRigidbodies[m_bodyIndex].AddForce(new Vector3(0,idleForceCoreUp,0),ForceMode.Force);
        physicsRigidbodies[m_leftFootIndex].AddForce(new Vector3(0,-idleForceCoreUp/2,0),ForceMode.Force);
        physicsRigidbodies[m_rightFootIndex].AddForce(new Vector3(0,-idleForceCoreUp/2,0),ForceMode.Force);
    }
    void LeftStepBalance()
    {
        physicsRigidbodies[m_leftFootIndex].AddForce(new Vector3(0,stepForceStepFootUp,0),ForceMode.Force);
        physicsRigidbodies[m_leftLegIndex].AddForce(new Vector3(0,stepForceStepLegUp,0),ForceMode.Force);
        physicsRigidbodies[m_bodyIndex].AddForce(new Vector3(0,stepForceCoreUp,0),ForceMode.Force);
        physicsRigidbodies[m_rightLegIndex].AddForce(new Vector3(0,stepForceSupportLegUp,0),ForceMode.Force);
        physicsRigidbodies[m_rightFootIndex].AddForce(new Vector3(0,-(stepForceStepFootUp+stepForceStepLegUp+stepForceCoreUp+stepForceSupportLegUp+additionalForce),0),ForceMode.Force);
    }
    void RightStepBalance()
    {
        physicsRigidbodies[m_rightFootIndex].AddForce(new Vector3(0,stepForceStepFootUp,0),ForceMode.Force);
        physicsRigidbodies[m_rightLegIndex].AddForce(new Vector3(0,stepForceStepLegUp,0),ForceMode.Force);
        physicsRigidbodies[m_bodyIndex].AddForce(new Vector3(0,stepForceCoreUp,0),ForceMode.Force);
        physicsRigidbodies[m_leftLegIndex].AddForce(new Vector3(0,stepForceSupportLegUp,0),ForceMode.Force);
        physicsRigidbodies[m_leftFootIndex].AddForce(new Vector3(0,-(stepForceStepFootUp+stepForceStepLegUp+stepForceCoreUp+stepForceSupportLegUp+additionalForce),0),ForceMode.Force);
    }
    //event


    private void OnDestroy()
    {
        EventManager.Instance.OnPlayerRespawned -= HandlePlayerRespawned;
    }

    private void HandlePlayerRespawned()
    {
        StartCoroutine(Respawning());
    }
    
    private IEnumerator Respawning()
    {
        isRespawning = true;

        yield return isRespawningTime;

        InputManager.Instance.HaveInput();
        isRespawning = false;
    }

    public int GetRigidIndex(Rigidbody targetRigidbody)
    {
        int index = System.Array.IndexOf(physicsRigidbodies, targetRigidbody);

        return index;
    }
}
