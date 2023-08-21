using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveStoneParticle : MonoBehaviour
{
    private ParticleSystem[] particleSystems;
    public float particleTime=5.0f;
    private WaitForSeconds waitForParticleSeconds;

    private void Start()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        waitForParticleSeconds = new WaitForSeconds(particleTime);
        EventManager.Instance.OnButtonSequenceCompleted += HandleStoneParticle;
    }

    void OnDestroy()
    {
        EventManager.Instance.OnButtonSequenceCompleted -= HandleStoneParticle;
    }
    private void HandleStoneParticle()
    {
        StartCoroutine(PlayParticleEffect());
    }

    private IEnumerator PlayParticleEffect()
    {
        foreach (var VARIABLE in particleSystems)
        {
            VARIABLE.Play();
        }

        // Wait for the stone to move
        yield return waitForParticleSeconds;
        foreach (var VARIABLE in particleSystems)
        {
            VARIABLE.Stop();
        }
    }
}
