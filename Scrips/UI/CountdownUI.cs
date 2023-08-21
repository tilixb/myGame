using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownUI : MonoBehaviour
{
    private TextMeshProUGUI countdownText; // 倒计时文本组件
    public Image background; // 完成时的Image组件
    public Color iniColor; // 完成时的Image组件
    public float duration = 1f; // 向上移动的持续时间
    private Color startColor = Color.gray; // Image初始颜色

    void Start()
    {
        countdownText = GetComponentInChildren<TextMeshProUGUI>();
        iniColor = background.color;
    }

    public void StartCountdown()
    {
        gameObject.SetActive(true);
        background.color = iniColor;
    }

    public void SucessEndCountdown()
    {
        StartCoroutine(AnimateAndHideCompletionImage(Color.green));
    }
    public void EndCountdown()
    {
        StartCoroutine(AnimateAndHideCompletionImage(Color.black));
    }

    public void UpdateCountdownText(float currentTime)
    {
        countdownText.text = Mathf.RoundToInt(currentTime).ToString();
    }

    private IEnumerator AnimateAndHideCompletionImage(Color endColor)
    {
        float elapsedTime = 0f;
        endColor.a = startColor.a;
        // 向上平滑移动并变绿
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            background.color = Color.Lerp(startColor, endColor, t);
            elapsedTime += Time.deltaTime;

            // 等待剩余的动画持续时间
            yield return null;
        }
        background.color = endColor;
        
        // 隐藏CountdownUI
        gameObject.SetActive(false); 
    }
}