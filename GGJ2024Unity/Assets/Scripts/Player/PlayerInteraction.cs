using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
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
