using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Core"))
        {
            RespawnPosManager.Instance.SetRespawnPos(transform.parent.position);
        }
    }
}