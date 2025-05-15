using System;
using System.Collections;
using Suits;
using UnityEngine;

[CreateAssetMenu(fileName = "Dash", menuName = "Ability/Dash")]
public class DashMovement : SuitAbility
{
    public override void ExecuteAbility(GameObject character)
    {
        CharacterMovement movement = character.GetComponent<CharacterMovement>();
        movement.Dash();
    }
    
    
}