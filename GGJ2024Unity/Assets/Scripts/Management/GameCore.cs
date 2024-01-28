using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

public class GameCore : AllosiusDevUtilities.Singleton<GameCore>
{
    public TapirController tapir { get; protected set; }
    
    public PlayerInteraction player { get; protected set; }

    public float gameDuration = 60;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        tapir = FindObjectOfType<TapirController>();
        player = FindObjectOfType<PlayerInteraction>();

        GameManager.Instance.SetCurrentScore(0);
        GameManager.Instance.SetCurrentTimer(gameDuration);
        
        GameCanvasManager.Instance.UpdateScore();
        GameCanvasManager.Instance.UpdateTimer();
        
    }

    private void Update()
    {
        UpdateTimer();
        
#if UNITY_EDITOR
        
        if (Input.GetKeyDown(SFPSC_KeyManager.QuitPlayMode))
        {
            Debug.Log("Quit Play Mode");
            EditorApplication.ExitPlaymode();
        }
#endif
    }

    public void UpdateTimer()
    {
        if (GameManager.Instance.currentTimer > 0)
        {
            GameManager.Instance.ChangeCurrentTimer(-Time.deltaTime);
        }
    }
}
