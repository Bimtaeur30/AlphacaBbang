using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Systems.ObjectPoolSystems;
using JJH._02_Scripts_Systems.EventSystems;
using System.Collections.Generic;
using UnityEngine;

namespace JJH._02_Scripts.Systems.SoundSystems
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO soundItem;

        [field: SerializeField] public EventChannelSO SoundChannel { get; private set; }

        private readonly Dictionary<int, SoundPlayer> _soundPlayerDict = new();

        private void Awake()
        {
            SoundChannel.AddListener<PlaySoundEvent>(HandlePlaySoundEvent);
            SoundChannel.AddListener<StopSoundEvent>(HandleStopSoundEvent);
        }

        private void OnDestroy()
        {
            SoundChannel.RemoveListener<PlaySoundEvent>(HandlePlaySoundEvent);
            SoundChannel.RemoveListener<StopSoundEvent>(HandleStopSoundEvent);
        }

        private void HandlePlaySoundEvent(PlaySoundEvent evt)
        {
            SoundPlayer player = poolManager.Pop<SoundPlayer>(soundItem);
            if (evt.Trans != null)
            {
                player.transform.SetParent(evt.Trans);
                player.transform.position = evt.Trans.position;
            }
            player.PlaySound(evt.ClipData);
            player.OnSoundFinished += HandleSoundFinish;

            if (evt.ChannelNumber > 0 && evt.ClipData.loop)
            {
                if (_soundPlayerDict.TryGetValue(evt.ChannelNumber, out SoundPlayer beforePlayer))
                {
                    beforePlayer.ForceStopSound();
                    poolManager.Push(beforePlayer);
                    _soundPlayerDict.Remove(evt.ChannelNumber);
                }
                _soundPlayerDict.Add(evt.ChannelNumber, player);
            }
            else if (evt.ChannelNumber <= 0 && evt.ClipData.loop)
            {
                Debug.LogWarning($"Channel must be greater than 0, when the Sound data loop is enabled : {evt.ClipData.name}");
            }
        }

        private void HandleSoundFinish(SoundPlayer player)
        {
            player.OnSoundFinished -= HandleSoundFinish;
            poolManager.Push(player);
        }


        private void HandleStopSoundEvent(StopSoundEvent evt)
        {
            if (_soundPlayerDict.TryGetValue(evt.ChannelNumber, out SoundPlayer beforePlayer))
            {
                beforePlayer.ForceStopSound();
                beforePlayer.OnSoundFinished -= HandleSoundFinish;
                poolManager.Push(beforePlayer);
                _soundPlayerDict.Remove(evt.ChannelNumber);
            }
        }
    }
}