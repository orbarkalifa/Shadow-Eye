using GameStateManagement;
using Scriptable.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "ChannelBeacon", menuName = "Beacon/Channel Beacon")]
public class BeaconSO : ScriptableObject
{
    [Header("Game State Channels")]
    public GameStateChannelSO gameStateChannel;

    [Header("UI Channels")]
    public HealthChannelSo uiChannel; // Assuming you have a UIChannelSO (example below)

    // Add more channels here as needed (e.g., AudioChannelSO, VFXChannelSO, etc.)
}