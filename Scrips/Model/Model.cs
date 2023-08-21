using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : Singleton<Model>
{
    [HideInInspector]public PhysicsSkeletonBehaviour m_physicsSkeletonBehaviour;
    [HideInInspector]public VirtualSkeletonBehaviour m_VirtualSkeletonBehaviour;
    
   
    private void Start()
    {
        m_physicsSkeletonBehaviour = GetComponentInChildren<PhysicsSkeletonBehaviour>();
        m_VirtualSkeletonBehaviour = GetComponentInChildren<VirtualSkeletonBehaviour>();
    }
}
