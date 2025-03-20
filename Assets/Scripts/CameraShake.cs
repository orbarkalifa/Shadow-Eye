using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    private Vector3 initialPosition;
    private bool isShaking = false;

    void Start()
    {
        initialPosition = transform.position;
    }
    public void ShakeCamera(float duration, float magnitude)
    {
        if (!isShaking)
        {
            StartCoroutine(ShakeRoutine(duration, magnitude));
        }
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        isShaking = true;
        float timer = 0f;

        initialPosition = transform.position;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            
            Vector3 shakeOffset = (transform.right * Random.Range(-1f, 1f) + transform.up * Random.Range(-1f, 1f)) * magnitude;

            transform.position = Vector3.Lerp(transform.position, initialPosition + shakeOffset, 0.5f);

            yield return null;
        }

        transform.position = initialPosition;
        isShaking = false;
    }
}