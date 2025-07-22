namespace GameStateManagement
{
    // PlayerChannel.cs
    using UnityEngine;
    using UnityEngine.Events;
    using Suits;

    [CreateAssetMenu(menuName = "Channels/Player Channel")]
    public class PlayerChannel : ScriptableObject
    {
        public UnityEvent<Vector2> OnPositionUpdated;
        public UnityEvent<int, int> OnHealthChanged;
        public UnityEvent<int, Vector2> OnPlayerDamaged;
        public UnityEvent OnPlayerDied;
        public UnityEvent<Suit> OnSuitChanged;

        public Vector2 CurrentPosition { get; private set; }
        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; private set; }
        public Suit CurrentSuit { get; private set; }
        public bool IsInvincible { get; private set; }

        public void UpdatePosition(Vector2 pos) => CurrentPosition = pos;

        public void UpdateHealth(int current, int max)
        {
            CurrentHealth = current;
            MaxHealth = max;
            OnHealthChanged?.Invoke(current, max);
        }

        public void NotifyDamaged(int amount, Vector2 source) => OnPlayerDamaged?.Invoke(amount, source);

        public void NotifyDeath() => OnPlayerDied?.Invoke();

        public void UpdateSuit(Suit newSuit)
        {
            CurrentSuit = newSuit;
            OnSuitChanged?.Invoke(newSuit);
        }

        public void SetInvincible(bool state) => IsInvincible = state;
    }

}