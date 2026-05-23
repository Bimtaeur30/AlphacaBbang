using System.Collections.Generic;
using TMPro;
using MemberWorkspace.JJG._02_Scripts.Item.Data;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class InventoryUI : MonoBehaviour
{
    [Header("Options")]
    [Range(1, 20)]
    [SerializeField] private int _horizontalSlotCount = 8;
    
    [Range(1, 30)]
    [SerializeField] private int _verticalSlotCount = 8;

    [SerializeField] private float _slotMargin = 8f;
    [SerializeField] private float _contentAreaPadding = 20f;

    [Range(32, 160)]
    [SerializeField] private float _slotSize = 64f;

    [Header("Connected Objects")]
    [SerializeField] private RectTransform _contentAreaRT;
    [SerializeField] private GameObject _slotUIPrefab;
    [SerializeField] private ItemContainer inventory;
    [SerializeField] private WeaponHolder _weaponHolder;
    
    [Header("Inventory Only")]
    [SerializeField] private TextMeshProUGUI _inventoryStatusText;

    [Header("Preview")]
    [SerializeField] private bool _showPreview = false;

    private readonly List<ItemSlotUI> _slotUIList = new();
    private int _selectedSlotIndex = -1;

    private int SlotCount
    {
        get
        {
            return _horizontalSlotCount * _verticalSlotCount;
        }
    }

    private void Awake()
    {
        if (inventory == null)
            inventory = GetComponent<ItemContainer>();
    }

    public void SetInventory(ItemContainer container)
    {
        if (container == null)
            return;

        inventory = container;
        RefreshUI();
    }

    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            if (inventory != null)
                inventory.OnContainerChanged += RefreshUI;

            if (_weaponHolder != null)
            {
                _weaponHolder.OnWeaponChanged += OnWeaponChanged;
                _weaponHolder.OnThrowingItemChanged += OnThrowingItemChanged;
            }

            RebuildRuntime();
        }
#if UNITY_EDITOR
        else
        {
            SchedulePreviewRebuild();
        }
#endif
    }

    private void OnDisable()
    {
        if (Application.isPlaying)
        {
            if (inventory != null)
                inventory.OnContainerChanged -= RefreshUI;

            if (_weaponHolder != null)
            {
                _weaponHolder.OnWeaponChanged -= OnWeaponChanged;
                _weaponHolder.OnThrowingItemChanged -= OnThrowingItemChanged;
            }
        }
    }

    private void OnWeaponChanged(WeaponItemData weaponData)
    {
        _selectedSlotIndex = weaponData != null ? _weaponHolder.CurrentSlotIndex : -1;
        UpdateSelectionUI();
        Debug.Log("Changed");
    }

    private void OnThrowingItemChanged(ThrowingItemData throwingData)
    {
        _selectedSlotIndex = throwingData != null ? _weaponHolder.CurrentThrowingSlotIndex : -1;
        UpdateSelectionUI();
    }

    private void UpdateSelectionUI()
    {
        for (int i = 0; i < _slotUIList.Count; i++)
            _slotUIList[i].SetSelected(i == _selectedSlotIndex);
    }

    private void Start()
    {
        if (!Application.isPlaying)
            return;

        RebuildRuntime();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying)
            return;

        SchedulePreviewRebuild();
    }

    private void SchedulePreviewRebuild()
    {
        EditorApplication.delayCall -= RebuildPreview;
        EditorApplication.delayCall += RebuildPreview;
    }

    private void RebuildPreview()
    {
        if (this == null)
            return;

        if (_contentAreaRT == null || _slotUIPrefab == null)
            return;

        ClearGeneratedSlots();

        if (!_showPreview)
            return;

        GenerateSlots(isPreview: true);
    }
