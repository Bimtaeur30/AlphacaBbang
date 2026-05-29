using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MemberWorkspace.CHG._02_Scripts.SettingUI;
using TMPro;
using UnityEngine;

public class ScreenSetting : AbstractSettingUI
{
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private TMP_Dropdown _fullScreenModeDropdown;

    private static Resolution _savedResolution;
    private static FullScreenMode _savedScreenMode = FullScreenMode.ExclusiveFullScreen;
    private static bool _hasInitialized = false;

    private Resolution _currentResolution;
    private FullScreenMode _currentScreenMode;

    public override void Awake()
    {
        _resolutionDropdown.onValueChanged.AddListener(SetResolution);
        _fullScreenModeDropdown.onValueChanged.AddListener(SetFullScreenMode);

        _resolutionDropdown.options.Clear();
        foreach (Resolution resolution in GetResolutions())
            _resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolution.width + "x" + resolution.height));

        ResetData();
    }

    public override void OnEnable()
    {
        _resolutionDropdown.onValueChanged.RemoveListener(SetResolution);
        _fullScreenModeDropdown.onValueChanged.RemoveListener(SetFullScreenMode);

        List<Resolution> resolutions = GetResolutions();
        int defaultIndex = resolutions.FindIndex(r =>
            r.width == _currentResolution.width &&
            r.height == _currentResolution.height);

        _resolutionDropdown.value = Mathf.Max(0, defaultIndex);
        _resolutionDropdown.RefreshShownValue();

        _fullScreenModeDropdown.value = (int)_currentScreenMode;
        _fullScreenModeDropdown.RefreshShownValue();

        _resolutionDropdown.onValueChanged.AddListener(SetResolution);
        _fullScreenModeDropdown.onValueChanged.AddListener(SetFullScreenMode);
    }

    private void OnDestroy()
    {
        _resolutionDropdown.onValueChanged.RemoveListener(SetResolution);
        _fullScreenModeDropdown.onValueChanged.RemoveListener(SetFullScreenMode);
    }

    public override void SettingData()
    {
        _savedResolution = _currentResolution;
        _savedScreenMode = _currentScreenMode;
        StartCoroutine(ApplyResolutionCoroutine());
    }

    public override void ResetData()
    {
        if (_hasInitialized)
        {
            _currentResolution = _savedResolution;
            _currentScreenMode = _savedScreenMode;
        }
        else
        {
            List<Resolution> resolutions = GetResolutions();
            Resolution mainRes = resolutions.FirstOrDefault(r =>
                r.width == Display.main.systemWidth &&
                r.height == Display.main.systemHeight);

            _currentResolution = mainRes.width > 0 ? mainRes : resolutions.Last();
            _currentScreenMode = FullScreenMode.ExclusiveFullScreen;

            _savedResolution = _currentResolution;
            _savedScreenMode = _currentScreenMode;
            _hasInitialized = true;

            StartCoroutine(ApplyResolutionCoroutine());
        }

        _resolutionDropdown.onValueChanged.RemoveListener(SetResolution);
        _fullScreenModeDropdown.onValueChanged.RemoveListener(SetFullScreenMode);

        List<Resolution> currentResolutions = GetResolutions();
        int idx = currentResolutions.FindIndex(r =>
            r.width == _currentResolution.width &&
            r.height == _currentResolution.height);

        _resolutionDropdown.value = Mathf.Max(0, idx);
        _resolutionDropdown.RefreshShownValue();
        _fullScreenModeDropdown.value = (int)_currentScreenMode;
        _fullScreenModeDropdown.RefreshShownValue();

        _resolutionDropdown.onValueChanged.AddListener(SetResolution);
        _fullScreenModeDropdown.onValueChanged.AddListener(SetFullScreenMode);
    }

    private List<Resolution> GetResolutions()
    {
        return Screen.resolutions
            .GroupBy(r => (r.width, r.height))
            .Select(g => g.Last())
            .OrderBy(r => r.width)
            .ThenBy(r => r.height)
            .ToList();
    }

    private void SetResolution(int index)
    {
        string[] parts = _resolutionDropdown.options[index].text.Split('x');
        if (parts.Length != 2) return;

        _currentResolution.width = int.Parse(parts[0]);
        _currentResolution.height = int.Parse(parts[1]);
    }

    private void SetFullScreenMode(int index)
    {
        _currentScreenMode = (FullScreenMode)index;
    }

    private IEnumerator ApplyResolutionCoroutine()
    {
        Screen.SetResolution(_currentResolution.width, _currentResolution.height, _currentScreenMode);
        yield return new WaitForEndOfFrame();
    }

    #region Test

    [SerializeField] private GameObject parent;
    [ContextMenu("ShowUI")]
    private void ShowUI()
    {
        parent.SetActive(true);
    }

    #endregion
}