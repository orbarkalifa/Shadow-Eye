using System;
using UnityEngine;

namespace Scriptable.Scripts
{
    public class HealthChannelSo : ScriptableObject
    {
        public event Action<int> OnChangeHealth;
        public void ChangeHealth(int health)
        {
            OnChangeHealth?.Invoke(health);
        }
    }
}
