using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicObject : MonoBehaviour
{
    public bool canGiveScore { get; set; } = true;
    
    public int scoreAmount = 100;

    private void OnTriggerEnter(Collider other)
    {
        PhysicTrigger physicTrigger = other.gameObject.GetComponent<PhysicTrigger>();
        if (physicTrigger && canGiveScore)
        {
            canGiveScore = false;
            GameManager.Instance.currentScore += scoreAmount;
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
