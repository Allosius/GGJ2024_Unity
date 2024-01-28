using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicObj : MonoBehaviour
{
    public bool hasTotalPhysic { get; set; } = false;
    
    public Rigidbody Rb;

    private void Start()
    {
        if (hasTotalPhysic)
        {
            Rb.constraints = RigidbodyConstraints.None;
        }
        else
        {
            //Rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            Rb.constraints = RigidbodyConstraints.None;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        TapirController tapir = collision.gameObject.GetComponent<TapirController>();

        if (tapir != null && hasTotalPhysic == false)
        {
            Debug.Log("On Collision Enter Tapir");
            Rb.constraints = RigidbodyConstraints.None;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        TapirController tapir = other.gameObject.GetComponent<TapirController>();

        if (tapir != null && hasTotalPhysic == false)
        {
            Debug.Log("On Collision Exit Tapir");
            //Rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        }
    }
}
