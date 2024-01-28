using System.Collections;
using System.Collections.Generic;
using AllosiusDevCore;
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


    [SerializeField] private AttractionItemType attractionItemType;
    
    [SerializeField] private AbsorptionEffectItemType absorptionEffectItemType;

    [SerializeField] private FeedbacksData[] attractionFeedbacks;

    [SerializeField] private int tapirFillGaugeAmount = 10;

    [SerializeField] private float absorbMoveSpeedItem = 10.0f;
    
    [SerializeField] private FeedbacksData[] onTapirCollisionFeedbacks;

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
       
        switch (attractionItemType)
        {
            case AttractionItemType.None:
                break;
            case AttractionItemType.TapirSits:
                break;
            case AttractionItemType.TapirFlees:
                break;
            case AttractionItemType.TapirFollows:
                tapir.FollowTarget(this);
                break;
            case AttractionItemType.TapirAttacks:
                break;
            case AttractionItemType.PlayMusic:
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
                    tapir.OnEnterCollisionWithObject += PlayFeedbacksOnTapirCollision;
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
                    tapir.OnEnterCollisionWithObject -= PlayFeedbacksOnTapirCollision;
                    break;
            }
        }
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
