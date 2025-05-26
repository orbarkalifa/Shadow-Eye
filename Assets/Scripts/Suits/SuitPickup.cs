using Player;
using UnityEngine.Serialization;

namespace Suits
{
    using UnityEngine;

    public class SuitPickup : MonoBehaviour
    {
         [SerializeField] private Suit suit;

        public void Initialize(Suit suitToInit)
        {
            suit = suitToInit;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.CompareTag("Player"))
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                if (player)
                {
                    player.EquipSuit(suit);
                    Destroy(gameObject);
                }
            }
        }
    }
}


