using MemberWorkspace.CHG._02_Scripts.SettingUI;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenBrightnessSetting : AbstractSettingUI
{
    [SerializeField] private Slider _brightnessSlider;
    private Volume _volume;
    [SerializeField] private TextMeshProUGUI _brightnessText;
    [SerializeField] VolumeProfile profile;
    private ColorAdjustments _adjustment;
    private float _currentBrightness;
    private float _lastBrightness;

    public override void Awake()
    {
        _brightnessSlider.onValueChanged.AddListener(ChangeBrightness);
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitVolumeFromScene();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        _brightnessSlider.onValueChanged.RemoveListener(ChangeBrightness);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitVolumeFromScene();
    }

    private void InitVolumeFromScene()
    {
        _volume = FindFirstObjectByType<Volume>();

        if (_volume == null)
        {
            Debug.LogWarning($"씬에서 Volume을 찾을 수 없습니다: {gameObject.name}");
            return;
        }

        profile = _volume.profile;

        if (!profile.TryGet(out _adjustment))
        {
            Debug.LogWarning($"VolumeProfile에 ColorAdjustments가 없습니다: {gameObject.name}");
            return;
        }

        _adjustment.postExposure.value = _lastBrightness;
    }

    public override void OnEnable()
    {
        _brightnessSlider.value = _lastBrightness;

        float t = Mathf.InverseLerp(-4, 2f, _lastBrightness);
        int result = Mathf.RoundToInt(t * 100f);

        _brightnessText.text = $"{result}";
    }

    public override void SettingData()
    {
        _lastBrightness = _currentBrightness;

        if (_adjustment != null)
            _adjustment.postExposure.value = _currentBrightness;
    }

    public override void ResetData()
    {
        _currentBrightness = 0;
        _lastBrightness = 0;
        _brightnessSlider.value = _lastBrightness;

        float t = Mathf.InverseLerp(-4, 2f, _lastBrightness);
        int result = Mathf.RoundToInt(t * 100f);
        _brightnessText.text = $"{result}";

        if (_adjustment != null)
            _adjustment.postExposure.value = _currentBrightness;
    }

    private void ChangeBrightness(float value)
    {
        _currentBrightness = value;
        float t = Mathf.InverseLerp(-4, 2f, value);
        int result = Mathf.RoundToInt(t * 100f);
        _brightnessText.text = $"{result}";
    }
}