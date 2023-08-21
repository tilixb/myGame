using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstraintHip : MonoBehaviour
{
    public Rigidbody hipRigidBody;

    private void Start()
    {
        hipRigidBody.constraints =  RigidbodyConstraints.FreezeRotation;
    }
    
}
