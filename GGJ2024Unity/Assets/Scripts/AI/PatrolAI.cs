using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PatrolAI : MonoBehaviour
{
    private bool walkPointSet;
    private bool canPatrol;

    [SerializeField] private float walkPointRange = 2.2f;

    [SerializeField] private float walkDirectionTimer = 5.0f;

    public Vector3 walkPoint { get; set; }
    

    public LayerMask whatIsGround;

    public NavMeshAgent agent;

    private void Start()
    {
        agent.enabled = true;
        StartCoroutine(RelaunchSearchWalkPoint());
    }

    private void Update()
    {
        Patroling();
    }

    public void SetCanPatrol(bool value)
    {
        canPatrol = value;
        if (canPatrol)
        {
            agent.enabled = true;
        }
        else
        {
            if (agent.enabled)
            {
                agent.SetDestination(transform.position);
                agent.enabled = false;
            }
        }
    }
    
    public void SearchWalkPoint()
    {
        if (canPatrol == false)
        {
            return;
        }
        
        Debug.Log("SearchWalkPoint");
        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX * 2, transform.position.y, transform.position.z + randomZ);

        
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    void Patroling()
    {
        if (canPatrol == false)
        {
            return;
        }
        
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // WalkPoint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    IEnumerator RelaunchSearchWalkPoint()
    {
        Debug.Log("RelaunchSearchWalkPoint");
        yield return new WaitForSeconds(walkDirectionTimer);
        SearchWalkPoint();
        StartCoroutine(RelaunchSearchWalkPoint());
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
    }
}