using System;
using System.Collections;
using System.Collections.Generic;
using AllosiusDevCore;
using DG.Tweening;
using Sirenix.Serialization;
using Unity.VisualScripting;
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
    FollowTarget,
    FleeDirection,
}

[RequireComponent(typeof(FeedbacksReader))]
public class TapirController : MonoBehaviour
{
    private FeedbacksReader feedbacksReader;
    
    private Rigidbody rb;

    private Collider collider;

    private PatrolAI patrolAI;

    private PickableItem currentFollowTarget;
    
    private Transform currentFleeTarget;
    
    private Vector3 currentFleeDirection;
    
    private PlayerInteraction currentPlayerInteraction;

    private PickableItem currentItemAbsorbing;
    private List<PickableItem> pickableItemsAbsorbed = new List<PickableItem>();

    public TapirAIState currentAiState = TapirAIState.Patrol;

    public TapirMovementState currentMovementState = TapirMovementState.Default;

    private Transform currentTargetObject;
    private Vector3 currentTargetPosition;
    
    private int fillGaugeProgress = 0;

    private bool isSneezing;

    public FeedbacksReader FeedbacksReader => feedbacksReader;

    public Rigidbody Rb => rb;

    public Animator Animator => animator;

    public bool canAbsorbObject { get; set; } = true;

    public bool IsSneezing => isSneezing;

    public GameObject graphics;
    
    [SerializeField] private bool drawGizmos = true;

    [SerializeField] private float threashold = 0.1f;
    
    [SerializeField] private float playerDetectionRange = 10.0f;

    [SerializeField] private LayerMask playerLayer;
    
    public LayerMask whatIsGround;
    
    [SerializeField] private Animator animator;

    [SerializeField] private Transform sneezePoint;
    
    [SerializeField] private float patrolTimer = 10.0f;

    [SerializeField] private float sittingTimer = 2.0f;

    [SerializeField] private float fleeSpeed = 4.0f;
    
    [SerializeField] private float fleeDirectionTimer = 3.0f;
    
    [SerializeField] private float followTargetSpeed = 3.0f;
    [SerializeField] private float followTargetTimer = 3.0f;

    [SerializeField] private FeedbacksData sneezeFeedbacks;

    [SerializeField] private float sneezeVerticalForce = 1.0f;
    [SerializeField] private float sneezeForceMagnitude = 50f;
    [SerializeField] private float sneezeEjectionAngle = 180f;
    
    [SerializeField] private float sneezeItemsEjectionAngleVariance = 45f;

    [SerializeField] private int sneezeFillGaugeAmount = 100;
    
    [SerializeField] private int sneezeAbsoluteMaxFillGaugeAmount = 300;

    [Space] 
    
    public Transform stateFeedbackPoint;
    
    [SerializeField] private PopUpText stateAttractedFeedbackPopUp;
    [SerializeField] private PopUpText stateScaredFeedbackPopUp;

    
    public event Action OnEnterCollisionWithObject;
    public event Action<bool> OnEndAbsorption;
    
    private void Start()
    {
        feedbacksReader = GetComponent<FeedbacksReader>();
        
        rb = GetComponent<Rigidbody>();

        collider = GetComponent<Collider>();
        
        patrolAI = GetComponent<PatrolAI>();
        
        UpdateAiState();
        
        StartCoroutine(SittingTimerCoroutine());
    }

