using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private BeaconSO beacon;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            beacon.playerChannel.UpdateCheckpoint(transform);
        }
    }
}
