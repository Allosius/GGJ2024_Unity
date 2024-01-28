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
    
    public TapirController tapir { get; protected set; }
    
    public PlayerInteraction player { get; protected set; }

    public float gameDuration = 60;

    public AudioData Mainmusic;
    
    [SerializeField] private SceneData endGameSceneData;

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

    public void EndGame()
    {
        gameEnded = true;
        
        SceneLoader.Instance.ChangeScene(endGameSceneData.sceneToLoad);
    }
}
