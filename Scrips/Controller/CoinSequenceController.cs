using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CoinSequenceController : MonoBehaviour
{
    public GameObject[] coins; // 一堆物体，包含触发器的物体
    public GameObject star; // Star物体
    public float resetTime = 12f; // 重置时间

    private int coinCount; // 已触发的触发器数量
    private int totalCoinCount; // 总触发器数量
    private float resetTimer; // 重置计时器

    private bool isSuccess=false;
    public CountdownUI countdownUI;

    private void Start()
    {
        totalCoinCount = coins.Length;
        ResetCoins();
        star.SetActive(false);
    }

    private void Update()
    {
        if (isSuccess)
        {
            if (coins != null)
            {
                Debug.Log(coins.Length);
                countdownUI.SucessEndCountdown();
                foreach (var VARIABLE in coins)
                {
                    Destroy(VARIABLE);
                }

                coins = null;
            }

            return;
        }
        if (coinCount > 0)
        {
            countdownUI.UpdateCountdownText(resetTimer);
            resetTimer -= Time.deltaTime;
            if (resetTimer <= 0f)
            {
                ResetCoins();
                countdownUI.EndCountdown();
            }
        }
    }

    public void OnCoinTriggered()
    {
        coinCount++;

        if (coinCount == 1)
        {
            ActivateAllCoins();
            resetTimer = resetTime;
            countdownUI.StartCountdown();
        }
        else if (coinCount >= totalCoinCount)
        {
            ActivateStar();
        }
    }

    public void ResetCoins()
    {
        coinCount = 0;

        for (int i = 0; i < coins.Length; i++)
        {
            coins[i].SetActive(i == 0);
        }
    }

    private void ActivateAllCoins()
    {
        foreach (GameObject coin in coins)
        {
            coin.SetActive(true);
        }
    }

    private void ActivateStar()
    {
        star.SetActive(true);
        isSuccess = true;
    }
}