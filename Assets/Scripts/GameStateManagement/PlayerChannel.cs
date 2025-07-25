using UnityEngine.Serialization;

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
        public UnityAction OnConsumeAbilityUnlocked;
        public UnityAction OnWallGrabAbilityUnlocked;
        public UnityEvent<Suit> OnSuitChanged;
        public UnityEvent<Transform> onCheckPointReached;
        public UnityEvent PlayerSpikeRpawned;
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
        public void UpdateCheckpoint(Transform pos)
        {
            onCheckPointReached?.Invoke(pos);
        }
        public void UnlockConsume()
        {
            OnConsumeAbilityUnlocked?.Invoke();
        }
        public void UnlockWallGrab()
        {
            OnWallGrabAbilityUnlocked?.Invoke();
        }

        public void NotifyHitSpikes() => PlayerSpikeRpawned?.Invoke();

        public void SetInvincible(bool state) => IsInvincible = state;
    }

}