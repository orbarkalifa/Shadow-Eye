using System;
using UnityEngine;

namespace GameStateManagement
{
    [CreateAssetMenu(fileName = "UIChannel", menuName = "Channel/UI Channel")] 
    public class UIChannelSO :ScriptableObject
    {
        public event Action<int> OnChangeHealth;
        public void ChangeHealth(int health)
        {
            OnChangeHealth?.Invoke(health);
        }
    }
}