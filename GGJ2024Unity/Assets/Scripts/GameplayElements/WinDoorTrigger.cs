using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinDoorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        TapirController tapir = other.GetComponent<TapirController>();
        if (tapir != null)
        {
            GameManager.Instance.tapirIsCaptured = true;
            GameCore.Instance.EndGame();
        }
    }
}
