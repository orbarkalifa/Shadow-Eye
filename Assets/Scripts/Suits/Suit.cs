using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewSuit", menuName = "Suits/Suit")]
public class Suit : ScriptableObject
{
    public string m_SuitName;
    public Sprite m_SuitSprite; // visual representation
    public SuitAbility m_SpecialAttack;
    public SuitAbility m_SpecialMovement;
    public GameObject m_SuitPrefab; // For the equipped visual
}