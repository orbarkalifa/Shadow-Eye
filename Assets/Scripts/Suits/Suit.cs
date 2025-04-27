using UnityEngine;
using UnityEngine.UI;

public class Suit : ScriptableObject
{
    public string suitName;
    public Sprite suitSprite; // visual representation
    public SuitAbility specialAttack;
    public SuitAbility specialMovement;
    public GameObject suitPrefab; // For the equipped visual
    public float attackRange = 4f;
    public float cooldownTime = 0.2f;
    public Sprite hudSprite;
}