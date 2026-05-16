using MemberWorkspace.CHG._02_Scripts.SettingUI;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ScreenBrightnessSetting : AbstractSettingUI
{
    [SerializeField] private Slider _brightnessSlider;
    [SerializeField] private Volume _volume;
    [SerializeField] private TextMeshProUGUI _brightnessText;

    private ColorAdjustments _adjustment;
    private float _currentBrightness;
    private float _lastBrightness;
    public override void Awake()
    {
        _brightnessSlider.onValueChanged.AddListener(ChangeBrightness);
        ResetData();
    }

    public override void OnEnable()
    {
        _brightnessSlider.value = _lastBrightness;
        
        float t = Mathf.InverseLerp(-4, 2f, _lastBrightness);
        int result = Mathf.RoundToInt(t * 100f);
        
        _brightnessText.text = $"밝기: {result}";
        
    }

    public override void SettingData()
    {
        _lastBrightness = _currentBrightness;

        _adjustment.postExposure.value = _currentBrightness;
        

    }

    public override void ResetData()
    {
        if (!_volume.profile.TryGet(out _adjustment))
        {
            Debug.LogWarning("volume not fount ColorAdjustments: " + gameObject.name);
        }
        
        _currentBrightness = 0;
        _lastBrightness = 0;
        //ChangeBrightness();
        
    }
    
    private void ChangeBrightness(float value)
    {
        _currentBrightness = value;
        float t = Mathf.InverseLerp(-4, 2f, value);
        int result = Mathf.RoundToInt(t * 100f);

        _brightnessText.text = $"밝기: {result}";

    }
}
