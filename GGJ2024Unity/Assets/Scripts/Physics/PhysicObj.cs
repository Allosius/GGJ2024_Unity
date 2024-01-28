using System;
using System.Collections;
using System.Collections.Generic;
using AllosiusDevCore;
using UnityEngine;

[RequireComponent(typeof(FeedbacksReader))]
public class PhysicObj : MonoBehaviour
{
    private FeedbacksReader feedbacksReader;
    
    public bool hasTotalPhysic { get; set; } = false;

    public FeedbacksReader FeedbacksReader => feedbacksReader;
    
    public Rigidbody Rb;

    public FeedbacksData WithCharacterCollisionFeedbacks;

    private void Start()
    {
        feedbacksReader = GetComponent<FeedbacksReader>();
        
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
        PlayerInteraction player = collision.gameObject.GetComponent<PlayerInteraction>();
        TapirController tapir = collision.gameObject.GetComponent<TapirController>();

        if (player || tapir)
        {
            if (WithCharacterCollisionFeedbacks != null)
            {
                feedbacksReader.ReadFeedback(WithCharacterCollisionFeedbacks);
            }
        }
        
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
