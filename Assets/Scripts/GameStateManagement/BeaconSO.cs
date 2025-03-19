using GameStateManagement;
using Scriptable.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "ChannelBeacon", menuName = "Beacon/Channel Beacon")]
public class BeaconSO : ScriptableObject
{
    [Header("Game State Channels")]
    public GameStateChannelSO gameStateChannel;

    [Header("UI Channels")]
    public HealthChannelSo healthChannel;

    // Add more channels here as needed
}