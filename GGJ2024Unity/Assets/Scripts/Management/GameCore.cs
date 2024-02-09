using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using AllosiusDevUtilities.Audio;
using AllosiusDevCore;

public class GameCore : AllosiusDevUtilities.Singleton<GameCore>
{
    private bool gameEnded = false;
    
    private IEnumerator _coroutinePlayMusic;
    
    public TapirController tapir { get; protected set; }
    
    public PlayerInteraction player { get; protected set; }

    public float gameDuration = 60;

    public AudioData Mainmusic;
    
    [SerializeField] private SceneData endGameSceneData;

    [SerializeField]
    private string startEventLabel = "Catch the tapir and get him out of the store before time runs out !";
    
    [SerializeField]
    private string victoryEventLabel = "The tapir has been caught ! Congratulations !";
    
    [SerializeField]
    private string defeatEventLabel = "You didn't catch the tapir in time...";
    
    [Space]
    
    [SerializeField] private PopUpText addScorePopUp;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        
        tapir = FindObjectOfType<TapirController>();
        player = FindObjectOfType<PlayerInteraction>();

        GameManager.Instance.SetCurrentScore(0);
        GameManager.Instance.SetCurrentTimer(gameDuration);
        GameManager.Instance.tapirIsCaptured = false;
        
        GameCanvasManager.Instance.UpdateScore();
        GameCanvasManager.Instance.UpdateTimer();

        GameCanvasManager.Instance.SetDisplayEventLabelUI(startEventLabel);

        AudioController.Instance.PlayAudio(Mainmusic);
        
    }

    private void Update()
    {
        UpdateTimer();
    }

    public void UpdateTimer()
    {
        if (GameManager.Instance.currentTimer > 0)
        {
            GameManager.Instance.ChangeCurrentTimer(-Time.deltaTime);
        }
        else if (gameEnded == false)
        {
            gameEnded = true;
            
            EndGame();
        }
    }

    public void PlayNewMusic(AudioData musicData, float musicDuration)
    {
        if (_coroutinePlayMusic != null)
        {
            StopCoroutine(_coroutinePlayMusic);
        }

        AudioController.Instance.StopAllMusics();
        AudioController.Instance.PlayAudio(musicData);
        
        _coroutinePlayMusic = PlayNewMusicCoroutine(musicDuration);
        StartCoroutine(_coroutinePlayMusic);
    }

    private IEnumerator PlayNewMusicCoroutine(float musicDuration)
    {
        yield return new WaitForSeconds(musicDuration);
        
        AudioController.Instance.StopAllMusics();
        AudioController.Instance.PlayAudio(Mainmusic);
    }
    
    public void CreateScorePopUp(Transform target, int amount)
    {
        PopUpText textToInstantiate = addScorePopUp;

        var myNewScore = Instantiate(textToInstantiate);
        //Vector2 screenPosition = Camera.main.WorldToScreenPoint(target.position);


        myNewScore.transform.SetParent(Camera.main.transform, true);
        myNewScore.transform.position = target.position;
        myNewScore.GetComponent<PopUpText>().SetPoints(amount);
    }

    public void EndGame()
    {
        gameEnded = true;

        if (GameManager.Instance.tapirIsCaptured)
        {
            GameCanvasManager.Instance.SetDisplayEventLabelUI(victoryEventLabel);
        }
        else
        {
            GameCanvasManager.Instance.SetDisplayEventLabelUI(defeatEventLabel);
        }

        StartCoroutine(CoroutineEndGame());
    }

    private IEnumerator CoroutineEndGame()
    {
        yield return new WaitForSeconds(3.0f);
        
        SceneLoader.Instance.ChangeScene(endGameSceneData.sceneToLoad);
    }
}
