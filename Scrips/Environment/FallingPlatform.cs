using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 2.0f;
    public float respawnDelay = 4.0f;
    public LayerMask playerLayer;

    private Vector3 initialPosition;
    public Transform artTransform;
    private Rigidbody platformRigidbody;
    private BoxCollider platformCollider;
    private bool isFalling = false;
    private new ParticleSystem particleSystem;
    private WaitForSeconds waitForOneSecond;

    private Quaternion initialGoRotation;
    private Quaternion originalRotation;
    private void Start()
    {
        initialPosition = transform.position;
        platformRigidbody = GetComponent<Rigidbody>();
        platformCollider = GetComponent<BoxCollider>();
        platformRigidbody.isKinematic = true;

        particleSystem = GetComponentInChildren<ParticleSystem>();
        waitForOneSecond = new WaitForSeconds(1.0f);
        
        // Save the platform's original rotation
        originalRotation = artTransform.rotation;
        initialGoRotation = transform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isFalling && playerLayer == 1 << collision.gameObject.layer)
        {
            particleSystem.Play();
            StartCoroutine(Shake());
            StartCoroutine(FallAndRespawn());
        }
    }

    private IEnumerator FallAndRespawn()
    {
        isFalling = true;
        yield return new WaitForSeconds(fallDelay);

        platformRigidbody.isKinematic = false;
        platformCollider.enabled = false;
        particleSystem.Stop();
        yield return new WaitForSeconds(respawnDelay);

        while (true)
        {
            Collider[] colliders = Physics.OverlapBox(initialPosition, platformCollider.size / 2, initialGoRotation, playerLayer);
            if (colliders.Length == 0)
            {
                platformRigidbody.isKinematic = true;
                platformRigidbody.velocity = Vector3.zero;
                platformCollider.enabled = true;
                transform.position = initialPosition;
                isFalling = false;
                break;
            }

            yield return waitForOneSecond;
        }
    }
    private IEnumerator Shake()
    {
        // Set the initial shake intensity and duration
        float intensity = 5.0f;
        float shakeInterval = 0.05f;

        // Shake once
        DoShake(intensity,intensity);

        // Wait for 1 second
        yield return waitForOneSecond;

        // Shake repeatedly with the specified interval until fallDelay - 1 seconds have passed
        float elapsedTime = 0;
        while (elapsedTime < fallDelay - 1.0f)
        {
            DoShake(-intensity,intensity);
            yield return new WaitForSeconds(shakeInterval);
            elapsedTime += shakeInterval;
        }
        artTransform.rotation = originalRotation;
    }

    private void DoShake(float leftIntensity,float rightIntensity)
    {
        // Apply a random rotation around the forward axis
        float randomAngle = Random.Range(leftIntensity, rightIntensity);
        artTransform.rotation = Quaternion.Slerp(artTransform.rotation,originalRotation * Quaternion.Euler(0, 0, randomAngle),0.5f) ;
    }

}