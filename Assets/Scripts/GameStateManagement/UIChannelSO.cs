using System;
using UnityEngine;

namespace GameStateManagement
{
    [CreateAssetMenu(fileName = "UIChannel", menuName = "Channel/UI Channel")] 
    public class UIChannelSO :ScriptableObject
    {
        public event Action<int> OnChangeHealth;

        public event Action InitListener; 
        public event Action<Sprite> OnHudChange;
        public event Action<float> Onload;
        public event Action<string , GameStateSO> OnChangeLevel;
        public void ChangeHealth(int health)
        {
            OnChangeHealth?.Invoke(health);
        }

        public void ChangeHud(Sprite hub)
        {
            OnHudChange?.Invoke(hub);
        }

        public void ChangeLevel(string level, GameStateSO state)
        {
            OnChangeLevel?.Invoke(level,state);
        }
        
        public void PassLoadPercent(float p)
        {
            Onload?.Invoke(p);
        }

        public void InitializeListeners()
        {
            InitListener?.Invoke();
        }
    }
}