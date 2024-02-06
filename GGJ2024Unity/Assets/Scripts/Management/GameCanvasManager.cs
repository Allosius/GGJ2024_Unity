using System;
using System.Collections;
using System.Collections.Generic;
using AllosiusDevUtilities;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameCanvasManager : Singleton<GameCanvasManager>
{
    private IEnumerator _coroutineWaitDisplayEventUi;
    
    public TextMeshProUGUI scoreAmountText;
    public TextMeshProUGUI timerAmountText;
    
    public CanvasGroup displayEventUi;
    public TextMeshProUGUI displayEventLabelText;
    public float displayEventUiFadeDuration = 1f;
    public float displayEventDuration = 1.5f;

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
    
    public void SetDisplayEventLabelUI(string newLabel)
    {
        displayEventUi.DOKill();
        displayEventLabelText.text = newLabel;
        displayEventUi.gameObject.SetActive(true);
        displayEventUi.DOFade(1.0f, displayEventUiFadeDuration).OnComplete(WaitDisplayEventUIDuration);
    }

    private void WaitDisplayEventUIDuration()
    {
        if (_coroutineWaitDisplayEventUi != null)
        {
            StopCoroutine(_coroutineWaitDisplayEventUi);
        }
        _coroutineWaitDisplayEventUi = CoroutineWaitSetDisplayEventLabelUI();
        StartCoroutine(_coroutineWaitDisplayEventUi);
    }

    private IEnumerator CoroutineWaitSetDisplayEventLabelUI()
    {
        yield return new WaitForSeconds(displayEventDuration);

        displayEventUi.DOFade(0.0f, displayEventUiFadeDuration).OnComplete(DeactiveDisplayEventUi);
    }

    private void DeactiveDisplayEventUi()
    {
        displayEventUi.gameObject.SetActive(false);
    }
}
