using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PickMapUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform rectTrm;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textMesh;

    [Header("Size")]
    [SerializeField] private float defaultWidth = 700f;
    [SerializeField] private float defaultHeight = 100f;
    [SerializeField] private float hoverWidth = 750f;

    [Header("Animation")]
    [SerializeField] private float duration = 0.2f;

    private Color _defaultColor;
    private Vector2 _defaultSize;
    private Vector2 _hoverSize;
    private int _number;

    private GamePickTrans _pickTrans;

    private Tween _sizeTween;
    private Tween _colorTween;

    private void Awake()
    {
        _pickTrans = GetComponentInParent<GamePickTrans>();

        _defaultColor = image.color;

        _defaultSize = new Vector2(defaultWidth, defaultHeight);

        float scaleRatio = hoverWidth / defaultWidth;
        float hoverHeight = defaultHeight * scaleRatio;

        _hoverSize = new Vector2(hoverWidth, hoverHeight);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _sizeTween?.Kill();
        _colorTween?.Kill();

        _sizeTween = rectTrm
            .DOSizeDelta(_hoverSize, duration)
            .SetEase(Ease.OutQuad).SetUpdate(true); ;

        Color targetColor = _defaultColor * 1.2f;
        targetColor.a = _defaultColor.a;

        _colorTween = image
            .DOColor(targetColor, duration)
            .SetEase(Ease.OutQuad).SetUpdate(true); ;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _sizeTween?.Kill();
        _colorTween?.Kill();

        _sizeTween = rectTrm
            .DOSizeDelta(_defaultSize, duration)
            .SetEase(Ease.OutQuad).SetUpdate(true); ;

        _colorTween = image
            .DOColor(_defaultColor, duration)
            .SetEase(Ease.OutQuad).SetUpdate(true);
    }

    public void SetText(string name)
    {
        textMesh.text = $"º±≈√ : {name}";
    }

    public void GetNumber(int num)
    {
        _number = num;
    }
    public void SetTrans()
    {
        _pickTrans.SetSpawnTrans(_number);
    }
}
