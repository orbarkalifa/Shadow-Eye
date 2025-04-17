using System;
using UnityEngine;
using Cinemachine;
namespace GameStateManagement
{
    [CreateAssetMenu(fileName = "UIChannel", menuName = "Channel/UI Channel")] 
    public class UIChannelSO :ScriptableObject
    {
        public event Action<int> OnChangeHealth;
        public event Action<float> Onload;
        public event Action<string> OnChangeLevel;
        public void ChangeHealth(int health)
        {
            OnChangeHealth?.Invoke(health);
        }

        public void ChangeLevel(string level)
        {
            OnChangeLevel?.Invoke(level);
        }

        public void PassLoadPercent(float percent)
        {
            Onload?.Invoke(percent);
        }
    }
}