namespace Suits
{
    using UnityEngine;

    public class SuitPickup : MonoBehaviour
    {
         [SerializeField] private Suit suit;
         [SerializeField] private BeaconSO beacon;

        public void Initialize(Suit suitToInit)
        {
            suit = suitToInit;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.CompareTag("Player"))
            { 
                beacon.playerChannel.UpdateSuit(suit); 
                Destroy(gameObject);
            }
        }
    }
}


