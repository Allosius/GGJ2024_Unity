using System;
using System.Collections;
using System.Collections.Generic;
using AllosiusDevCore;
using DG.Tweening;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

public enum TapirAIState
{
    Patrol,
    Sit,
    None,
}

public enum TapirMovementState
{
    Default,
    Flee,
    Charge,
}

[RequireComponent(typeof(FeedbacksReader))]
public class TapirController : MonoBehaviour
{
    private FeedbacksReader feedbacksReader;
    
    private Rigidbody rb;

    private Collider collider;

    private PatrolAI patrolAI;
    
    private PlayerInteraction currentPlayerInteraction;

    private PickableItem currentItemAbsorbing;
    private List<PickableItem> pickableItemsAbsorbed = new List<PickableItem>();

    private TapirAIState currentAiState = TapirAIState.Patrol;

    private TapirMovementState currentMovementState = TapirMovementState.Default;

    private Transform currentTargetObject;
    private Vector3 currentTargetPosition;
    
    private int fillGaugeProgress = 0;

    private bool isSneezing;

    public bool canAbsorbObject { get; set; } = true;
    
    [SerializeField] private bool drawGizmos = true;

    [SerializeField] private float threashold = 0.1f;
    
    [SerializeField] private float playerDetectionRange = 10.0f;

    [SerializeField] private LayerMask playerLayer;
    
    public LayerMask whatIsGround;
    
    [SerializeField] private Animator animator;

    [SerializeField] private Transform sneezePoint;
    
    [SerializeField] private float patrolTimer = 10.0f;
    
    [SerializeField] private float sittingTimer = 2.0f;

    [SerializeField] private float angerSpeed = 10.0f;

    [SerializeField] private FeedbacksData sneezeFeedbacks;

    [SerializeField] private float sneezeVerticalForce = 1.0f;
    [SerializeField] private float sneezeForceMagnitude = 50f;
    [SerializeField] private float sneezeEjectionAngle = 180f;
    
    [SerializeField] private float sneezeItemsEjectionAngleVariance = 45f;

    [SerializeField] private float sneezeFillGaugeAmount = 100f;

    private void Start()
    {
        feedbacksReader = GetComponent<FeedbacksReader>();
        
        rb = GetComponent<Rigidbody>();

        collider = GetComponent<Collider>();
        
        patrolAI = GetComponent<PatrolAI>();
        
        UpdateAiState();
        
        StartCoroutine(SittingTimerCoroutine());
    }

