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
    public float attackRange = 4f;
    public float basicAttackCooldown = 0.2f;
    public Sprite hudSprite;
    public SpriteLibraryAsset spriteLibrary;
}