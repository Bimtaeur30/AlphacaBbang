using MemberWorkspace.CHG._02_Scripts.SettingUI;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ScreenBrightnessSetting : AbstractSettingUI
{
    [SerializeField] private Slider _brightnessSlider;
    [SerializeField] private Volume _volume;
    [SerializeField] private TextMeshProUGUI _brightnessText;

    private float _currentBrightness;
    private float _lastBrightness;
    public override void Awake()
    {
        _brightnessSlider.onValueChanged.AddListener(ChangeBrightness);
    }

    public override void OnEnable()
    {
        
    }

    public override void SettingData()
    {
        _lastBrightness = _currentBrightness;
        //_volume.profile.TryGet(out cool)
    }

    public override void ResetData()
    {
        
    }
    
    private void ChangeBrightness(float value)
    {
        _currentBrightness = value;
    }
}
