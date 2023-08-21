using UnityEngine;
using Cinemachine;

public class CameraManager : Singleton<CameraManager>
{
    public CinemachineVirtualCamera[] cameras;

    

    public void SwitchToCamera(CinemachineVirtualCamera vcam)
    {
        // 将虚拟相机按照优先级进行排序
        System.Array.Sort(cameras, (a, b) => b.Priority.CompareTo(a.Priority));

        // 找到传入的虚拟相机在数组中的位置
        int index = -1;
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] == vcam)
            {
                index = i;
                break;
            }
        }

        // 如果传入的虚拟相机不在数组中，则直接返回
        if (index == -1)
        {
            return;
        }
        int temp = cameras[0].Priority;
        cameras[0].Priority = vcam.Priority;
        vcam.Priority = temp;
        
    }
}