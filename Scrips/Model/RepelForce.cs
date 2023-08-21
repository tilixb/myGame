using System.Collections.Generic;
using UnityEngine;

public class RepelForce : MonoBehaviour
{
    public float repelForce = 10.0f;
    public string repelLayerName = "Player";
    public int objectIndex;

    private int repelLayerMask;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        repelLayerMask = LayerMask.GetMask(repelLayerName);
    }

    private void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, GetComponent<Collider>().bounds.extents.magnitude, repelLayerMask);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject == this.gameObject)
            {
                continue;
            }

            RepelForce otherRepelForce = collider.GetComponent<RepelForce>();
            if (otherRepelForce != null && Mathf.Abs(objectIndex - otherRepelForce.objectIndex) > 1)
            {
                Rigidbody otherRb = collider.GetComponent<Rigidbody>();
                if (otherRb != null)
                {
                    Vector3 direction = this.transform.position - otherRb.transform.position;
                    otherRb.AddForce(direction.normalized * repelForce);
                }
            }
        }
    }
}