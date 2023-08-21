using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;

public class VirtualSkeletonBehaviour : MonoBehaviour
{
//input
    private InputManager m_inputManager;
    public Vector2 MouseCameraPos { get; private set; }
    //鼠标到player的方向,不同状态root不一样，随着update更新
    public Vector3  DragRootToMouseDir{ get; set; }
    public LayerMask layerMask;

    [SerializeField] private float characterCirclethreshold = 1f;
    [SerializeField] private float maxAngleY = 150f;
    
    [SerializeField] private float clickQuickTime = 0.2f;
    //插值的速度
    [Space(10)]
    [SerializeField] private float m_stepCoreLerpSpeed = 10;
    [SerializeField] private float m_stepReachLerpSpeed = 5;
    [Range(0,1)]
    [SerializeField] private float m_stepClickQuickStartRatio = 0.5f;
    public float m_stepCoreParameter = 0;
    public float m_stepReachParameter = 0;

    public float lastSquartChange { get; private set; } = -10;
    [Space(10)]
    [SerializeField] private float squartChangeTime;
    
    private float m_stepReachParameterBase = 0;
    
    private bool isPressLeft, isPressRight;
    private bool isClickLeftDouble,isClickRightDouble;
    private float m_lastLeftPressTime = 0;
    private float m_lastRightPressTime = 0;
    private float m_lastLeftClickTime = 0;
    private float m_lastRightClickTime = 0;
    
    //base仅仅作为数据应用到函数，不做记载的功效
    [Space(10)]
    [SerializeField]public float alignCoreAngleBase;
    [SerializeField]private float alignCoreAngleBaseMax = 36f;
    
    [Space(5)]
    [SerializeField]private float alignLegStepReachAngleBase;
    [SerializeField]private float alignLegStepReachAngleBaseMax = 90f;
    [SerializeField]private float minusForFoot = 10f;
    
    [Space(5)]
    [SerializeField]private float alignLegStepSupportAngleBase;
    [SerializeField]private float alignLegStepSupportAngleBaseMax = 36f;

    [Space(10)] [SerializeField] private Transform[] physicsSkeleton;  
    public Vector3[] initialPositions{ get; private set; }
    public Vector3[] targetPositions { get; private set; }
    public Vector3[] virtualSkeletonPositions { get; private set; }
    public Vector3[] initialDirections { get; private set; }
    public Vector3[] targetDirections{ get; private set; }
    public Vector3[] virtualSkeletonDirections{ get; private set; }
    [SerializeField] private float m_skeletonLerpSpeed=5;
    // private Quaternion[] writeRotations;
    // private Vector3[] leftLocalPositions;//left foot
    // private Vector3[] rightLocalPositions;//right foot
    // private Vector3[] coreLocalPositions;//body
    // private Quaternion[] leftLocalRotations;//left foot
    // private Quaternion[] rightLocalRotations;//right foot
    // private Quaternion[] coreLocalRotations;//body
    private int m_leftFootIndex=0;
    private int m_leftLegIndex=1;
    private int m_bodyIndex=2;
    private int m_rightLegIndex=3;
    private int m_rightFootIndex=4;
    
    public enum BipedAction
    {
        Idle,
        LeftStep,
        RightStep,
        Squart,
    }
    public BipedAction BipedState { get; set; }
    [Range(0,1)]
    [SerializeField] private float legIdelSupportAngleRitio = 0.2f;
    private bool isFootRotateXY;
    private Quaternion rotationY;//绕y旋转的四元数
    // //子在父的位置
    // //当模型一开始是歪的时候，初始化的差值不是xy平面的,local下求即可
    // private Vector3[] InitialLocalPosition(int rootIndex)
    // {
    //     Vector3[] Position = new Vector3[5];
    //     for (int i = 0; i < 5; i++)
    //     {
    //         if (i < rootIndex)
    //             Position[i] = initialPositions[i] - initialPositions[i + 1];
    //         else if (i > rootIndex)
    //             Position[i] = initialPositions[i] - initialPositions[i - 1];
    //         else Position[i] = new Vector3(0, 0, 0);
    //     }
    //     return Position;
    // }
    // private Quaternion[] InitialLocalRotation(int rootIndex)
    // {
    //     Quaternion[] Rotation = new Quaternion[5];
    //     for (int i = 0; i < 5; i++)
    //     {
    //         if (i < rootIndex)
    //             Rotation[i] = Quaternion.Inverse(initialRotations[i + 1]) * initialRotations[i];
    //         else if (i > rootIndex)
    //             Rotation[i] = Quaternion.Inverse(initialRotations[i - 1]) * initialRotations[i];
    //         else Rotation[i] = quaternion.identity;
    //     }
    //
    //     return Rotation;
    // }
    private Transform modelRoot;
    private Camera m_mainCamera;


