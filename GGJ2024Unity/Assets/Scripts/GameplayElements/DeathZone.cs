using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    
    private void OnTriggerEnter(Collider other)
    {
        TapirController tapir = other.GetComponent<TapirController>();
        if (tapir != null)
        {
            tapir.Rb.velocity = new Vector3(0, 0, 0);
            tapir.transform.position = respawnPoint.position;
        }
        
        PickableItem pickable = other.GetComponent<PickableItem>();
        if (pickable != null)
        {
            pickable.Rb.velocity = new Vector3(0, 0, 0);
            pickable.transform.position = respawnPoint.position;
        }
    }
}
