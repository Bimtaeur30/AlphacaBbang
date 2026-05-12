using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class ScreenSetting : MonoBehaviour
{
    private FullScreenMode _fullScreenMode = FullScreenMode.ExclusiveFullScreen;
    private Resolution _resolution;
 
    private UIDocument _document;
    private VisualElement _root;
    
    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _root = _document.rootVisualElement;
        
    }

    public void ChangeResolution(Resolution resolution, FullScreenMode fullScreenMode)
    {
        _resolution = resolution;
        _fullScreenMode = fullScreenMode;
        StartCoroutine(SetResolutionCoroutine());
    }

    [ContextMenu("SetResolution")]
    private void TestSet()
    {
        StartCoroutine(SetResolutionCoroutine());
    }

    private IEnumerator SetResolutionCoroutine()
    {
        Screen.SetResolution(_resolution.width, _resolution.height, _fullScreenMode);

        yield return new WaitForEndOfFrame(); 

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

        Debug.Log("resolution: " + _resolution);
        
        foreach (CanvasScaler canvasScaler in FindObjectsOfType<CanvasScaler>())
        {
            if (canvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(_resolution.width, _resolution.height); 
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                canvasScaler.matchWidthOrHeight = 0.5f;
            }
        }
    
        if (_root != null)
        {
            _root.style.paddingLeft   = new StyleLength(camRect.x * deviceWidth);
            _root.style.paddingBottom = new StyleLength(camRect.y * deviceHeight);
            _root.style.paddingRight  = new StyleLength((1f - camRect.width - camRect.x) * deviceWidth);
            _root.style.paddingTop    = new StyleLength((1f - camRect.height - camRect.y) * deviceHeight);
        }


    }
    
    
}