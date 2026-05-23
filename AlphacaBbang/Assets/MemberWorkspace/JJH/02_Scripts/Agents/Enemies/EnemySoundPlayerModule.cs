using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Systems.SoundSystems;
using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies
{
    public class EnemySoundPlayerModule : MonoBehaviour, IModule, IEnemySoundPlayer
    {
        [SerializeField] private EventChannelSO soundChannel;

        private AbstractEnemy _owner;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner as AbstractEnemy;
        }

        public void PlaySound(SoundClipSO soundClip)
        {
            soundChannel.RaiseEvent(SoundEvents.PlaySoundEvent.Init(soundClip, this.transform));
        }
    }
}