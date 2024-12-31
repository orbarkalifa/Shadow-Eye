using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedBoxAttack : MonoBehaviour
{
    public LayerMask AffectedLayers; // Layers of objects that can take damage
    public int Damage = 10; // Amount of damage inflicted
    public Vector2 AreaSize = new Vector2(2f, 2f); // Size of the damage area
    public float DamageInterval = 1f; // Time between each damage application

    private float _nextDamageTime;

    private void Update()
    {
        if (Time.time >= _nextDamageTime)
        {
            ApplyDamage();
            _nextDamageTime = Time.time + DamageInterval;
        }
    }

    private void ApplyDamage()
    {
        // Get all colliders in the area
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(transform.position, AreaSize, 0, AffectedLayers);

        foreach (Collider2D collider in hitColliders)
        {
            Character gotHit = collider.GetComponent<Character>();
            if (gotHit != null)
            {
                gotHit.TakeDamage(Damage);
                Debug.Log($"{collider.name} took {Damage} damage.");
            }
        }
    }

    // Draw the damage area in the editor for visualization
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, AreaSize);
    }
}
    
