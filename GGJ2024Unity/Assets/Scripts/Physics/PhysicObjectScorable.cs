using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicObject : MonoBehaviour
{
    public bool canGiveScore { get; set; } = true;
    
    public int scoreAmount = 100;

    public PhysicObj physicObj;

    private void OnTriggerEnter(Collider other)
    {
        PhysicTrigger physicTrigger = other.gameObject.GetComponent<PhysicTrigger>();
        if (physicTrigger && canGiveScore)
        {
            canGiveScore = false;
            GameManager.Instance.AddScore(scoreAmount);
            physicObj.hasTotalPhysic = true;
            physicObj.Rb.constraints = RigidbodyConstraints.None;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PhysicTrigger physicTrigger = other.gameObject.GetComponent<PhysicTrigger>();
        if (physicTrigger && canGiveScore == false)
        {
            canGiveScore = true;
        }
    }
}
