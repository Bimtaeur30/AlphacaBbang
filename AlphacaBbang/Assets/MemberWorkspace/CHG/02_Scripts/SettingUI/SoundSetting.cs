using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MemberWorkspace.CHG._02_Scripts.SettingUI
{
    public class SoundSetting : MonoBehaviour
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
        
        private void Awake()
        {
            volumeSlider.onValueChanged.AddListener(ChangeVolume);
            muteToggle.onValueChanged.AddListener(Mute);
            UpperParam = audioMixerParam.ToUpper();
            ResetSound();
        }

        private void OnEnable()
        {
            volumeSlider.value = _lastVolume;
            
            float t = Mathf.InverseLerp(-80f, 0f, _lastVolume);
            int result = Mathf.RoundToInt(t * 100f);

            volumeLabel.text = $"{UpperParam}: {result}";
            volumeSlider.value = 0;
            muteToggle.isOn = false;
        }

        private void ResetSound()
        {
            _lastVolume = 0f;
            _currentVolume = 0f;
            audioMixer.SetFloat(audioMixerParam, _lastVolume);
            _isMute = false;
            volumeLabel.text = $"{UpperParam}: {_lastVolume}";
        }

        private void ChangeVolume(float volume)
        {
            _currentVolume = volume;
            float t = Mathf.InverseLerp(-80f, 0f, volume);
            int result = Mathf.RoundToInt(t * 100f);

            volumeLabel.text = $"{UpperParam}: {result}";
        }

        public void SetVolume()
        {
            if (!_isMute)
                audioMixer.SetFloat(audioMixerParam, _currentVolume);
            else 
                audioMixer.SetFloat(audioMixerParam, -80f);
            _lastVolume = _currentVolume;
        }

        public void Mute(bool value) => _isMute = value;
    }
}