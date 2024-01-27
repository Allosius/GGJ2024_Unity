using System;
using UnityEngine;
using UnityEngine.Events;

public class Ejector : MonoBehaviour
{
    [SerializeField] private Rigidbody targetRigidbody;
    [SerializeField] private float forceMagnitude = 10f;
    [SerializeField] private float ejectionAngle = 180f;
    [SerializeField] private bool useTargetPosition = false;
    [SerializeField] private Transform targetTransform;

    [Tooltip("The force applied to the ejected object")]
    public float ForceMagnitude
    {
        get { return forceMagnitude; }
        set { forceMagnitude = value; }
    }

    [Tooltip("The angle between the ejected object's initial velocity and the target object's position")]
    public float EjectionAngle
    {
        get { return ejectionAngle; }
        set { ejectionAngle = value; }
    }

    [Tooltip("Whether to use the target object's position as the ejected object's initial position")]
    public bool UseTargetPosition
    {
        get { return useTargetPosition; }
        set { useTargetPosition = value; }
    }

    [Tooltip("The transform of the target object")]
    public Transform TargetTransform
    {
        get { return targetTransform; }
        set { targetTransform = value; }
    }

    private void Start()
    {
        if (targetRigidbody == null)
        {
            targetRigidbody = GetComponent<Rigidbody>();
        }

        if (useTargetPosition)
        {
            transform.position = targetTransform.position;
        }

        Vector3 direction = targetTransform.position - transform.position;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
        direction.Normalize();
        direction = Quaternion.Euler(0f, 0f, -angle + ejectionAngle) * direction;
        targetRigidbody.velocity = direction * forceMagnitude;
    }
}