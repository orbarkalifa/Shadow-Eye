using System.Collections;
using UnityEngine;

public class DashMovement : SuitAbility
{

    public override void ExecuteAbility(GameObject i_Character)
    {
        CharacterMovement movement = i_Character.GetComponent<CharacterMovement>();
        movement.Dash();
    }
    
    
}