    void Start()
    {
        modelRoot = gameObject.transform.parent;
        m_inputManager = InputManager.Instance;
        m_mainCamera = Camera.main;

        m_inputManager.mouseDragAction += GetDir;
        m_inputManager.leftClickStarted += GetLeftStarted;
        m_inputManager.rightClickStarted += GetRightStarted;
        m_inputManager.leftClickPerformed += GetLeftPerformed;
        m_inputManager.rightClickPerformed += GetRightPerformed;
        m_inputManager.leftClickCanceled += GetLeftCanceled;
        m_inputManager.rightClickCanceled += GetRightCanceled;

        initialPositions = new Vector3[5];
        initialDirections = new Vector3[5];
        targetPositions = new Vector3[5];
        targetDirections = new Vector3[5];
        virtualSkeletonPositions = new Vector3[5];
        virtualSkeletonDirections = new Vector3[5];
        for (int i = 0; i < 5; i++)
        {
            //防止变形,initial就是为了求父子local的
            initialPositions[i] = modelRoot.worldToLocalMatrix * physicsSkeleton[i].position;
            //世界位置
            targetPositions[i] = physicsSkeleton[i].position;
            virtualSkeletonPositions[i] = physicsSkeleton[i].position;
            
            //记录最初的局部方向up
            if (i == m_bodyIndex)
            {
                initialDirections[i] = Quaternion.Inverse(modelRoot.rotation) * physicsSkeleton[i].up;
                targetDirections[i] = physicsSkeleton[i].up;
                virtualSkeletonDirections[i] = physicsSkeleton[i].up;
            }
            else
            {
                initialDirections[i] = Quaternion.Inverse(modelRoot.rotation) * physicsSkeleton[i].right;
                targetDirections[i] = physicsSkeleton[i].right;
                virtualSkeletonDirections[i] = physicsSkeleton[i].right;
            }
        }
        // leftLocalPositions=InitialLocalPosition(m_leftFootVirtual);
        // rightLocalPositions=InitialLocalPosition(m_rightFootVirtual);
        // coreLocalPositions=InitialLocalPosition(m_bodyVirtual);
        // leftLocalRotations=InitialLocalRotation(m_leftFootVirtual);
        // rightLocalRotations=InitialLocalRotation(m_rightFootVirtual);
        // coreLocalRotations=InitialLocalRotation(m_bodyVirtual);
    }

    void GetDir(Vector2 mousePosition)
    {
        MouseCameraPos = new Vector3(mousePosition.x,mousePosition.y);
    }

    bool isLeftClickQuick()
    {
        float timeSinceLastPress = Time.time - m_lastLeftPressTime;
        if (timeSinceLastPress < clickQuickTime)
        {
            return true;
        }
        return false;
    }
    bool isRightClickQuick()
    {
        float timeSinceLastPress = Time.time - m_lastRightPressTime;
        if (timeSinceLastPress < clickQuickTime)
        {
            return true;
        }
        return false;
    }
    
