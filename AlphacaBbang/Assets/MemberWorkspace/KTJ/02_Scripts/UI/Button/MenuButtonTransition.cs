using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 메뉴 버튼 호버 트랜지션 컴포넌트
/// 마우스 오버 시 버튼 크기, 배경색, 아이콘 크기, 텍스트 폰트 크기, 레이아웃 간격이 변화합니다.
/// Inspector에서 값을 변경하면 에디터(비플레이 포함)에서도 즉시 Default 상태로 반영됩니다.
/// </summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(HorizontalLayoutGroup))]
[ExecuteAlways] // 에디터 비플레이 모드에서도 실행
public class MenuButtonTransition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("참조")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image arrowIcon;
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    [SerializeField] private RectTransform buttonRect;

    [Header("크기 설정")]
    [SerializeField] private Vector2 defaultSize = new Vector2(280f, 56f);
    [SerializeField] private Vector2 hoveredSize = new Vector2(320f, 72f);

    [Header("배경색 설정")]
    [SerializeField] private Color defaultColor = new Color(0.15f, 0.13f, 0.10f, 0.85f);
    [SerializeField] private Color hoveredColor = new Color(0.85f, 0.78f, 0.60f, 0.95f);

    [Header("아이콘 설정")]
    [SerializeField] private float defaultIconSize = 28f;
    [SerializeField] private float hoveredIconSize = 40f;

    [Header("텍스트 설정")]
    [SerializeField] private float defaultFontSize = 18f;
    [SerializeField] private float hoveredFontSize = 24f;
    [SerializeField] private Color defaultTextColor = new Color(0.75f, 0.72f, 0.65f, 1f);
    [SerializeField] private Color hoveredTextColor = new Color(0.12f, 0.10f, 0.08f, 1f);

    [Header("레이아웃 간격 설정")]
    [SerializeField] private float defaultSpacing = 10f;
    [SerializeField] private float hoveredSpacing = 18f;

    [Header("애니메이션 설정")]
    [SerializeField] private float tweenDuration = 0.25f;
    [SerializeField] private Ease enterEase = Ease.OutBack;
    [SerializeField] private Ease exitEase = Ease.InOutQuad;

    // 내부 상태
    private Sequence _currentSequence;
    private bool _isHovered = false;

    // 아이콘 RectTransform 캐시
    private RectTransform _iconRect;

    // ─────────────────────────────────────────────
    // 라이프사이클
    // ─────────────────────────────────────────────

    private void Awake()
    {
        CacheReferences();
        ApplyDefaultState();
    }

    private void OnDestroy()
    {
        _currentSequence?.Kill();
    }

    /// <summary>
    /// 참조가 없을 때 자동으로 수집합니다.
    /// </summary>
    private void CacheReferences()
    {
        if (buttonRect == null)
            buttonRect = GetComponent<RectTransform>();

        if (layoutGroup == null)
            layoutGroup = GetComponent<HorizontalLayoutGroup>();

        if (iconImage != null && _iconRect == null)
            _iconRect = iconImage.GetComponent<RectTransform>();
    }

    // ─────────────────────────────────────────────
    // 이벤트 핸들러 (플레이 모드 전용)
    // ─────────────────────────────────────────────

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Application.isPlaying) return;
        if (_isHovered) return;
        _isHovered = true;
        PlayTransition(toHovered: true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Application.isPlaying) return;
        if (!_isHovered) return;
        _isHovered = false;
        PlayTransition(toHovered: false);
    }

    // ─────────────────────────────────────────────
    // 트랜지션 실행 (플레이 모드)
    // ─────────────────────────────────────────────

    private void PlayTransition(bool toHovered)
    {
        _currentSequence?.Kill();
        _currentSequence = DOTween.Sequence();

        Ease ease = toHovered ? enterEase : exitEase;

        // 1) 버튼 전체 크기
        Vector2 targetSize = toHovered ? hoveredSize : defaultSize;
        _currentSequence.Join(
            buttonRect.DOSizeDelta(targetSize, tweenDuration).SetEase(ease)
        );

        // 2) 배경색
        if (backgroundImage != null)
        {
            Color targetBg = toHovered ? hoveredColor : defaultColor;
            _currentSequence.Join(
                backgroundImage.DOColor(targetBg, tweenDuration).SetEase(ease)
            );
        }

        // 3) 아이콘 크기
        if (_iconRect != null)
        {
            float targetIconSize = toHovered ? hoveredIconSize : defaultIconSize;
            _currentSequence.Join(
                _iconRect.DOSizeDelta(new Vector2(targetIconSize, targetIconSize), tweenDuration).SetEase(ease)
            );
        }

        // 4) 화살표 아이콘 페이드
        if (arrowIcon != null)
        {
            float fade = toHovered ? 1f : 0f;
            _currentSequence.Join(
                arrowIcon.DOFade(fade, tweenDuration).SetEase(ease)
            );
        }

        // 5) 텍스트 폰트 크기 & 색상
        if (labelText != null)
        {
            float targetFontSize = toHovered ? hoveredFontSize : defaultFontSize;
            Color targetTextColor = toHovered ? hoveredTextColor : defaultTextColor;

            _currentSequence.Join(
                DOTween.To(
                    () => labelText.fontSize,
                    v => labelText.fontSize = v,
                    targetFontSize,
                    tweenDuration
                ).SetEase(ease)
            );

            _currentSequence.Join(
                labelText.DOColor(targetTextColor, tweenDuration).SetEase(ease)
            );
        }

        // 6) HorizontalLayoutGroup spacing
        float targetSpacing = toHovered ? hoveredSpacing : defaultSpacing;
        _currentSequence.Join(
            DOTween.To(
                () => layoutGroup.spacing,
                v =>
                {
                    layoutGroup.spacing = v;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(buttonRect);
                },
                targetSpacing,
                tweenDuration
            ).SetEase(ease)
        );

        _currentSequence.SetUpdate(true); // TimeScale 무시
        _currentSequence.Play();
    }

    // ─────────────────────────────────────────────
    // Default 상태 즉시 적용
    // ─────────────────────────────────────────────

    /// <summary>
    /// 현재 Inspector 설정값을 기반으로 Default 상태를 UI에 즉시 반영합니다.
    /// 에디터 / 플레이 모드 모두 동작합니다.
    /// </summary>
    private void ApplyDefaultState()
    {
        CacheReferences();

        if (buttonRect != null)
            buttonRect.sizeDelta = defaultSize;

        if (backgroundImage != null)
            backgroundImage.color = defaultColor;

        if (iconImage != null)
        {
            if (_iconRect == null)
                _iconRect = iconImage.GetComponent<RectTransform>();
            _iconRect.sizeDelta = new Vector2(defaultIconSize, defaultIconSize);
        }

        if (arrowIcon != null)
        {
            Color c = arrowIcon.color;
            c.a = 0f; // 기본 상태: 화살표 숨김
            arrowIcon.color = c;
        }

        if (labelText != null)
        {
            labelText.fontSize = defaultFontSize;
            labelText.color = defaultTextColor;
        }

        if (layoutGroup != null)
        {
            layoutGroup.spacing = defaultSpacing;
            if (buttonRect != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(buttonRect);
        }
    }

    // ─────────────────────────────────────────────
    // 에디터 전용
    // ─────────────────────────────────────────────

#if UNITY_EDITOR
    /// <summary>
    /// Inspector에서 값을 변경할 때마다 자동으로 호출됩니다.
    /// [ExecuteAlways] 덕분에 비플레이 모드에서도 씬 뷰 / 게임 뷰에 즉시 반영됩니다.
    ///
    /// ※ OnValidate 내부에서 직접 UI를 수정하면
    ///   "SendMessage cannot be called during Awake/OnEnable" 경고가 발생할 수 있어
    ///   EditorApplication.delayCall로 한 프레임 뒤에 실행합니다.
    /// </summary>
#if UNITY_EDITOR
    private void OnValidate()
    {
        ApplyDefaultState();
    }
#endif
#endif
}