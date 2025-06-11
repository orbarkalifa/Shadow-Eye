using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakOnProjectile : Destructible
{
    public override void TakeDamage(float damage)
    {
        return;
    }
    public void TakeDamage(float damage, GameObject attacker)
    {
        if(attacker.GetComponent<Projectile>())
        {
            base.TakeDamage(damage);            
        }
    }
}
