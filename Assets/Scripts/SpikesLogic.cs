using Player;
using UnityEngine;

public class SpikesLogic : MonoBehaviour
{
    [SerializeField] private BeaconSO beacon;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
                ResetPlayer();
        }
    }

    private void ResetPlayer()
    {
        beacon.playerChannel.DealDamage(1, transform.position);
        Debug.Log("SPIKED");
        beacon.playerChannel.NotifyHitSpikes();
    }
}
