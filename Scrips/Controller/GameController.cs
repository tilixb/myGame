using UnityEngine;

public class GameController : Singleton<GameController>
{
    public GameObject pauseMenu; // 暂停菜单的引用
    public bool isLoadingScene = false;
    void Update()
    {
        // 如果按下ESC键，切换暂停状态
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (isLoadingScene) return; // 如果正在加载场景，则不执行暂停操作

        pauseMenu.SetActive(!pauseMenu.activeSelf);

        if (pauseMenu.activeSelf)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f; // 暂停游戏中的时间
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f; // 恢复游戏中的时间
    }
}