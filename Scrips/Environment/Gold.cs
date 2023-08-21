using UnityEngine;

public class Gold : MonoBehaviour
{
    public GameObject particlePrefab;
    private GameObject particleInstance;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // Play the particle effect
            particleInstance = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            Destroy(particleInstance, particleInstance.GetComponent<ParticleSystem>().main.duration);

            // Notify the ItemManager singleton
            GameSettlementManager.Instance.AddGold();

            // Destroy the gold object
            Destroy(gameObject);
        }
    }
}