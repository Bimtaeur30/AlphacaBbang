using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

public class EscapeHole : MonoBehaviour
{
    [SerializeField] private EventChannelSO SystemChannel;
    public void Escape()
    {
        SystemChannel.RaiseEvent(SystemEvents.OnGameEnd.Init(true));
    }
}
