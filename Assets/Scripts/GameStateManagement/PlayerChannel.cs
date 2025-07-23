namespace GameStateManagement
{ 
    using UnityEngine;
    using UnityEngine.Events;
    using Suits;

    [CreateAssetMenu(menuName = "Channels/Player Channel")]
    public class PlayerChannel : ScriptableObject
    {
        public UnityEvent<int, Vector2> OnPlayerDamaged;
        public UnityEvent OnPlayerDied;
        public UnityEvent<Suit> OnSuitChanged;

        public Vector2 CurrentPosition { get; private set; }
        public Suit CurrentSuit { get; private set; }
        public bool IsInvincible { get; private set; }

        public bool IsAlive { get; private set; } = true;
        public void UpdatePosition(Vector2 pos) => CurrentPosition = pos;
        
        public void DealDamage(int amount, Vector2 source) => OnPlayerDamaged?.Invoke(amount, source);

        public void NotifyDeath() => _ = IsAlive = false;
        public void NotifySpawn() => _ = IsAlive = true;

        public void UpdateSuit(Suit newSuit)
        {
            CurrentSuit = newSuit;
            OnSuitChanged?.Invoke(newSuit);
        }

        public void SetInvincible(bool state) => IsInvincible = state;
    }

}