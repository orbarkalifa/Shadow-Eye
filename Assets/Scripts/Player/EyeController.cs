using UnityEngine;

public class EyeController : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 3f;
    public float floatAmplitude = 0.2f;
    public float floatFrequency = 2f;
    public float transitionSpeed = 5f; 
    public float horizontalOffset = 0.5f;

    private Vector3 desiredLocalOffset; 
    private Vector3 currentLocalOffset; 
    private float startY; 
    private int lastPlayerDirection = 1;

    void Start()
    {
        if (player == null)
        {
            player = transform.parent;
        }

        SpriteRenderer eyeSprite = GetComponent<SpriteRenderer>();
        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();

        if (eyeSprite != null && playerSprite != null)
        {
            eyeSprite.sortingOrder = playerSprite.sortingOrder - 1; // Ensures the eye is behind the player
        }

        desiredLocalOffset = new Vector3(horizontalOffset, 0, 0);
        currentLocalOffset = desiredLocalOffset;
        startY = transform.localPosition.y;
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

        if (currentDirection != lastPlayerDirection)
        {
            lastPlayerDirection = currentDirection;
            desiredLocalOffset.x = horizontalOffset * currentDirection;
        }

        currentLocalOffset.x = Mathf.Lerp(currentLocalOffset.x, desiredLocalOffset.x, Time.deltaTime * transitionSpeed);
    }

    private void FollowPlayer()
    {
        Vector3 targetPosition = player.position + currentLocalOffset;
        targetPosition.z = player.position.z - 0.1f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private void ApplyFloatingMotion()
    {
        float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        float horizontalSway = Mathf.Cos(Time.time * (floatFrequency / 2)) * (floatAmplitude * 0.3f); // Subtle left-right sway

        transform.position = new Vector3(player.position.x + currentLocalOffset.x + horizontalSway, player.position.y + startY + floatOffset, transform.position.z);
    }
}