    //看时间来添加
    bool isLeftClickDouble()
    {
        float timeSinceLastClick = Time.time - m_lastLeftClickTime;
        if (timeSinceLastClick < clickQuickTime)
        {
            return true;
        }
        return false;
    }
    bool isRightClickDouble()
    {
        float timeSinceLastClick = Time.time - m_lastRightClickTime;
        if (timeSinceLastClick < clickQuickTime)
        {
            return true;
        }
        return false;
    }
    //还要初始化click
    void GetLeftStarted()
    {
        isPressLeft= true;
        
        m_stepCoreParameter = 0;
        if (isLeftClickQuick()) m_stepReachParameterBase = 0;
        else m_stepReachParameterBase = m_stepClickQuickStartRatio;

        //满足isLeftClickDouble的一定满足isLeftClickQuick，故m_stepReachParameter=0;
        //可以调整参数m_stepReachParameter=0或者m_stepReachLerpSpeed=0
        m_lastLeftClickTime = Time.time;
    }

    void GetRightStarted()
    {
        isPressRight = true;
        
        m_stepCoreParameter = 0;
        if (isRightClickQuick()) m_stepReachParameterBase = 0;
        else m_stepReachParameterBase = m_stepClickQuickStartRatio;
        
        
        m_lastRightClickTime = Time.time;
    }

    void GetLeftPerformed()
    {
        isPressLeft = true;
    }

    void GetRightPerformed()
    {
        isPressRight = true;
    }

    void GetLeftCanceled()
    {
        isPressLeft = false;
        m_lastLeftPressTime = Time.time;
    }

    void GetRightCanceled()
    {
        isPressRight = false;
        m_lastRightPressTime = Time.time;
    }

    void UpdateParameter()
    {
        switch (BipedState)
        {
            case BipedAction.LeftStep:
                m_stepCoreParameter =Mathf.Lerp(0, 1, (Time.time - m_lastLeftClickTime) * m_stepCoreLerpSpeed);
                m_stepReachParameter = Mathf.Lerp(m_stepReachParameterBase, 1, (Time.time - m_lastLeftClickTime) * m_stepReachLerpSpeed);
                break;
            case BipedAction.RightStep:
                m_stepCoreParameter =Mathf.Lerp(0, 1, (Time.time-m_lastRightClickTime) * m_stepCoreLerpSpeed);
                m_stepReachParameter = Mathf.Lerp(m_stepReachParameterBase, 1, (Time.time-m_lastRightClickTime) * m_stepReachLerpSpeed);
                break;
            case BipedAction.Idle:
                break;
            case BipedAction.Squart:
                m_stepCoreParameter = 0;
                m_stepReachParameter = 0;
                break;
        }
    }


    public void CalculateMouseDirection(Vector3 characterPos, Vector2 mouseCameraPos)
    {
        Vector2 characterCameraPos = m_mainCamera.WorldToScreenPoint(characterPos);
        Vector2 diff =  mouseCameraPos- characterCameraPos;
        // 计算xz平面上的方向向量
        if (diff.magnitude >= characterCirclethreshold||DragRootToMouseDir == Vector3.zero)
        {
            DragRootToMouseDir = new Vector3(diff.x,0,diff.y);
            DragRootToMouseDir = DragRootToMouseDir.normalized;
            float angle = m_mainCamera.transform.rotation.eulerAngles.y;
            DragRootToMouseDir = Quaternion.AngleAxis(angle, Vector3.up) * DragRootToMouseDir;
        }

    }

    void UpdateBool()
    {
        isFootRotateXY = alignLegStepSupportAngleBase > alignLegStepSupportAngleBaseMax * legIdelSupportAngleRitio;
    }

    private void UpdateVirtualSkeletonPosRoot()
    {
        if (BipedState == BipedAction.LeftStep)
        {
            virtualSkeletonPositions[m_rightFootIndex] = physicsSkeleton[m_rightFootIndex].position;
        }
        else if (BipedState == BipedAction.RightStep)
        { 
            virtualSkeletonPositions[m_leftFootIndex] = physicsSkeleton[m_leftFootIndex].position;
        }
        else if (BipedState == BipedAction.Idle || BipedState == BipedAction.Squart)
        {
            for (int i = 0; i < 5; i++) virtualSkeletonPositions[i] = physicsSkeleton[i].position;
        }
    }

    private void FixedUpdate()
    {
        //input
        SetBipedState();
        UpdateParameter();
        UpdateBool();
        SetStepAngleBase();
        UpdateVirtualSkeletonPosRoot();
        UpdateActionDir();
        //function
        UpdateTarget();
    }

