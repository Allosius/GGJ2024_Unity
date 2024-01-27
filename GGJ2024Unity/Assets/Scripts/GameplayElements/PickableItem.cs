using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    // Reference to the rigidbody
    private Rigidbody rb;

    private Animator animator;
    
    public Rigidbody Rb => rb;


    public Animator Animator => animator;
    
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
    }
}
