namespace Suits.Abilities
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "DashMovement", menuName = "Suits/Abilities/Dash Movement")]
    public class DashMovement : SuitAbility
    {
        public float m_DashSpeed = 20f;

        public override void ExecuteAbility(GameObject character)
        {
            Rigidbody2D rb = character.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
              
                Vector2 dashVelocity = new Vector2(m_DashSpeed * rb.velocity.x, rb.velocity.y);
                rb.velocity = dashVelocity;

                Debug.Log($"{character.name} dashed with speed {m_DashSpeed}.");
            }
        }
    }    
}

