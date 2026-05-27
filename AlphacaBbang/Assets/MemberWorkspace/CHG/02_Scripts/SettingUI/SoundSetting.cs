using System;
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

        private string UpperParam;
        private float _lastVolume;
        private float _currentVolume;
        private bool _isMute;

        public override void Awake()
        {
            volumeSlider.onValueChanged.AddListener(ChangeVolume);
            muteToggle.onValueChanged.AddListener(Mute);
            UpperParam = audioMixerParam.ToUpper();
            ResetData();
        }
        
        public override void OnEnable()
        {
            volumeSlider.value = _lastVolume;
            
            float t = Mathf.InverseLerp(-80f, 0f, _lastVolume);
            int result = Mathf.RoundToInt(t * 100f);

            volumeLabel.text = $"{result}";
            muteToggle.isOn = false;
        }

        private void OnDestroy()
        {
            volumeSlider.onValueChanged.RemoveListener(ChangeVolume);
            muteToggle.onValueChanged.RemoveListener(Mute);
            
        }

        public override void SettingData()
        {
            if (!_isMute)
                audioMixer.SetFloat(audioMixerParam, _currentVolume);
            else 
                audioMixer.SetFloat(audioMixerParam, -80f);
            _lastVolume = _currentVolume;
        }

        public override void ResetData()
        {
            _lastVolume = 0f;
            _currentVolume = 0f;
            _isMute = false;
            audioMixer.SetFloat(audioMixerParam, _lastVolume);
            volumeLabel.text = $"{_lastVolume}";
            volumeSlider.value = _lastVolume;
            muteToggle.isOn = false;
            _isMute = false;
        }

        private void ChangeVolume(float volume)
        {
            _currentVolume = volume;
            float t = Mathf.InverseLerp(-80f, 0f, volume);
            int result = Mathf.RoundToInt(t * 100f);

            volumeLabel.text = $"{result}";
        }


        public void Mute(bool value) => _isMute = value;
    }
}