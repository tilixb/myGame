using UnityEngine;
using UnityEngine.UI;

public class ImageRotator : MonoBehaviour
{
    public float rotationSpeed = 50f; // 旋转速度，可以自行设置
    public float minAngle = -45f; // 最小旋转角度，可以自行设置
    public float maxAngle = 45f; // 最大旋转角度，可以自行设置

    private RectTransform rectTransform;
    private float currentAngle;
    private int rotationDirection = 1; // 旋转方向，1为顺时针，-1为逆时针

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        currentAngle = rectTransform.eulerAngles.z;
    }

    void Update()
    {
        // 根据旋转方向和速度更新旋转角度
        currentAngle += rotationDirection * rotationSpeed * Time.deltaTime;

        // 如果达到最小或最大角度，改变旋转方向
        if (currentAngle >= maxAngle)
        {
            rotationDirection = -1;
            currentAngle = maxAngle;
        }
        else if (currentAngle <= minAngle)
        {
            rotationDirection = 1;
            currentAngle = minAngle;
        }

        // 更新RectTransform的旋转角度
        rectTransform.rotation = Quaternion.Euler(0, 0, currentAngle);
    }
}