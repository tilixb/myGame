using UnityEngine;

public class Coin : MonoBehaviour
{
    public string playerTag = "Core"; // 玩家的标签

    private CoinSequenceController coinController;
    private ParticlePlay particleController;

    private void Start()
    {
        coinController = FindObjectOfType<CoinSequenceController>();
        particleController = GetComponent<ParticlePlay>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            coinController.OnCoinTriggered();
            particleController.PlayParticleEffect();
            gameObject.SetActive(false);
        }
    }
}