    void UpdateTarget()
    {
        if (BipedState == BipedAction.LeftStep)
        {
            SetLeftStep();
        }
        else if (BipedState == BipedAction.RightStep)
        {
            SetRightStep();
        }
        else if (BipedState == BipedAction.Idle)
        {
            Setidle();
        }
        else if (BipedState == BipedAction.Squart)
        {
            SetSquart();
        }
    }

    //function
    //状态切换！！所有先回归idle
    private void SetBipedState()
    {
        if (Time.time - lastSquartChange <= squartChangeTime)
        {
            BipedState = BipedAction.Squart;
            return;
        }
        //双脚不能都浮空
        if (isPressLeft && isPressRight)
        {
            BipedState = BipedAction.Squart;
            lastSquartChange = Time.time;
        }
        else if (!isPressLeft && !isPressRight) BipedState = BipedAction.Idle;
        else if (isPressLeft)
        {
            BipedState = BipedAction.LeftStep;
        }
        else
        {
            BipedState = BipedAction.RightStep;
        }
    }
    //step下 姿势的几个角度
    private void SetStepAngleBase()
    {
        alignCoreAngleBase = m_stepCoreParameter * alignCoreAngleBaseMax;
        alignLegStepReachAngleBase = m_stepReachParameter * alignLegStepReachAngleBaseMax;
        //为了落下平衡 c-a=b,suppot不随关节转
        //alignLegStepSupportAngleBase = Mathf.Lerp(m_stepReachParameter, 1, Time.deltaTime * m_alignLegStepReachAngleBaseLerpSpeed)*alignLegStepSupportAngleBaseMax;
        alignLegStepSupportAngleBase = alignLegStepReachAngleBase - alignCoreAngleBase;
        alignLegStepSupportAngleBase = Mathf.Clamp(alignLegStepSupportAngleBase,0, alignLegStepSupportAngleBaseMax);
    }

