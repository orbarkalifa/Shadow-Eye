using GameStateManagement;
using UnityEngine;

[CreateAssetMenu(fileName = "ChannelBeacon", menuName = "Beacon/Channel Beacon")]
public class BeaconSO : ScriptableObject
{
    [Header("Game State Channels")]
    public GameStateChannelSO gameStateChannel;
    [Header("UI Channels")]
    public UIChannelSO uiChannel;
    
    public PlayerChannel playerChannel;

    // Add more channels here as needed
}