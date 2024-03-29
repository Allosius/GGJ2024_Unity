using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCharacterGrabObjectsController : MonoBehaviour
{
    private PlayerInteraction playerInteraction;
    // Reference to the character camera.
    [SerializeField]
    private Camera characterCamera;
    // Reference to the slot for holding picked item.
    [SerializeField]
    private Transform slot;
    // Reference to the currently held item.
    public PickableItem pickedItem { get; protected set; }


    public float grabRayLength = 0.5f;
    public float maxGrabDistance = 5f;

    public LayerMask grabLayer;


    private void Start()
    {
        playerInteraction = GetComponent<PlayerInteraction>();
    }

    /// <summary>
    /// Method called very frame.
    /// </summary>
    private void Update()
    {
        if(!pickedItem)
        {
            // If no, try to pick item in front of the player
            // Create ray from center of the screen
            var ray = characterCamera.ViewportPointToRay(Vector3.one * grabRayLength);
            RaycastHit hit;
            // Shot ray to find object to pick
            if (Physics.Raycast(ray, out hit, maxGrabDistance, grabLayer))
            {
                // Check if object is pickable
                var pickable = hit.transform.GetComponent<PickableItem>();
                // If object has PickableItem class
                if (pickable)
                {
                    // Can Pick it
                    GameCanvasManager.Instance.UpdateCursorState(true);
                }
                else
                {
                    GameCanvasManager.Instance.UpdateCursorState(false);
                }
            }
            else
            {
                GameCanvasManager.Instance.UpdateCursorState(false);
            }
        }
        else
        {
            if (playerInteraction.GetCurrentTapirInRange() == false)
            {
                GameCanvasManager.Instance.UpdateCursorState(false);
            }
            else
            {
                if (playerInteraction.CurrentTapirInRange && pickedItem && playerInteraction.CurrentTapirInRange.canAbsorbObject)
                {
                    GameCanvasManager.Instance.UpdateCursorState(true);
                }
                else
                {
                    GameCanvasManager.Instance.UpdateCursorState(false);
                }
            }
        }
        
        // Execute logic only on button pressed
        if (Input.GetMouseButtonDown(0))
        {
            if(!pickedItem)
            {
                // If no, try to pick item in front of the player
                // Create ray from center of the screen
                var ray = characterCamera.ViewportPointToRay(Vector3.one * grabRayLength);
                RaycastHit hit;
                // Shot ray to find object to pick
                if (Physics.Raycast(ray, out hit, maxGrabDistance, grabLayer))
                {
                    // Check if object is pickable
                    var pickable = hit.transform.GetComponent<PickableItem>();
                    // If object has PickableItem class
                    if (pickable)
                    {
                        // Pick it
                        PickItem(pickable);
                    }
                }
            }
            else
            {
                if (playerInteraction.GetCurrentTapirInRange() == false)
                {
                    if (pickedItem && pickedItem.Animator)
                    {
                        pickedItem.Animator.enabled = true;
                        pickedItem.Animator.SetTrigger("Shake");
                        pickedItem.OnAttractionItem(GameCore.Instance.tapir);
                    }
                }
                else
                {
                    if (playerInteraction.CurrentTapirInRange && pickedItem && playerInteraction.CurrentTapirInRange.canAbsorbObject)
                    {
                        playerInteraction.CurrentTapirInRange.AbsorbPickableItem(pickedItem);
                        pickedItem = null;
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            // Check if player picked some item already
            if (pickedItem)
            {
                // If yes, drop picked item
                DropItem(pickedItem);
            }
        }
    }
    /// <summary>
    /// Method for picking up item.
    /// </summary>
    /// <param name="item">Item.</param>
    private void PickItem(PickableItem item)
    {
        // Assign reference
        pickedItem = item;
        // Disable rigidbody and reset velocities
        item.Rb.isKinematic = true;
        item.Rb.velocity = Vector3.zero;
        item.Rb.angularVelocity = Vector3.zero;
        // Set Slot as a parent
        item.transform.SetParent(slot);
        // Reset position and rotation
        item.transform.localPosition = Vector3.zero;
        item.transform.localEulerAngles = Vector3.zero;
    }
    /// <summary>
    /// Method for dropping item.
    /// </summary>
    /// <param name="item">Item.</param>
    private void DropItem(PickableItem item)
    {
        // Remove reference
        pickedItem = null;
        // Remove parent
        item.transform.SetParent(null);
        // Enable rigidbody
        item.Rb.isKinematic = false;
        // Add force to throw item a little bit
        item.Rb.AddForce(item.transform.forward * 2, ForceMode.VelocityChange);
    }
}