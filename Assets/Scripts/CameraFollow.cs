using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;

    [Header("Offset Settings")]
    [SerializeField] private Vector3 offset = new(0, 1.5f, -11);

    [Header("Follow Smoothness")]
    [Range(0f, 1f)] [SerializeField] private float smoothSpeed = 0.125f;

    private BeaconSO beacon;

    private void Awake()
    {
        if(!target)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void LateUpdate()
    {
        if (!target) return;
        
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}