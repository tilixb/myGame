using System;
using System.Collections;
using UnityEngine;

public class RespawnController : MonoBehaviour
{
    private Model model;
    
    public Material dissolveMaterial;
    public float dissolveSpeed = 0.01f;
    public float dissolveDuration = 1.0f;
    public GameObject deathParticlePrefab;
    public GameObject respawnParticlePrefab;
    public float respawnDelay = 2.0f;
    private WaitForSeconds waitRespawnTime;
    

    private void Start()
    {
        model = Model.Instance;
        waitRespawnTime = new WaitForSeconds(respawnDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Core"))
        {
            StartCoroutine(RespawnPlayer(other.gameObject));
        }
    }

    //go:core
    private IEnumerator RespawnPlayer(GameObject core)
    {
        // Disable player control and play death particle effect
        model.gameObject.SetActive(false);
        InputManager.Instance.NoInput();
        
        // Clear player's velocity
        foreach (var VARIABLE in model.m_physicsSkeletonBehaviour.physicsRigidbodies)
        {
            VARIABLE.velocity = Vector3.zero;
            VARIABLE.angularVelocity = Vector3.zero;
        }
        
        GameObject deathParticle = Instantiate(deathParticlePrefab, core.transform.position, Quaternion.identity);
        
        // Play dissolve effect while waiting for respawnDelay
        float dissolveAmount = 0.0f;
        float dissolveStartTime = Time.time;
        while (Time.time < dissolveStartTime + dissolveDuration)
        {
            dissolveAmount = (Time.time - dissolveStartTime)*dissolveSpeed / dissolveDuration;
            dissolveMaterial.SetFloat("_Strength", dissolveAmount);
            yield return null;
        }
        yield return waitRespawnTime;

        // Destroy death particle effect and respawn player
        Destroy(deathParticle);
        var relativePos= RespawnPosManager.Instance.GetRespawnPos()-core.transform.position;
        for (int i = 0; i < 5; i++) model.m_physicsSkeletonBehaviour.physicsSkeleton[i].position += relativePos;
        model.gameObject.SetActive(true);
        
        EventManager.Instance.CallPlayerRespawned();
        GameSettlementManager.Instance.IncrementDeathCount();

        // Play respawn particle effect
        GameObject respawnParticle = Instantiate(respawnParticlePrefab, core.transform.position, Quaternion.identity);
        Destroy(respawnParticle, respawnDelay);
    }
}