using System.Collections;
using UnityEngine;

public class DashMovement : SuitAbility
{

    public override void ExecuteAbility(GameObject character)
    {
        CharacterMovement movement = character.GetComponent<CharacterMovement>();
        movement.Dash();
    }
    
    
}