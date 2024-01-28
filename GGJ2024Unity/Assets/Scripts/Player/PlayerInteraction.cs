using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Transform posPoint;
    
    private TapirController currentTapirInRange;

    public TapirController CurrentTapirInRange => currentTapirInRange;

    public TapirController GetCurrentTapirInRange()
    {
        return currentTapirInRange;
    }

    public void SetTapirInRange(TapirController tapir)
    {
        currentTapirInRange = tapir;
    }
}
