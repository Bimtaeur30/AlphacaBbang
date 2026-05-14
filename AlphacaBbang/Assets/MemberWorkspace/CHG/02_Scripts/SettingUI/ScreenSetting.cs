using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

//[RequireComponent(typeof(UIDocument))]
public class ScreenSetting : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private TMP_Dropdown _fullScreenModeDropdown;
    private Resolution _resolution;
    private FullScreenMode _screenMode;
    
    //private UIDocument _document;
    //private VisualElement _root;
    
    private void Awake()
    {
        //_document = GetComponent<UIDocument>();
       // _root = _document.rootVisualElement;
        _resolutionDropdown.options.Clear();
       foreach (Resolution resolution in GetResolutions())
           _resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolution.width + "x" + resolution.height));
       
       ResetContent();
       StartCoroutine(SetScreenCoroutine());
       
       _resolutionDropdown.onValueChanged.AddListener(SetResolution);
       _fullScreenModeDropdown.onValueChanged.AddListener(SetFullScreenMode);
    }
    
    public void OnEnable()
    {
        List<Resolution> resolutions = GetResolutions();

        int defaultIndex = resolutions.FindIndex(r => 
            r.width == _resolution.width && 
            r.height == _resolution.height);
    
        _resolutionDropdown.value = defaultIndex;
        _fullScreenModeDropdown.value = (int)_screenMode;
    }
    
    public void ResetContent()
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

    public void SetScreen()
    {
        StartCoroutine(SetScreenCoroutine());
    }
    
    public IEnumerator SetScreenCoroutine()
    {
        Screen.SetResolution(_resolution.width, _resolution.height, _screenMode);
        
        yield return null;
        
        Debug.Log(_resolution.width + "x" + _resolution.height);
        int deviceWidth = Screen.width;
        int deviceHeight = Screen.height;

        float targetAspect = (float)_resolution.width / _resolution.height;
        float deviceAspect = (float)deviceWidth / deviceHeight;

        Rect camRect = new Rect(0f, 0f, 1f, 1f);

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
        {
            cam.rect = camRect;
        }
        

        foreach (CanvasScaler canvasScaler in FindObjectsOfType<CanvasScaler>())
        {
            Debug.Log(canvasScaler.gameObject.name);
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(_resolution.width, _resolution.height);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;
        }
        
    
        /*if (_root != null)
        {
            _root.style.paddingLeft   = new StyleLength(camRect.x * deviceWidth);
            _root.style.paddingBottom = new StyleLength(camRect.y * deviceHeight);
            _root.style.paddingRight  = new StyleLength((1f - camRect.width - camRect.x) * deviceWidth);
            _root.style.paddingTop    = new StyleLength((1f - camRect.height - camRect.y) * deviceHeight);
        }*/
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