    //注意不要有缩放，对于骨骼的transform只积累位移上面的变化（通过局部xy平面上的旋转），用物理拟合的时候，对physics上再考虑rotation
    //记录旋转，此处旋转都是自旋，是位移通过旋转（子在父节点下）积累的。angle用于1.自己旋转2.给父子相对位置旋转，得到位移
    //MoveStatic=true：相对位移一定(不随旋转变)，有位移的积累效果；false：标准的父子运动；parent和children相同:自身旋转
    //xy平面（z方向），是从零开始，实时更新，不写入；xz平面，记录；yz平面，头部的旋转！！
    //跟三个角相关的内容是——从0开始initial去做，因为三个角都是从0开始去计算的！！！但是旋转是通过鼠标，每帧更新！！代码前，思考哪些是要覆盖的，哪些从零开始
    // private void PosChangeForChild(int parentTransform, int childTransform, float angle,Vector3 localAxis,bool MoveStatic=false,bool RotateStatic=false,bool RotateXY=true)
    // {
    //     // Vector3[] localPositions;
    //     // Quaternion[] LocalRotations;
    //     // // 传入的时候已经知道谁是父母了
    //     // if (rootNow == RootType.leftFoot)
    //     // {
    //     //     localPositions = leftLocalPositions;
    //     //     LocalRotations = leftLocalRotations;
    //     // }else if (rootNow == RootType.RightFoot)
    //     // {
    //     //     localPositions = rightLocalPositions;
    //     //     LocalRotations = rightLocalRotations;
    //     // }
    //     // else
    //     // {
    //     //     localPositions = coreLocalPositions;
    //     //     LocalRotations = coreLocalRotations;
    //     // }
    //     //保持在xy平面，稳定;前方是世界坐标下的，在世界中直接指定通行位置（模型局部坐标朝着前方）
    //     //rotation沿着y旋转（从零开始，模型朝向是forward），angle沿着z旋转（从零开始）！！！
    //     Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, localAxis);
    //     // 计算子节点相对于父节点的位置（initial），转换到在xy平面上的相对位置
    //     Vector3 relativePosition;
    //     Quaternion selfRotation;
    //     //局部与偏移方向最后得到的相对坐标,每次都重启，不需要write initialPositon！！
    //     relativePosition = rotation * (initialPositions[childTransform]-initialPositions[parentTransform]);
    //
    //     // 局部坐标系下的旋转,导致position的变化,相对静止下，相对的位移没有旋转的变化
    //     if(!MoveStatic) relativePosition = Quaternion.AngleAxis(angle, localAxis) * relativePosition;
    //     //必须要rotate后，旋转angle才在局部的xy方向上旋转
    //     if (!RotateStatic)
    //     {
    //         //写入,y上的旋转,记载上一帧rotation即可！！！
    //         writeRotations[childTransform] = rotation *  initialRotations[childTransform];
    //         
    //         if (RotateXY) selfRotation = Quaternion.AngleAxis(angle, localAxis) * writeRotations[childTransform];
    //         else selfRotation = writeRotations[childTransform];
    //         
    //     }
    //     else
    //     {
    //         //脚相对于其他旋转（绕y）的整体的rotate，这个rotate的逆刚好对应脚的局部坐标系下沿y的旋转
    //         selfRotation = writeRotations[childTransform];
    //         initialRotations[childTransform] = Quaternion.Inverse(rotation);
    //     }
    //
    //     // 将计算出的相对位置应用到子节点上,插值还需要一个数组，要保证准确skeleton的准确性
    //     virtualSkeleton[childTransform].position = virtualSkeleton[parentTransform].position + relativePosition;
    //     //virtualSkeleton[childTransform].rotation = selfRotation;
    //     //为了保证一直在一个
    //     //lerpSkeletonPositons[childTransform] = Vector3.Slerp(lerpSkeletonPositons[childTransform],virtualSkeleton[childTransform].position,Time.deltaTime * m_skeletonLerpSpeed);
    //     //lerpSkeletonRotations[childTransform] = Quaternion.Slerp(lerpSkeletonRotations[childTransform],virtualSkeleton[childTransform].rotation,Time.deltaTime * m_skeletonLerpSpeed);
    //     // //位置不一定正确对于动画来说lerpSkeletonPositons[parentTransform] + relativePosition
    //     // lerpSkeletonPositons[childTransform] = Vector3.Slerp(lerpSkeletonPositons[childTransform],lerpSkeletonPositons[parentTransform] + relativePosition,Time.deltaTime * m_skeletonLerpSpeed);
    //     // //原本怎么转，不去影响，用initialRotations
    //     // lerpSkeletonRotations[childTransform] = Quaternion.Slerp(lerpSkeletonRotations[childTransform],initialRotations[parentTransform] * relativeRotation,Time.deltaTime * m_skeletonLerpSpeed);
    // }
    
    //位置position根据父子计算，每次只用更新root的位置（parent）即可
    private void GetTargetPosition(int parentIndex, int childIndex, float localAngleZ,Quaternion rotationY)
    {
        Vector3 relativePosition;
        //initial是local下的坐标差，local是朝前的
        relativePosition =rotationY*Quaternion.AngleAxis(localAngleZ, Vector3.forward) * (initialPositions[childIndex]-initialPositions[parentIndex]);
        //以parent的postion作为基准，在抬脚时，左脚的实时位移作为基准（把virtualSkeleton[root]=physicsSkeleton[root]）
        virtualSkeletonPositions[childIndex] = virtualSkeletonPositions[parentIndex] + relativePosition;
        //位置的插值，动画的平滑过渡-》lerpSkeleton作为物理追随计算的目标
        targetPositions[childIndex] = Vector3.Lerp(targetPositions[childIndex],virtualSkeletonPositions[childIndex],Time.deltaTime * m_skeletonLerpSpeed);
    }

    public void GetTargetDirections(int index,float localAngleZ,Quaternion rotationY,bool rotateXY=true)
    {
        Vector3 relativeDirection;
        if (!rotateXY) localAngleZ = 0;
        relativeDirection =rotationY* Quaternion.AngleAxis(localAngleZ, Vector3.forward) * initialDirections[index];
        virtualSkeletonDirections[index] = relativeDirection;
        targetDirections[index] = Vector3.Slerp(targetDirections[index],virtualSkeletonDirections[index],Time.deltaTime * m_skeletonLerpSpeed);
    }

