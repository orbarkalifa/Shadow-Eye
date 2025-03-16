using UnityEngine;
using UnityEngine.U2D.Animation;

public class Suit : ScriptableObject
{
    public string suitName;
    public Sprite suitSprite; // visual representation
    public SuitAbility specialAttack;
    public SuitAbility specialMovement;
    public GameObject suitPrefab; // For the equipped visual
}