    private void FixedUpdate()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, playerDetectionRange, playerLayer);
        PlayerInteraction playerInteraction = null;
        foreach (Collider col in cols)
        {
            playerInteraction = col.GetComponent<PlayerInteraction>();
            if (playerInteraction && currentPlayerInteraction == null)
            {
                currentPlayerInteraction = playerInteraction;
                currentPlayerInteraction.SetTapirInRange(this);
            }
        }

        if (playerInteraction == null)
        {
            if (currentPlayerInteraction && canAbsorbObject)
            {
                currentPlayerInteraction.SetTapirInRange(null);
                currentPlayerInteraction = null;
            }
        }

        bool grounded = Physics.Raycast(transform.position, -transform.up, threashold, whatIsGround);

        if (isSneezing)
        {
            if (grounded && rb.velocity.x <= Mathf.Abs(-5f) && rb.velocity.y <= Mathf.Abs(-5f) && rb.velocity.z <= Mathf.Abs(-5f))
            {
                isSneezing = false;
                rb.isKinematic = true;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                transform.eulerAngles = Vector3.zero;
                
                currentAiState = TapirAIState.Patrol;
                UpdateAiState();
            }
        }
    }
    
    private void UpdateAiState()
    {
        switch (currentAiState)
        {
            case TapirAIState.Patrol :
                patrolAI.SetCanPatrol(true);
                break;
            case TapirAIState.Sit:
                patrolAI.SetCanPatrol(false);
                break;
            case TapirAIState.None:
                patrolAI.SetCanPatrol(false);
                break;
        }
    }

    public void AbsorbPickableItem(PickableItem item)
    {
        canAbsorbObject = false;
        
        currentAiState = TapirAIState.None;
        UpdateAiState();

        if (currentPlayerInteraction != null)
        {
            transform.LookAt(new Vector3(currentPlayerInteraction.transform.position.x, transform.position.y, currentPlayerInteraction.transform.position.z));
        }

        currentItemAbsorbing = item;
        item.transform.parent = null;
        item.transform.DOMove(transform.position, item.AbsorbMoveSpeedItem).SetSpeedBased().OnComplete(EndAbsorptionItem);
       
        
        if (pickableItemsAbsorbed.Contains(item) == false)
        {
            pickableItemsAbsorbed.Add(item);

            fillGaugeProgress += item.TapirFillGaugeAmount;
            fillGaugeProgress = Mathf.Clamp(fillGaugeProgress, 0, 100);
        }
    }

    public void EndAbsorptionItem()
    {
        if (currentItemAbsorbing == null)
        {
            return;
        }

        currentItemAbsorbing.transform.parent = this.transform;
        currentItemAbsorbing.gameObject.SetActive(false);

        currentItemAbsorbing = null;

        if (fillGaugeProgress >= sneezeFillGaugeAmount)
        {
            Sneeze();
        }
        else
        {
            canAbsorbObject = true;
            
            // currentAiState = TapirAIState.Patrol;
            // UpdateAiState();
        }
    }

    public void Sneeze()
    {
        if (currentPlayerInteraction == null)
        {
            return;
        }

        StartCoroutine(SneezeCoroutine());
    }

    private IEnumerator SneezeCoroutine()
    {
        Debug.Log("Sneeze");
        
        animator.SetTrigger("isSneezing");

        yield return new WaitForSeconds(0.4f);

        for (int i = 0; i < pickableItemsAbsorbed.Count; i++)
        {
            pickableItemsAbsorbed[i].gameObject.layer = 8;
            pickableItemsAbsorbed[i].transform.parent = null;
            pickableItemsAbsorbed[i].transform.position = sneezePoint.position;
            pickableItemsAbsorbed[i].Rb.isKinematic = false;
            
            pickableItemsAbsorbed[i].gameObject.SetActive(true);
            
            Vector3 itemDir = currentPlayerInteraction.transform.position - pickableItemsAbsorbed[i].transform.position;
            float itemAngle = Mathf.Rad2Deg * Mathf.Atan2(itemDir.y, itemDir.x);
            itemDir.Normalize();
            itemDir = Quaternion.Euler(0f, 0f, -itemAngle + UnityEngine.Random.Range(0, sneezeItemsEjectionAngleVariance)) * itemDir;
            rb.velocity = (new Vector3(itemDir.x, sneezeVerticalForce, itemDir.z) * sneezeForceMagnitude) * fillGaugeProgress;

           //pickableItemsAbsorbed[i].isSneezing = true;
        }
        
        pickableItemsAbsorbed.Clear();

        feedbacksReader.ReadFeedback(sneezeFeedbacks);
        
        rb.isKinematic = false;
        //rb.constraints = RigidbodyConstraints.None;

        Vector3 direction = currentPlayerInteraction.transform.position - transform.position;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
        direction.Normalize();
        direction = Quaternion.Euler(0f, 0f, -angle + sneezeEjectionAngle) * direction;
        rb.velocity = (new Vector3(-direction.x, sneezeVerticalForce, -direction.z) * sneezeForceMagnitude) * fillGaugeProgress;

        fillGaugeProgress = 0;
        
        canAbsorbObject = true;

        isSneezing = true;
    }

    public void SpitOutPickableItem(PickableItem item)
    {
        if (pickableItemsAbsorbed.Contains(item))
        {
            pickableItemsAbsorbed.Remove(item);
        }
    }
    
    IEnumerator SittingTimerCoroutine()
    {
        //Debug.Log("Sitting Timer Coroutine");
        switch (currentAiState)
        {
            case TapirAIState.Patrol :
                yield return new WaitForSeconds(patrolTimer);
                break;
            case TapirAIState.Sit:
                yield return new WaitForSeconds(sittingTimer);
                break;
            case TapirAIState.None:
                yield return new WaitForSeconds(1f);
                break;
        }
        
        switch (currentAiState)
        {
            case TapirAIState.Patrol :
                Debug.Log("Set Sit");
                currentAiState = TapirAIState.Sit;
                break;
            case TapirAIState.Sit:
                Debug.Log("Set Patrol");
                currentAiState = TapirAIState.Patrol;
                break;
            case TapirAIState.None:
                break;
        }
        UpdateAiState();
        StartCoroutine(SittingTimerCoroutine());
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos == false)
        {
            return;
        }

        Gizmos.color = Color.red;
        
        Gizmos.DrawWireSphere(transform.position, playerDetectionRange);
    }
}
