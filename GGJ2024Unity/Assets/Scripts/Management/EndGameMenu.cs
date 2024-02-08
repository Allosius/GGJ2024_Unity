using System.Collections;
using System.Collections.Generic;
using AllosiusDevCore;
using TMPro;
using UnityEditor;
using UnityEngine;

public class EndGameMenu : MonoBehaviour
{
    public TextMeshProUGUI scoreAmountText;
    
    public TextMeshProUGUI tapirSuccessText;
    public TextMeshProUGUI tapirFailText;
    
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        scoreAmountText.text = "-" + GameManager.Instance.currentScore.ToString() + "$";

        if (GameManager.Instance.tapirIsCaptured)
        {
            tapirSuccessText.gameObject.SetActive(true);
            tapirFailText.gameObject.SetActive(false);
        }
        else
        {
            tapirSuccessText.gameObject.SetActive(false);
            tapirFailText.gameObject.SetActive(true);
        }
    }

    public void MainMenuButton()
    {
        SceneLoader.Instance.ChangeScene(Scenes.MainMenu);
    }

    public void QuitButton()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }
}
