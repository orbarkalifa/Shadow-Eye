using UnityEngine;
using System.Collections.Generic;
using System.Linq; 
using Suits; 

[CreateAssetMenu(fileName = "SuitDatabase", menuName = "Game/Suit Database")]
public class SuitDatabase : ScriptableObject
{
    [SerializeField]
    private List<Suit> allSuits;

    public Suit GetSuitByName(string suitAssetName)
    {
        if (string.IsNullOrEmpty(suitAssetName))
        {
            return null;
        }
        return allSuits.FirstOrDefault(suit => suit.name == suitAssetName);
    }
}