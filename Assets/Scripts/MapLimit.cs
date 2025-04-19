using UnityEngine;
using Cinemachine;

public class MapLimit : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"{gameObject.name} OnTriggerEnter2D with {other.name}"); // Log any collision

        if(!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (vcam != null)
        {
            Debug.Log($"Player entered {gameObject.name}. Setting Priority to 10 for VCam: {vcam.name}");
            vcam.enabled = true;
            vcam.Priority = 10;
        }
        else
        {
            Debug.LogError($"Cannot set priority: VCam is not assigned on {gameObject.name}!", this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"{gameObject.name} OnTriggerExit2D with {other.name}"); // Log any collision exit

        if(!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (vcam != null)
        {
            Debug.Log($"Player exited {gameObject.name}. Setting Priority to 0 for VCam: {vcam.name}");
            vcam.enabled = false;
            vcam.Priority = 0;
        }
        else
        {
            Debug.LogError($"Cannot set priority: VCam is not assigned on {gameObject.name}!", this);
        }
    }
}