    private void Update()
    {
        switch (currentMovementState)
        {
            case TapirMovementState.Default :
                
                break;
            case TapirMovementState.Charge:
                
                break;
            case TapirMovementState.Flee:
                if (currentFleeTarget != null)
                {
                    float targetDist = Vector3.Distance(transform.position, currentFleeTarget.position);
                    if (targetDist < 0.5f)
                    {
                        FleeTarget(null, false);
                    }
                    else
                    {
                        patrolAI.agent.enabled = true;
                        patrolAI.agent.SetDestination(currentFleeTarget.position);
                    }
                }
                break;
            case TapirMovementState.FollowTarget:
                if (currentFollowTarget != null)
                {
                    patrolAI.agent.enabled = true;
                    if (GameCore.Instance.player.GetComponent<FirstPersonCharacterGrabObjectsController>().pickedItem ==
                        currentFollowTarget)
                    {
                        patrolAI.agent.SetDestination(GameCore.Instance.player.posPoint.transform.position);
                    }
                    else
                    {
                        patrolAI.agent.SetDestination(currentFollowTarget.transform.position);
                    }
                    
                }
                break;
            case TapirMovementState.FleeDirection:
                rb.isKinematic = false;
                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
                if (GameCore.Instance.player != null)
                {
                    Vector3 dir = transform.position - GameCore.Instance.player.transform.position;
                    dir.Normalize();
                    currentFleeDirection = dir;
                    
                    Vector3 velocity = new Vector3(currentFleeDirection.x * fleeSpeed, 
                        0.0f, currentFleeDirection.z * fleeSpeed);
                    rb.velocity = velocity;
                    // Orientation de l'objet vers la direction du mouvement
                    if (velocity != Vector3.zero)
                    {
                        //transform.LookAt(transform.position - velocity);
                        transform.forward = rb.velocity.normalized;
                    }
                }
                break;
        }

        if (currentMovementState == TapirMovementState.FollowTarget || currentMovementState == TapirMovementState.Flee
            || currentMovementState == TapirMovementState.FleeDirection || currentMovementState == TapirMovementState.Charge
            || currentAiState == TapirAIState.Patrol)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
        
        if (currentMovementState == TapirMovementState.Flee || currentMovementState == TapirMovementState.FleeDirection 
            || currentMovementState == TapirMovementState.Charge)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (currentAiState == TapirAIState.Sit)
        {
            animator.SetBool("isSit", true);
        }
        else
        {
            animator.SetBool("isSit", false);
        }
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
    
    public void CreateScaredFeedbackPopUp(Transform target, string label)
    {
        PopUpText textToInstantiate = stateScaredFeedbackPopUp;

        CreateStateFeedbackPopUp(target, label, textToInstantiate);
    }
    
    public void CreateAttractedFeedbackPopUp(Transform target, string label)
    {
        PopUpText textToInstantiate = stateAttractedFeedbackPopUp;

        CreateStateFeedbackPopUp(target, label, textToInstantiate);
    }

    public void CreateStateFeedbackPopUp(Transform target, string label, PopUpText popUpPrefab)
    {
        var myNewScore = Instantiate(popUpPrefab);
        //Vector2 screenPosition = Camera.main.WorldToScreenPoint(target.position);

        myNewScore.transform.SetParent(transform, true);
        myNewScore.transform.position = target.position;
        myNewScore.GetComponent<PopUpText>().SetText(label);
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

    public void FollowTarget(PickableItem target)
    {
        if (isSneezing)
        {
            return;;
        }
        
        StopCoroutine(FleeDirectionCoroutine());
        
        currentFollowTarget = target;
        
        currentAiState = TapirAIState.None;
        UpdateAiState();
        
        CreateAttractedFeedbackPopUp(stateFeedbackPoint, "?");
        
        animator.SetTrigger("TiltHead");
        
        StartCoroutine(FollowTargetCoroutine(target));
    }

    private IEnumerator FollowTargetCoroutine(PickableItem transform)
    {
        yield return new WaitForSeconds(0.2f);
        
        rb.velocity = Vector3.zero;
        currentMovementState = TapirMovementState.FollowTarget;

        patrolAI.agent.enabled = true;
        patrolAI.agent.speed = followTargetSpeed;
        
        yield return new WaitForSeconds(followTargetTimer);

        if (currentMovementState == TapirMovementState.FollowTarget)
        {
            currentFollowTarget = null;
        
            rb.velocity = Vector3.zero;
            currentMovementState = TapirMovementState.Default;
        
            currentAiState = TapirAIState.Patrol;
            UpdateAiState();
        }
    }
    
    public void FleeDirection()
    {
        if (isSneezing)
        {
            return;;
        }
        
        StopCoroutine(FollowTargetCoroutine(currentFollowTarget));
        
        currentFollowTarget = null;

        currentAiState = TapirAIState.None;
        UpdateAiState();
        
        rb.velocity = Vector3.zero;
        currentMovementState = TapirMovementState.FleeDirection;
        
        CreateScaredFeedbackPopUp(stateFeedbackPoint, "!");

        patrolAI.agent.enabled = false;
        patrolAI.agent.speed = fleeSpeed;

        StartCoroutine(FleeDirectionCoroutine());
    }
    
    private IEnumerator FleeDirectionCoroutine()
    {
        yield return new WaitForSeconds(fleeDirectionTimer);

        if (currentMovementState == TapirMovementState.FleeDirection)
        {
            currentFleeDirection = transform.position;
        
            rb.velocity = Vector3.zero;
            currentMovementState = TapirMovementState.Default;
        
            currentAiState = TapirAIState.Patrol;
            UpdateAiState();
        }
    }

    public void FleeTarget(Transform target, bool activeFlee)
    {
        if (isSneezing)
        {
            return;
        }

        if (activeFlee)
        {
            StopCoroutine(FollowTargetCoroutine(currentFollowTarget));
            StopCoroutine(FleeDirectionCoroutine());
        
            currentFollowTarget = null;
        
            currentAiState = TapirAIState.None;
            UpdateAiState();

            currentFleeTarget = target;
        
            rb.velocity = Vector3.zero;
            currentMovementState = TapirMovementState.Flee;
            
            CreateScaredFeedbackPopUp(stateFeedbackPoint, "!");

            patrolAI.agent.enabled = true;
            patrolAI.agent.speed = fleeSpeed;
        }
        else
        {
            if (currentMovementState == TapirMovementState.Flee)
            {
                currentFleeTarget = null;
                
                rb.velocity = Vector3.zero;
                currentMovementState = TapirMovementState.Default;
        
                currentAiState = TapirAIState.Patrol;
                UpdateAiState();
            }
        }

    }

    public void AbsorbPickableItem(PickableItem item)
    {
        animator.SetTrigger("Chew");
        
        canAbsorbObject = false;

        StopCoroutine(FollowTargetCoroutine(currentFollowTarget));
        
        currentFollowTarget = null;
        
        rb.velocity = Vector3.zero;
        currentMovementState = TapirMovementState.Default;
        
        currentAiState = TapirAIState.None;
        UpdateAiState();

        patrolAI.agent.enabled = false;

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
            item.OnIsAbsorbedItem(true, this);

            fillGaugeProgress += item.TapirFillGaugeAmount;
            fillGaugeProgress = Mathf.Clamp(fillGaugeProgress, 0, sneezeAbsoluteMaxFillGaugeAmount);
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
            OnEndAbsorption?.Invoke(true);
        }
        else
        {
            canAbsorbObject = true;
            OnEndAbsorption?.Invoke(false);
            
            currentMovementState = TapirMovementState.Default;
        
            currentAiState = TapirAIState.Patrol;
            UpdateAiState();
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

        if (GameCore.Instance.achievementSneeze == false)
        {
            GameCore.Instance.SetAchievementSneezeValue(true);
        }
        
        animator.SetTrigger("isSneezing");
        
        OnEnterCollisionWithObject?.Invoke();

        yield return new WaitForSeconds(0.4f);

        for (int i = 0; i < pickableItemsAbsorbed.Count; i++)
        {
            pickableItemsAbsorbed[i].OnIsAbsorbedItem(false, this);
            
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
        Debug.Log(direction);
        float dirXMultiplier = 1.0f;
        if (direction.x < 0)
        {
            dirXMultiplier = -1.0f;
        }
        float angle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
        Debug.Log(angle);
        direction.Normalize();
        direction = Quaternion.Euler(0f, 0f, angle + sneezeEjectionAngle) * direction;
        Debug.Log(direction);
        direction.Normalize();
        rb.velocity = (new Vector3(dirXMultiplier * direction.x * sneezeForceMagnitude, sneezeVerticalForce, -direction.z * sneezeForceMagnitude)) * fillGaugeProgress;

        fillGaugeProgress = 0;
        
        canAbsorbObject = true;

        isSneezing = true;
    }

    // public void SpitOutPickableItem(PickableItem item)
    // {
    //     if (pickableItemsAbsorbed.Contains(item))
    //     {
    //         pickableItemsAbsorbed.Remove(item);
    //     }
    // }
    
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
                animator.SetTrigger("TiltHead");
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

    private void OnCollisionEnter(Collision collision)
    {
        OnEnterCollisionWithObject?.Invoke();
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
