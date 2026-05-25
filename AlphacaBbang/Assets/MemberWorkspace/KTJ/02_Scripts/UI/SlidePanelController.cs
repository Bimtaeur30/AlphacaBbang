using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(RectTransform))]
public class SlidePanelController : MonoBehaviour
{// SlidePanelController.cs 에 추가
    public bool IsHidden => isHidden;
    public enum SlideDirection { Left, Right, Up, Down }

    public event Action OnEndMoving;

    [Header("슬라이드 설정")]
    [SerializeField] private SlideDirection slideDirection = SlideDirection.Left;
    [SerializeField] private float slideDuration = 0.4f;
    [SerializeField] private Ease easeOut = Ease.InOutCubic;
    [SerializeField] private Ease easeIn = Ease.InOutCubic;
    [SerializeField] private bool isHidden = false;

    [Header("오프셋")]
    [Tooltip("화면 가장자리로부터 얼마나 떨어진 위치에 멈출지 거리")]
    [SerializeField] private float edgeOffset = 0f;

    [Header("고정 축 설정")]
    [SerializeField] private bool overrideFixedAxis = false;
    [SerializeField] private float fixedAxisOverrideValue = 0f;

    private RectTransform _rectTransform;
    private Canvas _rootCanvas;

    private float _visibleAxisValue;
    private float _fixedAxisValue;
    private float _hiddenAxisValue;

    private bool _initialized = false;
    private Tweener _currentTween;

    public bool IsXAxis => slideDirection == SlideDirection.Left || slideDirection == SlideDirection.Right;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rootCanvas = GetRootCanvas();
    }

    private IEnumerator Start()
    {
        yield return null;
        Initialize();
    }

    private void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        Vector2 initialPos = Vector2.zero;
        if(_rectTransform != null) //
            initialPos = _rectTransform.anchoredPosition;   

        _fixedAxisValue = overrideFixedAxis
            ? fixedAxisOverrideValue
            : (IsXAxis ? initialPos.y : initialPos.x);

        _visibleAxisValue = (IsXAxis ? initialPos.x : initialPos.y) + GetOffsetForDirection();

        CalculateHiddenAxisValue();
        if(_rectTransform != null) //
        _rectTransform.anchoredPosition = isHidden ? HiddenPosition : VisiblePosition;
    }

    /// <summary>
    /// Slide 직전마다 호출 — RectTransform의 Width/Height가 바뀌었을 수 있으므로
    /// Hidden/Fixed 위치 값을 재계산합니다.
    /// </summary>
    private void RefreshLayout()
    {
        // Fixed 축 갱신 (override 아닐 때만)
        if (!overrideFixedAxis)
        {
            Vector2 currentPos = Vector2.zero;
            if(_rectTransform != null) //
                currentPos = _rectTransform.anchoredPosition;
            _fixedAxisValue = IsXAxis ? currentPos.y : currentPos.x;
        }

        // Hidden 위치는 Width/Height에 의존하므로 항상 재계산
        CalculateHiddenAxisValue();
    }

    private float GetOffsetForDirection()
    {
        return slideDirection switch
        {
            SlideDirection.Left => -edgeOffset,
            SlideDirection.Right => edgeOffset,
            SlideDirection.Down => -edgeOffset,
            SlideDirection.Up => edgeOffset,
            _ => 0f
        };
    }

    private Canvas GetRootCanvas()
    {
        Canvas c = GetComponentInParent<Canvas>();
        while (c != null && !c.isRootCanvas)
            c = c.transform.parent?.GetComponentInParent<Canvas>();
        return c;
    }

    private void CalculateHiddenAxisValue()
    {
        Vector2 canvasSize = GetCanvasSize();
        Rect rect = new Rect();
        Vector2 anchor = Vector2.zero;
        Vector2 pivot = Vector2.zero;
        if (_rectTransform != null) //
        {
            rect = _rectTransform.rect;           // 매 호출마다 최신 Width/Height 반영
            anchor = _rectTransform.anchorMin;
            pivot = _rectTransform.pivot;
        }

        switch (slideDirection)
        {
            case SlideDirection.Left:
                _hiddenAxisValue = -(anchor.x * canvasSize.x)
                                   - (rect.width - pivot.x * rect.width) - 1f;
                break;
            case SlideDirection.Right:
                _hiddenAxisValue = canvasSize.x - anchor.x * canvasSize.x
                                   + pivot.x * rect.width + 1f;
                break;
            case SlideDirection.Down:
                _hiddenAxisValue = -(anchor.y * canvasSize.y)
                                   - (rect.height - pivot.y * rect.height) - 1f;
                break;
            case SlideDirection.Up:
                _hiddenAxisValue = canvasSize.y - anchor.y * canvasSize.y
                                   + pivot.y * rect.height + 1f;
                break;
        }
    }

    private Vector2 GetCanvasSize()
    {
        if (_rootCanvas == null) return new Vector2(Screen.width, Screen.height);
        CanvasScaler scaler = _rootCanvas.GetComponent<CanvasScaler>();
        RectTransform canvasRect = _rootCanvas.GetComponent<RectTransform>();

        if (scaler == null || _rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return canvasRect != null ? canvasRect.rect.size : new Vector2(Screen.width, Screen.height);

        return scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize
            ? scaler.referenceResolution
            : canvasRect != null ? canvasRect.rect.size : new Vector2(Screen.width, Screen.height);
    }

    public Vector2 VisiblePosition => IsXAxis
        ? new Vector2(_visibleAxisValue, _fixedAxisValue)
        : new Vector2(_fixedAxisValue, _visibleAxisValue);

    public Vector2 HiddenPosition => IsXAxis
        ? new Vector2(_hiddenAxisValue, _fixedAxisValue)
        : new Vector2(_fixedAxisValue, _hiddenAxisValue);

    // ── Public API ────────────────────────────────────────────────

    public void SlideOut()
    {
        if (!_initialized) Initialize();
        if (isHidden) return;
        isHidden = true;
        RefreshLayout();                  // Width/Height 재측정
        PlayTween(HiddenPosition, easeOut);
    }

    public void SlideIn()
    {
        if (!_initialized) Initialize();
        if (!isHidden) return;
        isHidden = false;
        RefreshLayout();                  // Width/Height 재측정
        PlayTween(VisiblePosition, easeIn);
    }

    public void Toggle() { if (isHidden) SlideIn(); else SlideOut(); }

    private void PlayTween(Vector2 target, Ease ease)
    {
        _currentTween?.Kill();
        if(_rectTransform != null)
        _currentTween = _rectTransform
            .DOAnchorPos(target, slideDuration)
            .SetEase(ease)
            .SetUpdate(true)
            .OnComplete(() => OnEndMoving?.Invoke());
    }

    private void OnDestroy() => _currentTween?.Kill();
}