#endif

    [ContextMenu("Rebuild Slots")]
    private void RebuildRuntime()
    {
        if (_contentAreaRT == null || _slotUIPrefab == null)
            return;

        ClearGeneratedSlots();
        GenerateSlots(isPreview: false);
        RefreshUI();
    }

    private void GenerateSlots(bool isPreview)
    {
        _slotUIList.Clear();

        ApplyGridLayout();

        int slotCount = SlotCount;

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotGo = CreateSlotObject();
            slotGo.name = isPreview ? $"Preview Slot [{i}]" : $"Item Slot [{i}]";
            slotGo.SetActive(true);

            if (slotGo.GetComponent<GeneratedInventorySlot>() == null)
                slotGo.AddComponent<GeneratedInventorySlot>();

            RectTransform slotRT = slotGo.GetComponent<RectTransform>();
            if (slotRT != null)
            {
                slotRT.SetParent(_contentAreaRT, false);
                slotRT.localScale = Vector3.one;
                slotRT.anchorMin = new Vector2(0.5f, 0.5f);
                slotRT.anchorMax = new Vector2(0.5f, 0.5f);
                slotRT.pivot = new Vector2(0.5f, 0.5f);
                slotRT.sizeDelta = new Vector2(_slotSize, _slotSize);
            }

            LayoutElement layoutElement = slotGo.GetComponent<LayoutElement>();
            if (layoutElement == null)
                layoutElement = slotGo.AddComponent<LayoutElement>();

            layoutElement.preferredWidth = _slotSize;
            layoutElement.preferredHeight = _slotSize;
            layoutElement.minWidth = _slotSize;
            layoutElement.minHeight = _slotSize;

            ItemSlotUI slotUI = slotGo.GetComponent<ItemSlotUI>();
            if (slotUI == null)
                slotUI = slotGo.AddComponent<ItemSlotUI>();

            slotUI.Initialize(inventory, i);
            _slotUIList.Add(slotUI);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_contentAreaRT);
    }

    private void ApplyGridLayout()
    {
        GridLayoutGroup grid = _contentAreaRT.GetComponent<GridLayoutGroup>();
        if (grid == null)
            grid = _contentAreaRT.gameObject.AddComponent<GridLayoutGroup>();

        grid.cellSize = new Vector2(_slotSize, _slotSize);
        grid.spacing = new Vector2(_slotMargin, _slotMargin);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = _horizontalSlotCount;
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.UpperLeft;

        int padding = Mathf.RoundToInt(_contentAreaPadding);
        grid.padding = new RectOffset(padding, padding, padding, padding);

        ContentSizeFitter fitter = _contentAreaRT.GetComponent<ContentSizeFitter>();
        if (fitter == null)
            fitter = _contentAreaRT.gameObject.AddComponent<ContentSizeFitter>();

        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        _contentAreaRT.localScale = Vector3.one;
    }

    private GameObject CreateSlotObject()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            GameObject prefabInstance =
                PrefabUtility.InstantiatePrefab(_slotUIPrefab, _contentAreaRT) as GameObject;

            if (prefabInstance != null)
                return prefabInstance;
        }
#endif

        return Instantiate(_slotUIPrefab, _contentAreaRT, false);
    }

    private void RefreshUI()
    {
        if (inventory == null)
            return;

        if (_slotUIList.Count != SlotCount)
            RebuildRuntime();

        for (int i = 0; i < _slotUIList.Count; i++)
        {
            ItemSlot slot = inventory.GetSlot(i);

            if (slot != null && !slot.IsEmpty)
                _slotUIList[i].SetSlot(slot);
            else
                _slotUIList[i].ClearSlot();
        }

        UpdateInventoryStatus();
    }

    private void UpdateInventoryStatus()
    {
        if (_inventoryStatusText == null || inventory == null)
            return;

        int occupiedCount = 0;
        for (int i = 0; i < inventory.SlotCount; i++)
        {
            ItemSlot slot = inventory.GetSlot(i);
            if (slot != null && !slot.IsEmpty)
                occupiedCount++;
        }

        _inventoryStatusText.text = $"{occupiedCount}/{inventory.SlotCount}";
    }

    public void PrepareForReveal()
    {
        foreach (var slotUI in _slotUIList)
            slotUI.ClearSlot();
    }

    public void RevealSlot(int index)
    {
        if (index < 0 || index >= _slotUIList.Count || inventory == null)
            return;

        ItemSlot slot = inventory.GetSlot(index);
        if (slot != null && !slot.IsEmpty)
            _slotUIList[index].SetSlot(slot);
    }

    private void ClearGeneratedSlots()
    {
        if (_contentAreaRT == null)
            return;

        for (int i = _contentAreaRT.childCount - 1; i >= 0; i--)
        {
            Transform child = _contentAreaRT.GetChild(i);

            // Content 밑의 슬롯 UI는 전부 제거
            if (child.GetComponent<ItemSlotUI>() == null)
                continue;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(child.gameObject);
            else
                Destroy(child.gameObject);
#else
        Destroy(child.gameObject);
#endif
        }

        _slotUIList.Clear();
    }

    private class GeneratedInventorySlot : MonoBehaviour
    {
    }
}