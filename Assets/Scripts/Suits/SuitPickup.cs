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

            if (collision.CompareTag("Player"))
            {
                MainCharacter player = collision.GetComponent<MainCharacter>();
                if (player)
                {
                    player.EquipSuit(m_Suit);
                    Destroy(gameObject);
                }
            }
        }
    }
}


