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
    private Resolution _resolution;
    private FullScreenMode _screenMode;
    
    
    public override void Awake()
    {
        _resolutionDropdown.options.Clear();
       foreach (Resolution resolution in GetResolutions())
           _resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolution.width + "x" + resolution.height));
       
       ResetData();
       StartCoroutine(SetScreenCoroutine());
       
       _resolutionDropdown.onValueChanged.AddListener(SetResolution);
       _fullScreenModeDropdown.onValueChanged.AddListener(SetFullScreenMode);
    }
    
    public override void OnEnable()
    {
        List<Resolution> resolutions = GetResolutions();

        int defaultIndex = resolutions.FindIndex(r => 
            r.width == _resolution.width && 
            r.height == _resolution.height);
    
        _resolutionDropdown.value = defaultIndex;
        _fullScreenModeDropdown.value = (int)_screenMode;
    }

    public override void SettingData()
    {
        StartCoroutine(SetScreenCoroutine());
    }

    public override void ResetData()
    {
        _resolution = Screen.currentResolution;
        _screenMode = FullScreenMode.ExclusiveFullScreen;
    
        List<Resolution> resolutions = GetResolutions();
    
        
        int defaultIndex = resolutions.FindIndex(r => 
            r.width == Display.main.systemWidth && 
            r.height == Display.main.systemHeight);
    
        _resolutionDropdown.value = defaultIndex;
        _fullScreenModeDropdown.value = 0;
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
        string[] dropDownFieldName = _resolutionDropdown.options[index].text.Split('x');

        _resolution.width = int.Parse(dropDownFieldName[0]);
        _resolution.height = int.Parse(dropDownFieldName[1]);
    }

    private void SetFullScreenMode(int index)
    {
        _screenMode = (FullScreenMode)index;
    }
    
    private IEnumerator SetScreenCoroutine()
    {
        Screen.SetResolution(_resolution.width, _resolution.height, _screenMode);
        yield return null;

        float targetAspect = (float)_resolution.width / _resolution.height;
        float deviceAspect = (float)Screen.width / Screen.height;

        Rect camRect;
        if (deviceAspect < targetAspect)
        {
            float newHeight = deviceAspect / targetAspect;
            camRect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
        }
        else
        {
            float newWidth = targetAspect / deviceAspect;
            camRect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
        }

        foreach (Camera cam in Camera.allCameras)
            cam.rect = camRect;

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.transform.parent != null) continue;
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay) continue;

            Transform safeArea = canvas.transform.Find("SafeArea");
            if (safeArea == null) continue;

            RectTransform rt = safeArea.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(camRect.x, camRect.y);
            rt.anchorMax = new Vector2(camRect.x + camRect.width, camRect.y + camRect.height);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
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