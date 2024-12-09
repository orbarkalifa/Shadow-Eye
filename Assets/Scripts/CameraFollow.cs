using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // The object to follow

    [Header("Offset Settings")]
    public Vector3 offset = new Vector3(0, 5, -10); // Position offset from the target

    [Header("Follow Smoothness")]
    [Range(0, 1)]
    public float smoothSpeed = 0.125f; // Smaller values make the movement smoother/slower

    private void LateUpdate()
    {
        if (!target)
        {
            Debug.LogWarning("no target assigned for camera follow");
            return;
        }
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}

