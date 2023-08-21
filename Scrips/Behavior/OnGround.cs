using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGround : MonoBehaviour
{
    public float groundCheckDistance = 0.1f;
    public bool isGrounded { get; set; }
    public bool IsOnMovingPlatform { get; set; }
    public Vector3 PlatformVelocity { get; set; }
    public LayerMask groundLayers;
    private void FixedUpdate()
    {
        CheckIfGrounded();
    }
    private void CheckIfGrounded()
    {
        RaycastHit hit;
        float distanceToGround = GetComponent<Collider>().bounds.extents.y;
        Vector3 groundCheckPosition = transform.position;

        isGrounded = Physics.Raycast(groundCheckPosition, Vector3.down, out hit, distanceToGround + groundCheckDistance,groundLayers);
        // 检查是否在MovingPlat上
        if (isGrounded && hit.collider.CompareTag("MovingPlat"))
        {
            IsOnMovingPlatform = true;
            Rigidbody platformRigidbody = hit.collider.GetComponentInParent<Rigidbody>();
            if (platformRigidbody != null)
            {
                PlatformVelocity = platformRigidbody.velocity;
            }
        }
        else
        {
            IsOnMovingPlatform = false;
        }
    }
    
}
