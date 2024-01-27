using System.Collections;
using System.Collections.Generic;
using AllosiusDevCore;
using UnityEngine;

public enum AttractionItemType
{
    TapirSits,
    TapirFlees,
    TapirFollows,
    TapirAttacks,
    PlayMusic,
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


    [SerializeField] private AttractionItemType attractionItemType;

    [SerializeField] private FeedbacksData[] attractionFeedbacks;

    [SerializeField] private int tapirFillGaugeAmount = 10;

    [SerializeField] private float absorbMoveSpeedItem = 10.0f;

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
            case AttractionItemType.TapirSits:
                break;
            case AttractionItemType.TapirFlees:
                break;
            case AttractionItemType.TapirFollows:
                break;
            case AttractionItemType.TapirAttacks:
                break;
            case AttractionItemType.PlayMusic:
                break;
        }
    }
}
