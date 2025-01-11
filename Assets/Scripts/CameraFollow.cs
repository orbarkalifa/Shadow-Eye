using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform m_Target;

    [Header("Offset Settings")]
    [SerializeField] private Vector3 m_Offset = new Vector3(0, 1.5f, -10);

    [Header("Follow Smoothness")]
    [Range(0f, 1f)] [SerializeField] private float m_SmoothSpeed = 0.125f;

    private void LateUpdate()
    {
        if (!m_Target)
        {
            return;
        }

        // Smoothly move camera towards the desired position
        Vector3 desiredPosition = m_Target.position + m_Offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, m_SmoothSpeed);
    }

    // Public method to assign target dynamically
    public void SetTarget(Transform i_Target)
    {
        m_Target = i_Target;
    }
}