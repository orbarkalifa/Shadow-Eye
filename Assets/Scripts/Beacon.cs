using System.Collections;
using System.Collections.Generic;
using Scriptable.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

public class Beacon : MonoBehaviour
{
    public HealthChannelSo healthChannel;
    [FormerlySerializedAs("gameStateChannel")]
    public GameStateSO m_GameStateSo;
}
