/*
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager m_Instance;
    public List<GameObject> m_WeaponPrefabs;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
            Debug.Log("WeaponManager initialized.");
        }
        else
        {
            Debug.LogWarning("Multiple Weapon Manager instances detected. Using the first one.");   
        }
    }


    public GameObject GetWeaponByName(string i_WeaponName)
    {
        foreach (GameObject weapon in m_WeaponPrefabs)
        {
            if (weapon.name == i_WeaponName)
            {
                return weapon;
            }
        }
        return null;
    }
}
*/

