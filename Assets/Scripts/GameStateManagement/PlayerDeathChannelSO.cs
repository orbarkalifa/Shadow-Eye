using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Channels/Player Death Channel")]
public class PlayerDeathChannelSO : ScriptableObject
{
    public UnityAction<PlayerSaveData> OnPlayerDied;

    public void Raise(PlayerSaveData data)
    {
        OnPlayerDied?.Invoke(data);
    }
}