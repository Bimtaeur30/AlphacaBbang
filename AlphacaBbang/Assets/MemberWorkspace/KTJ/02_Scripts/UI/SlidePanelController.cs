using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(RectTransform))]
public class SlidePanelController : MonoBehaviour
{
    public enum SlideDirection { Left, Right, Up, Down }

    [Header("슬라이드 설정")]
    [SerializeField] private SlideDirection slideDirection = SlideDirection.Left;
    [SerializeField] private float slideDuration = 0.4f;
    [SerializeField] private Ease easeOut = Ease.InOutCubic;
    [SerializeField] private Ease easeIn = Ease.InOutCubic;

    [Header("오프셋")]
    [Tooltip("화면 테두리로부터 들어올 때의 멈춤 거리 (양수 = 테두리에서 더 안쪽에 멈춤, 음수 = 테두리 밖에 걸치게)")]
    [SerializeField] private float edgeOffset = 0f;

    [Header("고정 축 설정")]
    [Tooltip("체크하면 고정 축 값을 아래 수동 입력값으로 사용. 체크 해제 시 런타임에 자동으로 읽음")]
    [SerializeField] private bool overrideFixedAxis = false;
    [Tooltip("X축 이동 시 Y 고정값 / Y축 이동 시 X 고정값 (overrideFixedAxis 체크 시 사용)")]
    [SerializeField] private float fixedAxisOverrideValue = 0f;

    private RectTransform _rectTransform;
    private Canvas _rootCanvas;

    private float _visibleAxisValue;
    private float _fixedAxisValue;
    private float _hiddenAxisValue;

    private bool _isHidden = false;
    private bool _initialized = false;
    private Tweener _currentTween;

    // 에디터에서 참조할 수 있도록 public으로 노출
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

        Vector2 initialPos = _rectTransform.anchoredPosition;

        _fixedAxisValue = overrideFixedAxis
            ? fixedAxisOverrideValue
            : (IsXAxis ? initialPos.y : initialPos.x);

        _visibleAxisValue = (IsXAxis ? initialPos.x : initialPos.y) + GetOffsetForDirection();

        _rectTransform.anchoredPosition = VisiblePosition;
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
        Rect rect = _rectTransform.rect;
        Vector2 anchor = _rectTransform.anchorMin;
        Vector2 pivot = _rectTransform.pivot;

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

    private Vector2 VisiblePosition => IsXAxis
        ? new Vector2(_visibleAxisValue, _fixedAxisValue)
        : new Vector2(_fixedAxisValue, _visibleAxisValue);

    private Vector2 HiddenPosition => IsXAxis
        ? new Vector2(_hiddenAxisValue, _fixedAxisValue)
        : new Vector2(_fixedAxisValue, _hiddenAxisValue);

    // ── Public API ──────────────────────────────────

    public void SlideOut()
    {
        if (!_initialized) Initialize();
        if (_isHidden) return;
        _isHidden = true;
        CalculateHiddenAxisValue();
        PlayTween(HiddenPosition, easeOut);
    }

    public void SlideIn()
    {
        if (!_initialized) Initialize();
        if (!_isHidden) return;
        _isHidden = false;
        PlayTween(VisiblePosition, easeIn);
    }

    public void Toggle() { if (_isHidden) SlideIn(); else SlideOut(); }

    private void PlayTween(Vector2 target, Ease ease)
    {
        _currentTween?.Kill();
        _currentTween = _rectTransform
            .DOAnchorPos(target, slideDuration)
            .SetEase(ease)
            .SetUpdate(true);
    }

    private void OnDestroy() => _currentTween?.Kill();
}

// ── 커스텀 인스펙터 ──────────────────────────────────
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

        // 고정 축 설정 필드 제외하고 기본 인스펙터 그리기
        DrawPropertiesExcluding(serializedObject, "overrideFixedAxis", "fixedAxisOverrideValue");

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("고정 축 설정", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_overrideFixedAxis, new GUIContent("Override Fixed Axis"));

        EditorGUI.BeginDisabledGroup(!_overrideFixedAxis.boolValue);
        EditorGUILayout.PropertyField(_fixedAxisOverrideValue, new GUIContent("Fixed Axis Value"));
        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();

        // 플레이 중 현재 값 힌트
        if (Application.isPlaying)
        {
            var ctrl = (SlidePanelController)target;
            RectTransform rt = ctrl.GetComponent<RectTransform>();
            float hint = ctrl.IsXAxis ? rt.anchoredPosition.y : rt.anchoredPosition.x;
            string axis = ctrl.IsXAxis ? "Y" : "X";
            EditorGUILayout.HelpBox(
                $"현재 anchoredPosition: {rt.anchoredPosition}\n" +
                $"고정 축({axis})에 넣을 값: {hint}",
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