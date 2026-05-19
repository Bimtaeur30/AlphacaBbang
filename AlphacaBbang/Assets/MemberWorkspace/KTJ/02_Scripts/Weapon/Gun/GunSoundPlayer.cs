using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Systems.ObjectPoolSystems;
using JJH._02_Scripts.Systems.SoundSystems;
using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

public class GunSoundPlayer : MonoBehaviour
{
    [SerializeField] private EventChannelSO soundChannel;

    public void PlaySound(SoundClipSO audio)
    {
        if (audio == null) return;
        soundChannel.RaiseEvent(SoundEvents.PlaySoundEvent.Init(audio, this.transform));
        //Debug.Assert(audio != null, "오디오가 널입니다.");
        //soundChannel.RaiseEvent(SoundEvents.PlaySoundEvent.Init(transform.position, ))
    }
}
