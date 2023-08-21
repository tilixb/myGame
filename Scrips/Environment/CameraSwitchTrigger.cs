using UnityEngine;
using Cinemachine;

public class CameraSwitchTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Core"))
        {
            CameraManager.Instance.SwitchToCamera(vcam);
        }
    }
}