    private void getLimitRotationY(Vector3 localAxis)
    {
        Quaternion prevRotationY = rotationY;
        rotationY = Quaternion.FromToRotation(Vector3.forward, localAxis);
        float angle = Quaternion.Angle(prevRotationY, rotationY);
        if (angle > maxAngleY)
        {
            Quaternion rotation = Quaternion.AngleAxis(maxAngleY, Vector3.up);
            localAxis = rotation * prevRotationY * Vector3.forward;
            rotationY = Quaternion.FromToRotation(Vector3.forward, localAxis);
        }
    }
    private void UpdateActionDir()
    {
        Vector3 localAxis;
        switch (BipedState)
        {
            case BipedAction.LeftStep:
                //RootToMouseDir是脚的朝向
                CalculateMouseDirection(physicsSkeleton[m_rightFootIndex].position, MouseCameraPos);
                localAxis = -Vector3.Cross( DragRootToMouseDir, new Vector3(0, 1, 0));
                getLimitRotationY(localAxis);
                break;
            case BipedAction.RightStep:
                CalculateMouseDirection(physicsSkeleton[m_leftFootIndex].position, MouseCameraPos);
                localAxis = Vector3.Cross( DragRootToMouseDir, new Vector3(0, 1, 0));
                getLimitRotationY(localAxis);
                break;
            case BipedAction.Idle:
                break;
            case BipedAction.Squart:
                CalculateMouseDirection(
                    (physicsSkeleton[m_leftFootIndex].position + physicsSkeleton[m_rightFootIndex].position)/2, MouseCameraPos);
                localAxis =DragRootToMouseDir;   
                getLimitRotationY(localAxis);
                break;
        }
    }
    //不同状态不同的根节点
    private void SetLeftStep()
    {
        //顺时针正，逆时针负
        GetTargetPosition(m_rightFootIndex, m_rightFootIndex, 0,rotationY);
        GetTargetPosition(m_rightFootIndex, m_rightLegIndex, alignLegStepSupportAngleBase,rotationY);
        GetTargetPosition(m_rightLegIndex, m_bodyIndex, -alignCoreAngleBase,rotationY);
        GetTargetPosition(m_bodyIndex, m_leftLegIndex, -alignCoreAngleBase,rotationY);
        GetTargetPosition(m_leftLegIndex, m_leftFootIndex, -alignLegStepReachAngleBase,rotationY);
        GetTargetDirections(m_rightFootIndex,0,rotationY);
        GetTargetDirections(m_rightLegIndex,alignLegStepSupportAngleBase,rotationY);
        GetTargetDirections(m_bodyIndex,-alignCoreAngleBase,rotationY);
        GetTargetDirections(m_leftLegIndex,-alignLegStepReachAngleBase,rotationY);
        GetTargetDirections(m_leftFootIndex,-alignLegStepReachAngleBase+minusForFoot,rotationY);
    }
    private void SetRightStep()
    {
        GetTargetPosition(m_leftFootIndex, m_leftFootIndex, 0,rotationY);
        GetTargetPosition(m_leftFootIndex, m_leftLegIndex, -alignLegStepSupportAngleBase,rotationY);
        GetTargetPosition(m_leftLegIndex, m_bodyIndex, alignCoreAngleBase,rotationY);
        GetTargetPosition(m_bodyIndex, m_rightLegIndex, alignCoreAngleBase,rotationY);
        GetTargetPosition(m_rightLegIndex, m_rightFootIndex, alignLegStepReachAngleBase,rotationY);
        GetTargetDirections(m_leftFootIndex,0,rotationY);
        GetTargetDirections(m_leftLegIndex,-alignLegStepSupportAngleBase,rotationY);
        GetTargetDirections(m_bodyIndex,alignCoreAngleBase,rotationY);
        GetTargetDirections(m_rightLegIndex,alignLegStepReachAngleBase,rotationY);
        GetTargetDirections(m_rightFootIndex,alignLegStepReachAngleBase-minusForFoot,rotationY);
    }
    
