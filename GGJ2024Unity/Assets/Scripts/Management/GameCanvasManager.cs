using System;
using System.Collections;
using System.Collections.Generic;
using AllosiusDevUtilities;
using TMPro;
using UnityEngine;

public class GameCanvasManager : Singleton<GameCanvasManager>
{
    public TextMeshProUGUI scoreAmountText;
    public TextMeshProUGUI timerAmountText;

    private void Start()
    {
        GameManager.Instance.SetScoreEvent += UpdateScore;
        GameManager.Instance.SetTimerEvent += UpdateTimer;
    }

    private void OnDestroy()
    {
        GameManager.Instance.SetScoreEvent -= UpdateScore;
        GameManager.Instance.SetTimerEvent -= UpdateTimer;
    }

    public void UpdateScore()
    {
        scoreAmountText.text = GameManager.Instance.currentScore.ToString();
    }

    public void UpdateTimer()
    {
        int timer = (int)GameManager.Instance.currentTimer;

        int seconds = timer % 60;
        int minutes = timer / 60;
        timerAmountText.text = $"{minutes:00}:{seconds:00}";
    }
}
