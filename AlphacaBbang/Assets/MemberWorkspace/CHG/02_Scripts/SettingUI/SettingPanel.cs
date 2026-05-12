using System.Linq;
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
        
        private VisualElement _root;
        private DropdownField _resolutionField;
        private TabView _tabView;
        private DropdownField _screenModeField;
        private Button _saveButton;
        private Button _exitButton;
        private SliderInt _masterVolumeSlider;
        private SliderInt _bGMVolumeSlider;
        private SliderInt _sFXVolumeSlider;
        
        private Resolution[] _resolutions;
        private Resolution _currentResolution;  
        
        private FullScreenMode _screenMode;
        private int _masterVolume;
        private int _bGMVolume;
        private int _sFXVolume;
        private void Awake()
        {
            _root = settingsPanel.rootVisualElement;
            _root.style.display  = DisplayStyle.None;
            _tabView = _root.Q<TabView>("TabView");
            _saveButton = _root.Q<Button>("Save");
            _exitButton = _root.Q<Button>("Exit");
            // Screen
            _resolutionField = _root.Q<DropdownField>("ScreenResolution");
            _screenModeField = _root.Q<DropdownField>("ScreenMode");
            Debug.Assert(_resolutionField != null, $"ResolutionField was null in inspector: {gameObject.name}");
            Debug.Assert(_screenModeField != null, $"ScreenModeField was null in inspector: {gameObject.name}");
            Debug.Assert(_saveButton != null, $"SaveButton was null in inspector: {gameObject.name}");
            // Sound 
            _masterVolumeSlider = _root.Q<SliderInt>("MasterVolume");
            _bGMVolumeSlider = _root.Q<SliderInt>("BGMVolume");
            _sFXVolumeSlider = _root.Q<SliderInt>("SFXVolume");
            Debug.Assert(_masterVolumeSlider != null, $"masterVolumeSlider was null in inspector: {gameObject.name}");
            Debug.Assert(_bGMVolumeSlider  != null, $"bgmVolumeSlider was null in inspector: {gameObject.name}");
            Debug.Assert(_sFXVolumeSlider != null, $"sfxVolumeSlider was null in inspector: {gameObject.name}");
            
            
            _resolutions = Screen.resolutions
                .GroupBy(r => (r.width, r.height))
                .Select(g => g.Last()) 
                .OrderBy(r => r.width)
                .ThenBy(r => r.height)
                .ToArray();
            
            _resolutionField.index =  _resolutions.Length - 1;

            foreach (var resolution in _resolutions)
                _resolutionField.choices.Add(resolution.width + "x" + resolution.height);

            ScreenSetting();

            SoundSetting();

            _saveButton.clicked += () =>
            {
                Tab curTab = _tabView.activeTab;
                if (curTab.name == "Screen")
                    screenSetting.ChangeResolution(_currentResolution, _screenMode);
                else if (curTab.name == "Sound")
                {
                    _audioMixer.SetFloat("Master", _masterVolume);
                    _audioMixer.SetFloat("Bgm", _bGMVolume);
                    _audioMixer.SetFloat("Sfx", _sFXVolume);
                }
            };
            _exitButton.clicked += () => UIShowHide();
        }

        [ContextMenu("UIShowHide")]
        public void UIShowHide()
        {
            if (_root.style.display == DisplayStyle.Flex)
                _root.style.display = DisplayStyle.None;
            else 
                _root.style.display = DisplayStyle.Flex;
        }

        private void ScreenSetting()
        {
            _resolutionField.RegisterValueChangedCallback(evt =>
            {
                var value = evt.newValue.Split('x');
                _currentResolution.width = int.Parse(value[0]);
                _currentResolution.height = int.Parse(value[1]);
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
        }
        
        private void SoundSetting()
        {
            _masterVolumeSlider.RegisterValueChangedCallback(evt => _masterVolume = (int)evt.newValue);
            _bGMVolumeSlider.RegisterValueChangedCallback(evt => _bGMVolume = (int)evt.newValue);
            _sFXVolumeSlider.RegisterValueChangedCallback(evt => _sFXVolume = (int)evt.newValue);
        }
    }
}