using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PatrolAI : MonoBehaviour
{
    private bool walkPointSet;

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

    public void SearchWalkPoint()
    {
        Debug.Log("SearchWalkPoint");
        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX * 2, transform.position.y, transform.position.z + randomZ);

        walkPointSet = true;

        //if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        //{
            
        //}
    }

    void Patroling()
    {
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