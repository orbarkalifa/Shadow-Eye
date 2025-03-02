using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform Target;

    [Header("Offset Settings")]
    [SerializeField] private Vector3 Offset = new Vector3(0, 1.5f, -10);

    [Header("Follow Smoothness")]
    [Range(0f, 1f)] [SerializeField] private float SmoothSpeed = 0.125f;

    private void LateUpdate()
    {
        if (!Target)
        {
            return;
        }

        // Smoothly move camera towards the desired position
        Vector3 desiredPosition = Target.position + Offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed);
    }
}