using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceBalance : MonoBehaviour
{
    public Vector3 upConstantForce;
    public Vector3 downConstantForce;
    public Vector3 leftDownConstantForce;
    public Vector3 RightDownConstantForce;

    public Rigidbody head;
    public Rigidbody pivels;
    public Rigidbody leftFeet;
    public Rigidbody rightFeet;

    private Rigidbody[] rigidbodies;
    public float massAll=0;

    private void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var VARIABLE in rigidbodies)
        {
            massAll += VARIABLE.mass;
        }
    }

    void Update()
    {
        if(head!=null)
        head.AddForce(upConstantForce,ForceMode.Force);
        if(pivels!=null)
        pivels.AddForce(downConstantForce,ForceMode.Force);
        if(leftFeet!=null)
        leftFeet.AddForce(leftDownConstantForce,ForceMode.Force);
        if(rightFeet!=null)
        rightFeet.AddForce(RightDownConstantForce,ForceMode.Force);

        
    }
}
