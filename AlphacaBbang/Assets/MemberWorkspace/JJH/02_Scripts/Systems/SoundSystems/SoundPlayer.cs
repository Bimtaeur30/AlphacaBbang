using JJH._02_Scripts.Systems.ObjectPoolSystems;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace JJH._02_Scripts.Systems.SoundSystems
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundPlayer : MonoBehaviour, IPoolable
    {
        [SerializeField] private AudioMixerGroup bgmGroup;
        [SerializeField] private AudioMixerGroup sfxGroup;

        private AudioSource _audioSource;
        public GameObject GameObject => this == null ? null : gameObject;
        [field: SerializeField] public PoolItemSO PoolType { get; set; }

        public event Action<SoundPlayer> OnSoundFinished;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlaySound(SoundClipSO clipData)
        {

            if (clipData.audioType == AudioTypes.Sfx)
            {
                _audioSource.outputAudioMixerGroup = sfxGroup;
            }
            else if (clipData.audioType == AudioTypes.Music)
            {
                _audioSource.outputAudioMixerGroup = bgmGroup;
            }

            _audioSource.volume = clipData.volume;
            _audioSource.pitch = clipData.pitch;
            if (clipData.randomizePitch)
            {
                _audioSource.pitch += Random.Range(-clipData.randomPitchModifier, clipData.randomPitchModifier);
            }
            _audioSource.clip = clipData.clip;

            _audioSource.loop = clipData.loop;

            if (!clipData.loop)
            {
                float time = _audioSource.clip.length + .2f;
                _ = DisableSoundTimer(time);
            }
            _audioSource.Play();
        }

        public void ResetItem()
        {
            //do nothing
        }

        private async Task DisableSoundTimer(float time)
        {
            await Awaitable.WaitForSecondsAsync(time);
            OnSoundFinished?.Invoke(this);
        }

        public void ForceStopSound()
        {
            _audioSource.Stop();
        }

    }
}