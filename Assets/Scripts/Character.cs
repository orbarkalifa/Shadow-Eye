using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    private float HP;
    private float MaxHP;
    private GameObject Weapon;
    private GameObject Movement;

    public Character(float i_MaxHp)
    {
        MaxHP = i_MaxHp;
        HP = MaxHP;
    }
    public float GetHP(){return HP;}

    public void SwapWeapon(GameObject i_Weapon)
    {
        Weapon = i_Weapon;
    }

    public void SwapMovement(GameObject i_Movement)
    {
        Movement = i_Movement;
    }

    public void Heal()
    {
        HP += 20;
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;
    }
    
}