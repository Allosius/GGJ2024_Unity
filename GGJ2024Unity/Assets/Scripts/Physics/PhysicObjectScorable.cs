using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PhysicObject : MonoBehaviour
{
    public bool canGiveScore { get; set; } = true;
    
    public int scoreAmountMin = 10;
    public int scoreAmountMax = 150;

    public PhysicObj physicObj;

    private void OnTriggerEnter(Collider other)
    {
        PhysicTrigger physicTrigger = other.gameObject.GetComponent<PhysicTrigger>();
        if (physicTrigger && canGiveScore)
        {
            Debug.Log(transform.parent.name + " Give Score");
            
            canGiveScore = false;

            int scoreAmount = Random.Range(scoreAmountMin, scoreAmountMax);
            GameManager.Instance.AddScore(scoreAmount);
            GameCore.Instance.CreateScorePopUp(transform, scoreAmount);
            
            if (physicObj != null)
            {
                physicObj.hasTotalPhysic = true;
                physicObj.Rb.constraints = RigidbodyConstraints.None;
            }
        }
    }

    // private void OnTriggerExit(Collider other)
    // {
    //     PhysicTrigger physicTrigger = other.gameObject.GetComponent<PhysicTrigger>();
    //     if (physicTrigger && canGiveScore == false)
    //     {
    //         canGiveScore = true;
    //     }
    // }
}
