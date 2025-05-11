using System;
using System.Collections;
using Suits;
using UnityEngine;

public class DashMovement : SuitAbility
{
    private void Awake()
    {
        cooldownTime = 1.2f;
    }

    public override void ExecuteAbility(GameObject character)
    {
        CharacterMovement movement = character.GetComponent<CharacterMovement>();
        movement.Dash();
    }
    
    
}