    //人物的基本状态，应该和物理（现实表现绑定），也就是转到哪里，就在哪里停下保持稳定
    private void Setidle()
    {
        GetTargetPosition(m_bodyIndex, m_bodyIndex, 0, rotationY);
        GetTargetPosition(m_bodyIndex, m_rightLegIndex, 0, rotationY);
        GetTargetPosition(m_rightLegIndex, m_rightFootIndex, alignLegStepSupportAngleBase, rotationY);
        GetTargetPosition(m_bodyIndex, m_leftLegIndex, 0, rotationY);
        GetTargetPosition(m_leftLegIndex, m_leftFootIndex, -alignLegStepSupportAngleBase, rotationY);
        
        GetTargetDirections(m_bodyIndex, 0, rotationY);
        GetTargetDirections(m_rightFootIndex, alignLegStepSupportAngleBase, rotationY);
        GetTargetDirections(m_rightLegIndex, alignLegStepSupportAngleBase, rotationY);
        GetTargetDirections(m_leftLegIndex, -alignLegStepSupportAngleBase+minusForFoot, rotationY);
        GetTargetDirections(m_leftFootIndex, -alignLegStepSupportAngleBase+minusForFoot, rotationY, isFootRotateXY);
    }

    void SetSquart()
    {
        GetTargetPosition(m_bodyIndex, m_bodyIndex, 0, rotationY);
        GetTargetPosition(m_bodyIndex, m_rightLegIndex, 0, rotationY);
        GetTargetPosition(m_rightLegIndex, m_rightFootIndex, 0, rotationY);
        GetTargetPosition(m_bodyIndex, m_leftLegIndex, 0, rotationY);
        GetTargetPosition(m_leftLegIndex, m_leftFootIndex, 0, rotationY);

        GetTargetDirections(m_bodyIndex, 0, rotationY);
        GetTargetDirections(m_rightLegIndex, 20, rotationY);
        GetTargetDirections(m_rightFootIndex, 0, rotationY,isFootRotateXY);
        GetTargetDirections(m_leftLegIndex, 20, rotationY);
        GetTargetDirections(m_leftFootIndex, 0, rotationY,isFootRotateXY);
    }

    private void OnEnable()
    {
        
        isPressLeft = false; isPressRight=false;
        isClickLeftDouble = false;isClickRightDouble=false;
        BipedState = BipedAction.Idle;
    }
    //output
    // private void OnDrawGizmos()
    // {
    //     Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
    //     if (targetPositions != null)
    //     {
    //         Handles.color = Color.red;
    //         Handles.DrawAAPolyLine(5.0f, targetPositions[m_bodyIndex], targetPositions[m_leftLegIndex],
    //             targetPositions[m_leftFootIndex]);
    //         Handles.DrawAAPolyLine(5.0f, targetPositions[m_bodyIndex], targetPositions[m_rightLegIndex],
    //             targetPositions[m_rightFootIndex]);
    //         for (int i = 0; i < 5; i++)
    //         {
    //             Gizmos.DrawRay(targetPositions[i],targetDirections[i]);
    //         }
    //     }
    //     if (virtualSkeletonPositions != null)
    //     {
    //         Handles.color = Color.black;
    //         Handles.DrawAAPolyLine(5.0f, virtualSkeletonPositions[m_bodyIndex],
    //             virtualSkeletonPositions[m_leftLegIndex], virtualSkeletonPositions[m_leftFootIndex]);
    //         Handles.DrawAAPolyLine(5.0f, virtualSkeletonPositions[m_bodyIndex],
    //             virtualSkeletonPositions[m_rightLegIndex], virtualSkeletonPositions[m_rightFootIndex]);
    //         for (int i = 0; i < 5; i++)
    //         {
    //             Gizmos.DrawRay(virtualSkeletonPositions[i],virtualSkeletonDirections[i]);
    //         }
    //     }
    //     Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
    // }
}
