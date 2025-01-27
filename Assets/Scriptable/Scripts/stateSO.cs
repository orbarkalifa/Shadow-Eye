using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[SerializeField]
[CreateAssetMenu(fileName = "new State", menuName = "New State", order = 0)]
public class stateSO : ScriptableObject
{
    [FormerlySerializedAs("state")]
    public gameStates m_States;
    public enum gameStates
    {
        start,
        inGame,
        menu
    }

    public bool canMenu;
    public bool canMove;
}
