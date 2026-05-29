using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MemberWorkspace.CHG._02_Scripts.SettingUI
{
    public class SoundSetting : AbstractSettingUI
    {
        [SerializeField] private string audioMixerParam;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private TextMeshProUGUI volumeLabel;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Toggle muteToggle;

        private static readonly Dictionary<string, float> _savedVolumes = new();
        private static readonly Dictionary<string, bool> _savedMutes = new();

        private float _currentVolume;
        private bool _isMute;

        public override void Awake()
        {
            volumeSlider.onValueChanged.AddListener(ChangeVolume);
            muteToggle.onValueChanged.AddListener(Mute);
            ResetData();
        }

        public override void OnEnable()
        {
            float savedVolume = GetSavedVolume();

            volumeSlider.onValueChanged.RemoveListener(ChangeVolume);
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(ChangeVolume);

            _currentVolume = savedVolume;
            _isMute = GetSavedMute();

            muteToggle.onValueChanged.RemoveListener(Mute);
            muteToggle.isOn = _isMute;
            muteToggle.onValueChanged.AddListener(Mute);

            UpdateLabel(savedVolume);
        }

        private void OnDestroy()
        {
            volumeSlider.onValueChanged.RemoveListener(ChangeVolume);
            muteToggle.onValueChanged.RemoveListener(Mute);
        }

        public override void SettingData()
        {
            float mixerValue;

            if (_isMute)
            {
                mixerValue = -80f;
            }
            else if (Mathf.Approximately(_currentVolume, -40f))
            {
                mixerValue = -80f; 
            }
            else
            {
                mixerValue = _currentVolume;
            }

            audioMixer.SetFloat(audioMixerParam, mixerValue);

            _savedVolumes[audioMixerParam] = _currentVolume;
            _savedMutes[audioMixerParam] = _isMute;
        }

        public override void ResetData()
        {
            if (_savedVolumes.TryGetValue(audioMixerParam, out float saved))
            {
                _currentVolume = saved;
            }
            else
            {
                audioMixer.GetFloat(audioMixerParam, out _currentVolume);
                _savedVolumes[audioMixerParam] = _currentVolume;
            }

            _isMute = GetSavedMute();

            volumeSlider.onValueChanged.RemoveListener(ChangeVolume);
            volumeSlider.value = _currentVolume;
            volumeSlider.onValueChanged.AddListener(ChangeVolume);

            muteToggle.onValueChanged.RemoveListener(Mute);
            muteToggle.isOn = _isMute;
            muteToggle.onValueChanged.AddListener(Mute);

            UpdateLabel(_currentVolume);

            ApplyToMixer();
        }

        private void ChangeVolume(float volume)
        {
            _currentVolume = volume;
            UpdateLabel(volume);
        }

        public void Mute(bool value)
        {
            _isMute = value;
        }
        private float GetSavedVolume()
            => _savedVolumes.TryGetValue(audioMixerParam, out float v) ? v : 0f;

        private bool GetSavedMute()
            => _savedMutes.TryGetValue(audioMixerParam, out bool m) && m;

        private void UpdateLabel(float volume)
        {
            float t = Mathf.InverseLerp(-40f, 0f, volume);
            int result = Mathf.RoundToInt(t * 100f);
            volumeLabel.text = $"{result}";
        }

        private void ApplyToMixer()
        {
            float mixerValue;
            if (_isMute || Mathf.Approximately(_currentVolume, -40f))
                mixerValue = -80f;
            else
                mixerValue = _currentVolume;

            audioMixer.SetFloat(audioMixerParam, mixerValue);
        }
    }
}