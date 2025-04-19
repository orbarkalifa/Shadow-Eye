using UnityEngine.Serialization;

namespace Suits
{
    using UnityEngine;

    public class SuitPickup : MonoBehaviour
    {
         [SerializeField] private Suit suit;

        public void Initialize(Suit suit)
        {
            this.suit = suit;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.CompareTag("Player"))
            {
                MainCharacter player = collision.GetComponent<MainCharacter>();
                if (player)
                {
                    player.EquipSuit(suit);
                    Destroy(gameObject);
                }
            }
        }
    }
}


