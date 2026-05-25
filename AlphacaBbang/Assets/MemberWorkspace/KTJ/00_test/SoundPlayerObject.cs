using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Systems.SoundSystems;
using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

public class SoundPlayerObject : MonoBehaviour
{
    [SerializeField] private SoundClipSO audioClip;
    [SerializeField] private EventChannelSO soundChannel;

    private void Start()
    {
        soundChannel.RaiseEvent(SoundEvents.PlaySoundEvent.Init(audioClip));
    }
}
