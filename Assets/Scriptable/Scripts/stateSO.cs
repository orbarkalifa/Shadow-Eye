using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
[CreateAssetMenu(fileName = "new State", menuName = "New State", order = 0)]
public class stateSO : ScriptableObject
{
    public GameState state;
    public enum GameState
    {
        start,
        inGame,
        menu
    }

    public bool canMenu;
    public bool canMove;
}
