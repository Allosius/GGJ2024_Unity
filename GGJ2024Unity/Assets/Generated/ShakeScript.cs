using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeScript : MonoBehaviour
{
    private bool isShaking = false;
    private int currentShakeLoopCount = 0;
    
    [SerializeField] float shakeSpeed = 10f;
    [SerializeField] private int shakeLoopCount = 10;
    [SerializeField] float shakeDuration = 0.5f;
    [SerializeField] bool shakeOnStart = false;

    [Tooltip("The amount of shake applied to the object")]
    [SerializeField] Vector3 shakeAmount = new Vector3(0.1f, 0.1f, 0.1f);

    void Start()
    {
        if (shakeOnStart)
        {
            Shake();
        }
    }

    public void Shake()
    {
        if (isShaking)
        {
            return;
        }
        
        currentShakeLoopCount = 0;

        isShaking = true;
        
        StartCoroutine(CoroutineShake());
    }

    IEnumerator CoroutineShake()
    {
        Vector3 originalPosition = transform.position;

        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float x = Mathf.Sin(elapsedTime / shakeDuration * Mathf.PI) * shakeAmount.x;
            float y = Mathf.Sin(elapsedTime / shakeDuration * Mathf.PI) * shakeAmount.y;
            float z = Mathf.Sin(elapsedTime / shakeDuration * Mathf.PI) * shakeAmount.z;

            transform.position = originalPosition + new Vector3(x, y, z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPosition;

        currentShakeLoopCount++;
        if (currentShakeLoopCount < shakeLoopCount)
        {
            StartCoroutine(CoroutineShake());
        }
        else
        {
            isShaking = false;
        }
    }
}