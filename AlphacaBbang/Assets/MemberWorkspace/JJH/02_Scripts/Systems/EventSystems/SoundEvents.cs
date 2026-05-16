using JJH._02_Scripts.Systems.SoundSystems;
using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

namespace JJH._02_Scripts.Systems.EventSystems
{
    public static class SoundEvents
    {
        public static readonly PlaySoundEvent PlaySoundEvent = new PlaySoundEvent();
        public static readonly StopSoundEvent StopSoundEvent = new StopSoundEvent();
    }

    public class PlaySoundEvent : GameEvent
    {
        public Transform Trans { get; private set; }
        public SoundClipSO ClipData { get; private set; }
        public int ChannelNumber { get; private set; }

        public PlaySoundEvent Init(SoundClipSO clipData, Transform trans = null, int channelNumber = 0)
        {
            Trans = trans;
            ClipData = clipData;
            ChannelNumber = channelNumber;
            return this;
        }
    }

    public class StopSoundEvent : GameEvent
    {
        public int ChannelNumber { get; private set; }

        public StopSoundEvent Init(int channelNumber = 0)
        {
            ChannelNumber = channelNumber;
            return this;
        }
    }
}