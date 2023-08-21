using System;
using System.Collections;
using UnityEngine;
public class GameSettlementManager : Singleton<GameSettlementManager>
{
    
    public EndMenu endUI;
    
    private int deathCount = 0;

    public void IncrementDeathCount()
    {
        deathCount++;
    }

    public int GetDeathCount()
    {
        return deathCount;
    }
    
    
    private int itemNum = 0;
    public void AddGold()
    {
        itemNum++;
    }
    public int GetItemNum()
    {
        return itemNum;
    }
    
    private float startTime;
    private float endTime;
    private void Start()
    {
        startTime = Time.time;
    }

    public void SetEndTime()
    {
        endTime = Time.time;
    }

    public float GetAllTime()
    {
        SetEndTime();
        return endTime - startTime;
    }
    public void EndGame()
    {
        StartCoroutine(ShowGameOverUIWithDelay());
        // 在这里添加游戏结束时的逻辑，例如显示游戏结束 UI、计算分数等
    }
    public IEnumerator ShowGameOverUIWithDelay()
    {
        yield return new WaitForSeconds(3);
        endUI.gameObject.SetActive(true);
        endUI.itemNumText.text = itemNum.ToString();
        endUI.endGameTimeText.text = Mathf.RoundToInt(GetAllTime()).ToString();
        endUI.deathCountText.text = deathCount.ToString();
    }
}