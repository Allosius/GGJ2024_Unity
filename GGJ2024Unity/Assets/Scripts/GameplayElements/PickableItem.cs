using System.Collections;
using System.Collections.Generic;
using AllosiusDevCore;
using AllosiusDevUtilities.Audio;
using Sirenix.OdinInspector;
using UnityEngine;

public enum AttractionItemType
{
    None,
    TapirSits,
    TapirFlees,
    TapirFollows,
    TapirAttacks,
    PlayMusic,
}

public enum AbsorptionEffectItemType
{
    None,
    InstantSneeze,
    SuperSneeze,
    ChangeTapirMaterial,
    AddAdditionnalsFeedbacks,
}

[RequireComponent(typeof(FeedbacksReader))]
public class PickableItem : MonoBehaviour
{
    private bool hasAlreadyApplyTapirCollisionsBind;
    
    private FeedbacksReader feedbacksReader;
    
    // Reference to the rigidbody
    private Rigidbody rb;

    private Animator animator;
    
    public Rigidbody Rb => rb;
    public Animator Animator => animator;

    public int TapirFillGaugeAmount => tapirFillGaugeAmount;

    public float AbsorbMoveSpeedItem => absorbMoveSpeedItem;
    
    
    //public bool isSneezing { get; set; }
    
    public TapirController currentTapir { get; set; }

    [SerializeField] private bool isCatchThemAllItem;
    
    [SerializeField] private bool isEggPlantItem;

    [SerializeField] private bool isBookItem;
    
    [SerializeField] private bool isPlushItem;
    
    [SerializeField] private AttractionItemType attractionItemType;
    
    [ShowIf("attractionItemType", AttractionItemType.PlayMusic)]
    [SerializeField] private AudioData attractionMusicToPlay;
    
    [ShowIf("attractionItemType", AttractionItemType.PlayMusic)]
    [SerializeField] private float attractionMusicDuration = 10.0f;
    
    [SerializeField] private AbsorptionEffectItemType absorptionEffectItemType;

    [SerializeField] private FeedbacksData[] attractionFeedbacks;
    [SerializeField] private AudioData[] attractionSfx;

    [SerializeField] private int tapirFillGaugeAmount = 10;

    [SerializeField] private float absorbMoveSpeedItem = 10.0f;
    
    [SerializeField] private FeedbacksData[] onTapirCollisionFeedbacks;
    [SerializeField] private AudioData[] onTapirCollisionSfx;

    /// <summary>
    /// Method called on initialization.
    /// </summary>
    private void Awake()
    {
        // Get reference to the rigidbody
        rb = GetComponent<Rigidbody>();
        
        // Get reference to the rigidbody
        animator = GetComponent<Animator>();
        if (animator)
        {
            animator.enabled = false;
        }

        feedbacksReader = GetComponent<FeedbacksReader>();
    }

    public void OnAttractionItem(TapirController tapir)
    {
        for (int i = 0; i < attractionFeedbacks.Length; i++)
        {
            feedbacksReader.ReadFeedback(attractionFeedbacks[i]);
        }
        
        if (attractionSfx.Length > 0)
        {
            AudioController.Instance.PlayRandomAudio(attractionSfx);
        }

        if (GameCore.Instance.achievementUseBook == false && isBookItem)
        {
            GameCore.Instance.SetAchievementUseBook(true);
        }
        
        switch (attractionItemType)
        {
            case AttractionItemType.None:
                break;
            case AttractionItemType.TapirSits:
                break;
            case AttractionItemType.TapirFlees:
                tapir.FleeDirection();
                if (GameCore.Instance.achievementUseEggPlant == false && isEggPlantItem)
                {
                    GameCore.Instance.SetAchievementUseEggPlant(true);
                }
                break;
            case AttractionItemType.TapirFollows:
                tapir.FollowTarget(this);
                if (GameCore.Instance.achievementUsePlush == false && isPlushItem)
                {
                    GameCore.Instance.SetAchievementUsePlush(true);
                }
                break;
            case AttractionItemType.TapirAttacks:
                break;
            case AttractionItemType.PlayMusic:
                GameCore.Instance.PlayNewMusic(attractionMusicToPlay, attractionMusicDuration);
                if (GameCore.Instance.achievementCatchThemAll == false && isCatchThemAllItem)
                {
                    GameCore.Instance.SetAchievementCatchThemAllValue(true);
                }
                break;
        }
    }

    public void OnIsAbsorbedItem(bool isOn, TapirController tapir)
    {
        if (isOn)
        {
            currentTapir = tapir;
            
            switch (absorptionEffectItemType)
            {
                case AbsorptionEffectItemType.None:
                    break;
                case AbsorptionEffectItemType.InstantSneeze:
                    tapir.OnEndAbsorption += SetInstantSneeze;
                    break;
                case AbsorptionEffectItemType.SuperSneeze:
                    break;
                case AbsorptionEffectItemType.ChangeTapirMaterial:
                    break;
                case AbsorptionEffectItemType.AddAdditionnalsFeedbacks:
                    if (hasAlreadyApplyTapirCollisionsBind == false)
                    {
                        tapir.OnEnterCollisionWithObject += PlayFeedbacksOnTapirCollision;
                        hasAlreadyApplyTapirCollisionsBind = true;
                    }
                    break;
            }
        }
        else
        {
            currentTapir = null;
            
            switch (absorptionEffectItemType)
            {
                case AbsorptionEffectItemType.None:
                    break;
                case AbsorptionEffectItemType.InstantSneeze:
                    tapir.OnEndAbsorption -= SetInstantSneeze;
                    break;
                case AbsorptionEffectItemType.SuperSneeze:
                    break;
                case AbsorptionEffectItemType.ChangeTapirMaterial:
                    break;
                case AbsorptionEffectItemType.AddAdditionnalsFeedbacks:
                    //tapir.OnEnterCollisionWithObject -= PlayFeedbacksOnTapirCollision;
                    break;
            }

            if (gameObject.activeSelf)
            {
                StartCoroutine(OnAbsorbedOffCoroutine());
            }
        }
    }

    private IEnumerator OnAbsorbedOffCoroutine()
    {
        yield return new WaitForSeconds(1.5f);

        rb.velocity = Vector3.zero;
    }

    public void PlayFeedbacksOnTapirCollision()
    {
        if (currentTapir == null)
        {
            return;
        }
        
        Debug.Log(gameObject.name + " Play Feedbacks On Tapir Collision");
        
        for (int i = 0; i < onTapirCollisionFeedbacks.Length; i++)
        {
            currentTapir.FeedbacksReader.ReadFeedback(onTapirCollisionFeedbacks[i]);
        }

        if (onTapirCollisionSfx.Length > 0)
        {
            AudioController.Instance.PlayRandomAudio(onTapirCollisionSfx);
        }
    }

    public void SetInstantSneeze(bool value)
    {
        if (!value)
        {
            if (currentTapir == null)
            {
                return;
            }
        
            currentTapir.Sneeze();
        }
    }
}
