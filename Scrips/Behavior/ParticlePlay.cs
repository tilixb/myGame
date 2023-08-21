using UnityEngine;

public class ParticlePlay : MonoBehaviour
{
    public GameObject particlePrefab; // 特效预制体
    public bool playOnStart = false; // 是否在开始时播放特效
    public bool destroyAfterPlay = true; // 是否在播放完特效后销毁特效对象

    private void Start()
    {
        if (playOnStart)
        {
            PlayParticleEffect();
        }
    }

    public void PlayParticleEffect()
    {
        // 实例化特效预制体
        GameObject particleInstance = Instantiate(particlePrefab, transform.position, Quaternion.identity);

        // 获取特效对象上的ParticleSystem组件
        ParticleSystem particleSystem = particleInstance.GetComponentInChildren<ParticleSystem>();

        if (destroyAfterPlay)
        {
            // 在特效播放完毕后销毁特效对象
            Destroy(particleInstance, particleSystem.main.duration);
        }
    }
}