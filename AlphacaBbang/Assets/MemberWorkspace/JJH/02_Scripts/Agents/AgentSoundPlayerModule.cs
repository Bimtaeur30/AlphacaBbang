using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Systems.SoundSystems;
using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

namespace JJH._02_Scripts.Agents
{
    public class AgentSoundPlayerModule : MonoBehaviour, IModule, IAgentSoundPlayer
    {
        [SerializeField] private EventChannelSO soundChannel;

        private Agent _owner;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner as Agent;
        }

        public void PlaySound(SoundClipSO soundClip)
        {
            soundChannel.RaiseEvent(SoundEvents.PlaySoundEvent.Init(soundClip, this.transform));
        }
    }
}