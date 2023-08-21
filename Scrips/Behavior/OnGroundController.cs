using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundController : MonoBehaviour
{
    
    [SerializeField] private OnGround leftGround;
    [SerializeField] private OnGround rightGround;
    
    public GameObject tutorialUI;

    private bool isFirstLand = true;
    public GameObject landingParticlePrefab;
    public Transform particleTransform;
    private void FixedUpdate()
    {
        if (leftGround.isGrounded || rightGround.isGrounded)
        {
            Model.Instance.m_physicsSkeletonBehaviour.notInTheAir = true;
        }
        else Model.Instance.m_physicsSkeletonBehaviour.notInTheAir = false;
        
        // 检查是否有一个脚在MovingPlat上
        if (leftGround.IsOnMovingPlatform)
        {
            Model.Instance.m_physicsSkeletonBehaviour.AddForceBasedOnPlatformVelocity(leftGround.PlatformVelocity);
        }else if (rightGround.IsOnMovingPlatform)
        {
            Model.Instance.m_physicsSkeletonBehaviour.AddForceBasedOnPlatformVelocity(rightGround.PlatformVelocity);
        }
        else
        {
            Model.Instance.m_physicsSkeletonBehaviour.ResetPlatformVelocity();
        }
    }
    
    private void Update()
    {
        if (isFirstLand)
        {
            if (leftGround.isGrounded && rightGround.isGrounded)
            {
                InputManager.Instance.HaveInput();
                GameObject landingParticle =
                    Instantiate(landingParticlePrefab, particleTransform.position, Quaternion.FromToRotation(Vector3.up, Vector3.forward));
                Destroy(landingParticle, 2.0f);
                isFirstLand = false;
                
                tutorialUI.SetActive(true);
                //教学
            }
        }
    }
}
