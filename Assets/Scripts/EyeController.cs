using UnityEngine;

public class EyeController : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float followSpeed = 3f; // Speed of following movement
    public float floatAmplitude = 0.2f; // Floating height
    public float floatFrequency = 2f; // Floating speed
    public float transitionSpeed = 5f; // Speed of transition when flipping
    public float horizontalOffset = 0.5f; // Distance from the player

    private Vector3 m_DesiredLocalOffset; // Target local offset
    private Vector3 m_CurrentLocalOffset; // Current smoothly transitioning offset
    private float m_StartY; // Initial Y position for floating
    private int m_LastPlayerDirection = 1; // 1 (right), -1 (left)

    void Start()
    {
        if (player == null)
        {
            player = transform.parent; // Auto-assign if the EyeCompanion is a child of the player
        }

        // Set the sprite behind the player
        SpriteRenderer eyeSprite = GetComponent<SpriteRenderer>();
        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();

        if (eyeSprite != null && playerSprite != null)
        {
            eyeSprite.sortingOrder = playerSprite.sortingOrder - 1; // Ensures the eye is behind the player
        }

        m_DesiredLocalOffset = new Vector3(horizontalOffset, 0, 0);
        m_CurrentLocalOffset = m_DesiredLocalOffset;
        m_StartY = transform.localPosition.y;
    }


    void Update()
    {
        UpdatePositionBasedOnFlip();
        FollowPlayer();
        ApplyFloatingMotion();
    }

    private void UpdatePositionBasedOnFlip()
    {
        int currentDirection = player.localScale.x > 0 ? 1 : -1;

        if (currentDirection != m_LastPlayerDirection)
        {
            m_LastPlayerDirection = currentDirection;
            // Adjust offset based on new direction (invert X position)
            m_DesiredLocalOffset.x = horizontalOffset * currentDirection;
        }

        // Smooth transition to the new offset instead of snapping
        m_CurrentLocalOffset.x = Mathf.Lerp(m_CurrentLocalOffset.x, m_DesiredLocalOffset.x, Time.deltaTime * transitionSpeed);
    }

    private void FollowPlayer()
    {
        // Ensure the eye follows the player's position
        Vector3 targetPosition = player.position + m_CurrentLocalOffset;
        targetPosition.z = player.position.z - 0.1f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private void ApplyFloatingMotion()
    {
        // Floating motion
        float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        float horizontalSway = Mathf.Cos(Time.time * (floatFrequency / 2)) * (floatAmplitude * 0.3f); // Subtle left-right sway

        // Apply both the smooth transition and floating motion
        transform.position = new Vector3(player.position.x + m_CurrentLocalOffset.x + horizontalSway, player.position.y + m_StartY + floatOffset, transform.position.z);
    }
}
