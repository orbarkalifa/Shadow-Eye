using UnityEngine;
using Cinemachine;

public class MapLimit : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    
    private void OnTriggerEnter2D(Collider2D other)
    {

        if(!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (vcam != null)
        {
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

        if(!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (vcam != null)
        {
            vcam.enabled = false;
            vcam.Priority = 0;
        }
        else
        {
            Debug.LogError($"Cannot set priority: VCam is not assigned on {gameObject.name}!", this);
        }
    }
}
