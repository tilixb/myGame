using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    private Button backBtn;

    public GameObject loadingScreen;
    public Slider slider;
    public UGUISpriteAnimation animation;
    public TextMeshProUGUI itemNumText; 
    public TextMeshProUGUI endGameTimeText; 
    public TextMeshProUGUI deathCountText; 

    private void Awake()
    {
        loadingScreen.SetActive(false);
        backBtn = transform.GetChild(5).GetComponent<Button>();
        backBtn.onClick.AddListener(BackMenu);
        GameController.Instance.isLoadingScene = true;
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