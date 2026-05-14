using MemberWorkspace.JJG._02_Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemTooltip : MonoSingleton<ItemTooltip>
{
    [SerializeField] private RectTransform _panel;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _typeText;
    [SerializeField] private TextMeshProUGUI _statsText;
    [SerializeField] private TextMeshProUGUI _descText;

    [SerializeField] private Vector2 _offset = new Vector2(16f, -16f);

    private RectTransform _canvasRT;
    private Canvas _canvas;
    private bool _isVisible;

    protected override void Awake()
    {
        base.Awake();

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[ItemTooltip] Canvas 부모를 찾을 수 없습니다. Canvas 아래에 배치하세요.");
            return;
        }

        _canvasRT = canvas.rootCanvas.GetComponent<RectTransform>();
        _canvas   = canvas.rootCanvas;

        if (_panel == null)
        {
            Debug.LogError("[ItemTooltip] _panel이 연결되지 않았습니다. Inspector를 확인하세요.");
            return;
        }

        _panel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_isVisible)
            FollowMouse();
    }

    public void Show(ItemData data)
    {
        if (data == null || _panel == null || _canvasRT == null) return;

        _nameText.text  = data.ItemName;
        _nameText.color = GetGradeColor(data.GradeType);
        _typeText.text  = GetTypeLabel(data);
        _statsText.text = GetStatsText(data);
        _descText.text  = data.description;

        _panel.gameObject.SetActive(true);
        transform.SetAsLastSibling();
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_panel);

        _isVisible = true;
        FollowMouse();
    }

    public void Hide()
    {
        _panel.gameObject.SetActive(false);
        _isVisible = false;
    }
    
    private void FollowMouse()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 mouseScreenPos = mouse.position.ReadValue();
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRT,
            mouseScreenPos,
            null,
            out Vector2 localPoint
        );

        _panel.anchoredPosition = localPoint + _offset;
    }

    private string GetTypeLabel(ItemData data)
    {
        return data switch
        {
            WeaponItemData   => "무기",
            FoodItemData     => "음식",
            _                => "소비 아이템"
        };
    }

    private string GetStatsText(ItemData data)
    {
        if (data is WeaponItemData weapon)
            return $"공격력: {weapon.Damage}\n탄약: {weapon.Ammo}\n장전 속도: {weapon.ReloadSpeed:F1}s";

        if (data is CountableItemData countable)
            return $"최대 수량: {countable.MaxAmount}";

        return string.Empty;
    }

    private Color GetGradeColor(GradeType grade)
    {
        return grade switch
        {
            GradeType.Common    => Color.white,
            GradeType.UnCommon  => new Color(0.118f, 1f, 0f),
            GradeType.Rare      => new Color(0f, 0.44f, 0.87f),
            GradeType.Epic      => new Color(0.64f, 0.21f, 0.93f),
            GradeType.Legendary => new Color(1f, 0.5f, 0f),
            _                   => Color.white
        };
    }
}
