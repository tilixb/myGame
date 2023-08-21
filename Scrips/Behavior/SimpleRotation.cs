using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up; // 旋转轴，默认为Y轴
    public float rotationSpeed = 30f; // 每秒旋转的角度

    private void Update()
    {
        // 计算每帧应该旋转的角度
        float angle = rotationSpeed * Time.deltaTime;
        // 使用Quaternion.Euler方法根据角度和旋转轴计算旋转四元数
        Quaternion rotation = Quaternion.Euler(rotationAxis * angle);
        // 将当前物体的旋转与计算出的旋转四元数相乘，得到新的旋转
        transform.rotation = rotation * transform.rotation;
    }
}