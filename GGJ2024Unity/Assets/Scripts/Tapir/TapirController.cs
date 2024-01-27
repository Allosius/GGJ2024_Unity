using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapirController : MonoBehaviour
{
    private PlayerInteraction currentPlayerInteraction;
    
    [SerializeField] private bool drawGizmos = true;
    
    [SerializeField] private float playerDetectionRange = 10.0f;

    [SerializeField] private LayerMask playerLayer;

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
            if (currentPlayerInteraction)
            {
                currentPlayerInteraction.SetTapirInRange(null);
                currentPlayerInteraction = null;
            }
        }
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
