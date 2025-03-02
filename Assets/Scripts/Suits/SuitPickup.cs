namespace Suits
{
    using UnityEngine;

    public class SuitPickup : MonoBehaviour
    {
        private Suit m_Suit;

        public void Initialize(Suit suit)
        {
            m_Suit = suit;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log($"{collision.name} entered the suit pickup trigger.");

            if (collision.CompareTag("Player"))
            {
                MainCharacter player = collision.GetComponent<MainCharacter>();
                if (player)
                {
                    Debug.Log($"Player picked up {m_Suit.suitName}.");
                    player.EquipSuit(m_Suit);
                    Destroy(gameObject);
                }
            }
        }
    }
}


