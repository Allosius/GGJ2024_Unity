using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerTriggerZone : MonoBehaviour
{
    public Transform[] fleesPoints;


    public Transform GetClosestFleePointTarget(Transform target)
    {
        float closestDistance = float.MaxValue;
        Transform closestPoint = fleesPoints[0];
        
        for (int i = 0; i < fleesPoints.Length; i++)
        {
            float dist = Vector3.Distance(fleesPoints[i].position, target.position);
            if (dist <= closestDistance)
            {
                closestDistance = dist;
                closestPoint = fleesPoints[i];
            }
        }

        return closestPoint;
    }

    private void OnTriggerStay(Collider other)
    {
        TapirController tapir = other.GetComponent<TapirController>();
        if (tapir != null && tapir.currentMovementState != TapirMovementState.Flee && tapir.IsSneezing == false)
        {
            Transform closestFleePoint = GetClosestFleePointTarget(tapir.transform);
            tapir.FleeTarget(closestFleePoint, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        TapirController tapir = other.GetComponent<TapirController>();
        if (tapir != null)
        {
            
        }
    }
}
