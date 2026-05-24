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
{
    public enum SlideDirection { Left, Right, Up, Down }

    public event Action OnEndMoving;

    [Header("�����̵� ����")]
    [SerializeField] private SlideDirection slideDirection = SlideDirection.Left;
    [SerializeField] private float slideDuration = 0.4f;
    [SerializeField] private Ease easeOut = Ease.InOutCubic;
    [SerializeField] private Ease easeIn = Ease.InOutCubic;
    [SerializeField] private bool isHidden = false;

    [Header("������")]
    [Tooltip("ȭ�� �׵θ��κ��� ���� ���� ���� �Ÿ� (��� = �׵θ����� �� ���ʿ� ����, ���� = �׵θ� �ۿ� ��ġ��)")]
    [SerializeField] private float edgeOffset = 0f;

    [Header("���� �� ����")]
    [Tooltip("üũ�ϸ� ���� �� ���� �Ʒ� ���� �Է°����� ���. üũ ���� �� ��Ÿ�ӿ� �ڵ����� ����")]
    [SerializeField] private bool overrideFixedAxis = false;
    [Tooltip("X�� �̵� �� Y ������ / Y�� �̵� �� X ������ (overrideFixedAxis üũ �� ���)")]
    [SerializeField] private float fixedAxisOverrideValue = 0f;

    private RectTransform _rectTransform;
    private Canvas _rootCanvas;

    private float _visibleAxisValue;
    private float _fixedAxisValue;
    private float _hiddenAxisValue;

    private bool _initialized = false;
    private Tweener _currentTween;

    // �����Ϳ��� ������ �� �ֵ��� public���� ����
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

        CalculateHiddenAxisValue();
        _rectTransform.anchoredPosition = isHidden ? HiddenPosition : VisiblePosition;
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

    public Vector2 VisiblePosition => IsXAxis
        ? new Vector2(_visibleAxisValue, _fixedAxisValue)
        : new Vector2(_fixedAxisValue, _visibleAxisValue);

    public Vector2 HiddenPosition => IsXAxis
        ? new Vector2(_hiddenAxisValue, _fixedAxisValue)
        : new Vector2(_fixedAxisValue, _hiddenAxisValue);

    // ���� Public API ��������������������������������������������������������������������

    public void SlideOut()
    {
        if (!_initialized) Initialize();
        if (isHidden) return;
        isHidden = true;
        CalculateHiddenAxisValue();
        PlayTween(HiddenPosition, easeOut);
    }

    public void SlideIn()
    {
        if (!_initialized) Initialize();
        if (!isHidden) return;
        isHidden = false;
        PlayTween(VisiblePosition, easeIn);
    }

    public void Toggle() { if (isHidden) SlideIn(); else SlideOut(); }

    private void PlayTween(Vector2 target, Ease ease)
    {
        _currentTween?.Kill();
        _currentTween = _rectTransform
            .DOAnchorPos(target, slideDuration)
            .SetEase(ease)
            .SetUpdate(true)
            .OnComplete(() => OnEndMoving?.Invoke());
    }

    private void OnDestroy() => _currentTween?.Kill();
}

// ���� Ŀ���� �ν����� ��������������������������������������������������������������������
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

        // ���� �� ���� �ʵ� �����ϰ� �⺻ �ν����� �׸���
        DrawPropertiesExcluding(serializedObject, "overrideFixedAxis", "fixedAxisOverrideValue");

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("���� �� ����", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_overrideFixedAxis, new GUIContent("Override Fixed Axis"));

        EditorGUI.BeginDisabledGroup(!_overrideFixedAxis.boolValue);
        EditorGUILayout.PropertyField(_fixedAxisOverrideValue, new GUIContent("Fixed Axis Value"));
        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();

        // �÷��� �� ���� �� ��Ʈ
        if (Application.isPlaying)
        {
            var ctrl = (SlidePanelController)target;
            RectTransform rt = ctrl.GetComponent<RectTransform>();
            float hint = ctrl.IsXAxis ? rt.anchoredPosition.y : rt.anchoredPosition.x;
            string axis = ctrl.IsXAxis ? "Y" : "X";
            EditorGUILayout.HelpBox(
                $"���� anchoredPosition: {rt.anchoredPosition}\n" +
                $"���� ��({axis})�� ���� ��: {hint}",
                MessageType.Info);
        }

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("�׽�Ʈ (�÷��� ��� ����)", EditorStyles.boldLabel);

        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        EditorGUILayout.BeginHorizontal();
        var c = (SlidePanelController)target;
        if (GUILayout.Button("Slide Out")) c.SlideOut();
        if (GUILayout.Button("Slide In")) c.SlideIn();
        if (GUILayout.Button("Toggle")) c.Toggle();
        EditorGUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("�÷��� ��忡���� �׽�Ʈ�� �� �ֽ��ϴ�.", MessageType.Info);
    }
}
#endif