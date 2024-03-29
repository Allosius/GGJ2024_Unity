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

    
    public bool achievementSneeze { get; protected set; }
    
    public bool achievementTooDestructions { get; protected set; }
    public int AchievementTooDestructionsConditionAmount => achievementTooDestructionsConditionAmount;
    
    public bool achievementCatchThemAll { get; protected set; }
    
    public bool achievementUseEggPlant { get; protected set; }
    
    public bool achievementUseBook { get; protected set; }
    
    public bool achievementUsePlush { get; protected set; }
    
    
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
    
    
    [SerializeField]
    private string achievementSneezeEventLabel = "Sneezy - Make it sneeze !";
    
    [SerializeField]
    private string achievementTooDestructionsEventLabel = "It's Over 9000 ! - Destroy for more than 9000 $";
    [SerializeField] private int achievementTooDestructionsConditionAmount = 9000;
    
    [SerializeField]
    private string achievementCatchThemAllEventLabel = "Catch'them'All - I wanna be the very best";
    
    [SerializeField]
    private string achievementUseEggPlantEventLabel = "Not that Way - Affraid with the eggplant";
    
    [SerializeField]
    private string achievementUseBookEventLabel = "Cannot read Bro - Show her books";
    
    [SerializeField]
    private string achievementUsePlushEventLabel = "Love is on the way - Plushes are so cute, aren't they ?";
    
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


        myNewScore.transform.position = target.position;
        myNewScore.transform.SetParent(transform, true);
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

    public void SetAchievementSneezeValue(bool value)
    {
        bool tempAchievementSneeze = achievementSneeze;
        achievementSneeze = value;

        if (achievementSneeze && tempAchievementSneeze == false)
        {
            GameCanvasManager.Instance.SetDisplayEventLabelUI(achievementSneezeEventLabel);
        }
    }
    
    public void SetAchievementTooDestructionsValue(bool value)
    {
        bool tempAchievementTooDestructions = achievementTooDestructions;
        achievementTooDestructions = value;

        if (achievementTooDestructions && tempAchievementTooDestructions == false && GameManager.Instance.currentScore >= achievementTooDestructionsConditionAmount)
        {
            GameCanvasManager.Instance.SetDisplayEventLabelUI(achievementTooDestructionsEventLabel);
        }
    }
    
    public void SetAchievementCatchThemAllValue(bool value)
    {
        bool tempAchievementCatchThemAll = achievementCatchThemAll;
        achievementCatchThemAll = value;

        if (achievementCatchThemAll && tempAchievementCatchThemAll == false)
        {
            GameCanvasManager.Instance.SetDisplayEventLabelUI(achievementCatchThemAllEventLabel);
        }
    }
    
    public void SetAchievementUseEggPlant(bool value)
    {
        bool tempAchievementUseEggPlant = achievementUseEggPlant;
        achievementUseEggPlant = value;

        if (achievementUseEggPlant && tempAchievementUseEggPlant == false)
        {
            GameCanvasManager.Instance.SetDisplayEventLabelUI(achievementUseEggPlantEventLabel);
        }
    }
    
    public void SetAchievementUseBook(bool value)
    {
        bool tempAchievementUseBook = achievementUseBook;
        achievementUseBook = value;

        if (achievementUseBook && tempAchievementUseBook == false)
        {
            GameCanvasManager.Instance.SetDisplayEventLabelUI(achievementUseBookEventLabel);
        }
    }
    
    public void SetAchievementUsePlush(bool value)
    {
        bool tempAchievementUsePlush = achievementUsePlush;
        achievementUsePlush = value;

        if (achievementUsePlush && tempAchievementUsePlush == false)
        {
            GameCanvasManager.Instance.SetDisplayEventLabelUI(achievementUsePlushEventLabel);
        }
    }
}
