using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private Button continueGameBtn;
    private Button backBtn;

    public GameObject loadingScreen;
    public Slider slider;
    public UGUISpriteAnimation animation;

    private void Awake()
    {
        loadingScreen.SetActive(false);
        continueGameBtn = transform.GetChild(0).GetComponent<Button>();
        backBtn = transform.GetChild(1).GetComponent<Button>();
        
        continueGameBtn.onClick.AddListener(ContinueGame);
        backBtn.onClick.AddListener(BackMenu);
    }

    void ContinueGame()
    {
        
        Time.timeScale = 1f; // 恢复游戏中的时间
        gameObject.SetActive(false); // 隐藏暂停菜单
    }
    
    void BackMenu()
    {
        Time.timeScale = 1f;
        // 显示加载页面
        loadingScreen.SetActive(true);
        GameController.Instance.isLoadingScene = true;
        
        // 播放序列帧动画
        animation.Play();
        // 异步加载场景
        StartCoroutine(SceneLoader.Instance.LoadSceneAsync(0, slider));
    }
}