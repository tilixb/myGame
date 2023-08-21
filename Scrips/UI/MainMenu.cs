using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private Button startGameBtn;
    private Button quitBtn;

    public GameObject loadingScreen;
    public Slider slider;
    public UGUISpriteAnimation animation;

    private void Awake()
    {
        loadingScreen.SetActive(false);
        startGameBtn = transform.GetChild(1).GetComponent<Button>();
        quitBtn = transform.GetChild(2).GetComponent<Button>();
        
        startGameBtn.onClick.AddListener(StartGame);
        quitBtn.onClick.AddListener(QuitGame);
    }

    void StartGame()
    {
        // 显示加载页面
        loadingScreen.SetActive(true);
        // 播放序列帧动画
        animation.Play();
        // 异步加载场景
        StartCoroutine(SceneLoader.Instance.LoadSceneAsync(1, slider));
    }
    
    void QuitGame()
    {
        Application.Quit();
    }
}