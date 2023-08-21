using System;
using UnityEngine;
using System.Collections.Generic;
public class AttatchForce : MonoBehaviour
{
    
    public float pullForce = 10.0f;
    public LayerMask playerLayer;
    private Vector3 attachPos;

    private ButtonPlat buttonPlat;
    private List<Rigidbody> footRigidbodies = new List<Rigidbody>();
    public Collider otherCollider;
    private void Start()
    {
        buttonPlat = GetComponent<ButtonPlat>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerLayer == (playerLayer | (1 << other.gameObject.layer)) && other.CompareTag("Foot"))
        {
            Rigidbody footRigidbody = other.GetComponent<Rigidbody>();
            if (footRigidbody != null)
            {
                footRigidbodies.Remove(footRigidbody);
            }

            if (footRigidbodies.Count == 0) buttonPlat.isButtonActive = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (playerLayer == (playerLayer | (1 << other.gameObject.layer)) && other.CompareTag("Foot"))
        {
            if(Model.Instance.m_VirtualSkeletonBehaviour.BipedState ==
               VirtualSkeletonBehaviour.BipedAction.Idle||Model.Instance.m_VirtualSkeletonBehaviour.BipedState ==
               VirtualSkeletonBehaviour.BipedAction.Squart)
            {
                Rigidbody footRigidbody = other.GetComponent<Rigidbody>();
                if (footRigidbody != null && !footRigidbodies.Contains(footRigidbody)&& otherCollider.bounds.Intersects(other.bounds))
                {
                    footRigidbodies.Add(footRigidbody);
                    if (footRigidbodies.Count == 1) buttonPlat.ButtonActive();
                }
            }
            
            if (footRigidbodies.Count == 0) buttonPlat.isButtonActive = false;
            else
            {
                Rigidbody activeFootRigidbody = footRigidbodies[0];
                Vector3 pullDirection = (transform.position - activeFootRigidbody.transform.position).normalized;
                activeFootRigidbody.AddForce(pullDirection * pullForce, ForceMode.Acceleration);
            }
        }
    }
    private void FixedUpdate()
    {
        UpdateQueue();

    }

    private void UpdateQueue()
    {
        if (footRigidbodies.Count > 0)
        {
            Rigidbody firstFootRigidbody = footRigidbodies[0];
            int index = Model.Instance.m_physicsSkeletonBehaviour.GetRigidIndex(firstFootRigidbody);
            if (Model.Instance.m_VirtualSkeletonBehaviour.BipedState == VirtualSkeletonBehaviour.BipedAction.LeftStep &&
                index == 0)
            {
                footRigidbodies.RemoveAt(0);
                if (footRigidbodies.Count == 1)
                {
                    buttonPlat.ButtonActive();
                }
            }
            else if (Model.Instance.m_VirtualSkeletonBehaviour.BipedState ==
                     VirtualSkeletonBehaviour.BipedAction.RightStep && index == 4)
            {
                footRigidbodies.RemoveAt(0);
                if (footRigidbodies.Count == 1)
                {
                    buttonPlat.ButtonActive();
                }
            }

            
        }
    }
}