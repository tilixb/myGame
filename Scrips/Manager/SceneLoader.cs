using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    public IEnumerator LoadSceneAsync(int sceneIndex, Slider slider)
    {
        // 创建异步操作对象
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;
        // 等待场景加载完成
        // 等待场景加载完成
        while (!operation.isDone)
        {
            // 如果加载进度大于等于0.9，将进度条设置为1
            if (operation.progress >= 0.9f)
            {
                slider.value = 1f;
                operation.allowSceneActivation = true;
            }
            else
            {
                // 使用插值函数平滑更新进度条
                slider.value = Mathf.Lerp(slider.value, 0.9f, Time.deltaTime);
            }

            yield return null;
        }

    }
}