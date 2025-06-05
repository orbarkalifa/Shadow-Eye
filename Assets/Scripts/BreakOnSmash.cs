using Suits.Duri;
using UnityEngine;

public class BreakOnSmash : Destructible
{
    public override void TakeDamage(float damage)
    {
        return;
    }

    public void TakeDamage(float damage, GameObject attacker)
    {
        if(attacker.GetComponent<RockFormEffect>())
        {
            base.TakeDamage(damage);            
        }
    }
}