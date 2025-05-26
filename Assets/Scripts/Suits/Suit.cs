using Suits;
using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "New Suit", menuName = "Suit")]
public class Suit : ScriptableObject
{
    public string suitName;
    public Sprite suitSprite; // visual representation
    public SuitAbility specialAttack;
    public SuitAbility specialMovement;
    public GameObject suitPrefab; // For the equipped visual

    [Header("Basic Attack Modifiers")]
    public float attackRange = 4f;
    public float basicAttackCooldown = 0.2f;
    public float basicAttackAnimationSpeed = 1.0f; // Default to normal speed

    [Header("UI & Visuals")]
    public Sprite hudSprite;
    public SpriteLibraryAsset spriteLibrary;
}