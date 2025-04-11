using UnityEngine;
using GameStateManagement;

public class WinTrigger: MonoBehaviour
{
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] BeaconSO beacon;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if(trigger.gameObject.CompareTag("Player"))
        {
            var state = beacon.gameStateChannel.GetGameStateByName("WinState");
            beacon.gameStateChannel.RaiseStateTransitionRequest(state);
        }
    }
}

