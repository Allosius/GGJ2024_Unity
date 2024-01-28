using System;
using System.Collections;
using System.Collections.Generic;
using AllosiusDevUtilities;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int currentScore { get; set; }
    
    public float currentTimer { get; set; }

    public event Action SetScoreEvent;
    public event Action SetTimerEvent;

    public void SetCurrentScore(int value)
    {
        currentScore = value;
        SetScoreEvent?.Invoke();
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        SetScoreEvent?.Invoke();
    }

    public void SetCurrentTimer(float value)
    {
        currentTimer = value;
        SetTimerEvent?.Invoke();
    }

    public void ChangeCurrentTimer(float amount)
    {
        SetCurrentTimer(currentTimer + amount);
    }
}
