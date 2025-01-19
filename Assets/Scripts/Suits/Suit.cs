using UnityEngine;
using UnityEngine.Serialization;

public class Suit : ScriptableObject
{
    public string m_SuitName;
    public Sprite m_SuitSprite; // visual representation
    public SuitAbility m_SpecialAttack;
    public SuitAbility m_SpecialMovement;
    public GameObject m_SuitPrefab; // For the equipped visual
}