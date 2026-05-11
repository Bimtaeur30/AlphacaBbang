using System;
using System.Linq;
using MemberWorkspace.CHG._02_Scripts.TextBoxSystem;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

namespace MemberWorkspace.CHG._02_Scripts.SettingUI
{
    public class SettingPanel : MonoBehaviour
    {
        [SerializeField] private UIDocument settingsPanel;
        [SerializeField] private ScreenSetting screenSetting;
        [SerializeField] private AudioMixer _audioMixer;
        
        private DropdownField _resolutionField;
        private TabView _tabView;
        private DropdownField _screenModeField;
        private Button _saveButton;
        private SliderInt _masterVolumeSlider;
        private SliderInt _bGMVolumeSlider;
        private SliderInt _sFXVolumeSlider;
        
        private Resolution[] _resolutions;
        private Resolution _currentResolution;  
        private int _currentResolutionIndex = -1;
        
        private FullScreenMode _screenMode;
        private int _masterVolume;
        private int _bGMVolume;
        private int _sFXVolume;
        private void Awake()
        {
            var root = settingsPanel.rootVisualElement;
            
            _tabView = root.Q<TabView>("TabView");
            _saveButton = root.Q<Button>("Save");
            // Screen
            _resolutionField = root.Q<DropdownField>("ScreenResolution");
            _screenModeField = root.Q<DropdownField>("ScreenMode");
            Debug.Assert(_resolutionField != null, $"ResolutionField was null in inspector: {gameObject.name}");
            Debug.Assert(_screenModeField != null, $"ScreenModeField was null in inspector: {gameObject.name}");
            Debug.Assert(_saveButton != null, $"SaveButton was null in inspector: {gameObject.name}");
            // Sound 
            _masterVolumeSlider = root.Q<SliderInt>("MasterVolume");
            _bGMVolumeSlider = root.Q<SliderInt>("BGMVolume");
            _sFXVolumeSlider = root.Q<SliderInt>("SFXVolume");
            Debug.Assert(_masterVolumeSlider != null, $"masterVolumeSlider was null in inspector: {gameObject.name}");
            Debug.Assert(_bGMVolumeSlider  != null, $"bgmVolumeSlider was null in inspector: {gameObject.name}");
            Debug.Assert(_sFXVolumeSlider != null, $"sfxVolumeSlider was null in inspector: {gameObject.name}");
            
            
            _resolutions = Screen.resolutions
                .GroupBy(r => (r.width, r.height))
                .Select(g => g.Last()) 
                .OrderBy(r => r.width)
                .ThenBy(r => r.height)
                .ToArray();

            foreach (var resolution in _resolutions)
            {
                Debug.Log($"{resolution.width}x{resolution.height} : {resolution.refreshRate}");
                _resolutionField.choices.Add(resolution.width + "x" + resolution.height);
            }

#region Screen
            _resolutionField.RegisterValueChangedCallback(evt =>
            {
                var value = evt.newValue.Split('x');
                _currentResolution.width = int.Parse(value[0]);
                _currentResolution.height = int.Parse(value[1]);
                _currentResolutionIndex = _resolutionField.index;
            });

            _screenModeField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == "전체화면")
                    _screenMode = FullScreenMode.ExclusiveFullScreen;
                else if (evt.newValue == "창")
                    _screenMode = FullScreenMode.Windowed;
                else if (evt.newValue == "테두리 없는 창")
                    _screenMode = FullScreenMode.FullScreenWindow;
            });
#endregion

#region Sound
            _masterVolumeSlider.RegisterValueChangedCallback(evt => _masterVolume = (int)evt.newValue);
            _bGMVolumeSlider.RegisterValueChangedCallback(evt => _bGMVolume = (int)evt.newValue);
            _sFXVolumeSlider.RegisterValueChangedCallback(evt => _sFXVolume = (int)evt.newValue);
#endregion            

            _saveButton.clicked += () =>
            {
                Tab curTab = _tabView.activeTab;
                if (curTab.name == "Screen")
                    screenSetting.ChangeResolution(_currentResolution.width,_currentResolution.height, _screenMode);
                else if (curTab.name == "Sound")
                {
                    _audioMixer.SetFloat("Master", _masterVolume);
                    _audioMixer.SetFloat("Bgm", _bGMVolume);
                    _audioMixer.SetFloat("Sfx", _sFXVolume);
                }
            };
        }
    }
}