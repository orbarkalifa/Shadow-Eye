using System;
using UnityEngine;
using Cinemachine;
namespace GameStateManagement
{
    [CreateAssetMenu(fileName = "UIChannel", menuName = "Channel/UI Channel")] 
    public class UIChannelSO :ScriptableObject
    {
        public event Action<int> OnChangeHealth;
        public event Action<CinemachineVirtualCamera> OnChangeCamera;
        public event Action<string> OnChangeLevel;
        public void ChangeHealth(int health)
        {
            OnChangeHealth?.Invoke(health);
        }

        public void ChangeLevel(string level)
        {
            OnChangeLevel?.Invoke(level);
        }

        public void ChangeCamera(CinemachineVirtualCamera virtualCamera)
        {
            OnChangeCamera?.Invoke(virtualCamera);
        }
    }
}