// ── 커스텀 인스펙터 ────────────────────────────────────────────────
#if UNITY_EDITOR
[CustomEditor(typeof(SlidePanelController))]
public class SlidePanelControllerEditor : Editor
{
    SerializedProperty _overrideFixedAxis;
    SerializedProperty _fixedAxisOverrideValue;

    private void OnEnable()
    {
        _overrideFixedAxis = serializedObject.FindProperty("overrideFixedAxis");
        _fixedAxisOverrideValue = serializedObject.FindProperty("fixedAxisOverrideValue");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, "overrideFixedAxis", "fixedAxisOverrideValue");

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("고정 축 설정", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_overrideFixedAxis, new GUIContent("Override Fixed Axis"));

        EditorGUI.BeginDisabledGroup(!_overrideFixedAxis.boolValue);
        EditorGUILayout.PropertyField(_fixedAxisOverrideValue, new GUIContent("Fixed Axis Value"));
        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();

        if (Application.isPlaying)
        {
            var ctrl = (SlidePanelController)target;
            RectTransform rt = ctrl.GetComponent<RectTransform>();
            float hint = ctrl.IsXAxis ? rt.anchoredPosition.y : rt.anchoredPosition.x;
            string axis = ctrl.IsXAxis ? "Y" : "X";
            EditorGUILayout.HelpBox(
                $"현재 anchoredPosition: {rt.anchoredPosition}\n" +
                $"고정 축({axis})의 현재 값: {hint}",
                MessageType.Info);
        }

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("테스트 (플레이 모드 전용)", EditorStyles.boldLabel);

        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        EditorGUILayout.BeginHorizontal();
        var c = (SlidePanelController)target;
        if (GUILayout.Button("Slide Out")) c.SlideOut();
        if (GUILayout.Button("Slide In")) c.SlideIn();
        if (GUILayout.Button("Toggle")) c.Toggle();
        EditorGUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("플레이 모드에서만 테스트할 수 있습니다.", MessageType.Info);
    }
}
#endif