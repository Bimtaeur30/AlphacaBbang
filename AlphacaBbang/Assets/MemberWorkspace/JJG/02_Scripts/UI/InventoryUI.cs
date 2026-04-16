using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class InventoryUI : MonoBehaviour
{
    [Header("Options")]
    [Range(1, 10)]
    [SerializeField] private int _horizontalSlotCount = 8;
    [Range(1, 20)]
    [SerializeField] private int _verticalSlotCount = 8;
    [SerializeField] private float _slotMargin = 8f;
    [SerializeField] private float _contentAreaPadding = 20f;
    [Range(32, 100)]
    [SerializeField] private float _slotSize = 64f;

    [Header("Connected Objects")]
    [SerializeField] private RectTransform _contentAreaRT;
    [SerializeField] private GameObject _slotUIPrefab;
    [SerializeField] private ItemContainer inventory;

    [Header("Preview")]
    [SerializeField] private bool _showPreview = false;

    [Range(0.01f, 1f)]
    [SerializeField] private float _previewAlpha = 0.1f;

    private List<ItemSlotUI> _slotUIList = new();
    private List<GameObject> _previewSlotGoList = new();

    private int _prevSlotCountPerLine;
    private int _prevSlotLineCount;
    private float _prevSlotSize;
    private float _prevSlotMargin;
    private float _prevContentPadding;
    private float _prevAlpha;
    private bool _prevShow = false;

#if UNITY_EDITOR
    private bool _isRefreshingPreview;
#endif

    private void Awake()
    {
        if (inventory == null)
            inventory = GetComponent<ItemContainer>();
    }

    private void Start()
    {
        InitSlots();
        RefreshUI();
    }

    private void OnEnable()
    {
        if (inventory != null)
            inventory.OnContainerChanged += RefreshUI;
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnContainerChanged -= RefreshUI;
    }

    private void RefreshUI()
    {
        if (inventory == null)
            return;

        for (int i = 0; i < _slotUIList.Count; i++)
        {
            ItemSlot slot = inventory.GetSlot(i);

            if (slot != null && !slot.IsEmpty)
            {
                _slotUIList[i].SetSlot(slot);
            }
            else
            {
                _slotUIList[i].ClearSlot();
            }
        }
    }

    [ContextMenu("Initialize Slots")]
    private void InitSlots()
    {
        if (_contentAreaRT == null || _slotUIPrefab == null)
            return;

        ClearRuntimeSlots();

        Vector2 beginPos = new Vector2(_contentAreaPadding, -_contentAreaPadding);
        Vector2 curPos = beginPos;

        _slotUIList = new List<ItemSlotUI>(_verticalSlotCount * _horizontalSlotCount);

        for (int j = 0; j < _verticalSlotCount; j++)
        {
            for (int i = 0; i < _horizontalSlotCount; i++)
            {
                int slotIndex = (_horizontalSlotCount * j) + i;

                RectTransform slotRT = CloneSlot();
                if (slotRT == null)
                    continue;

                slotRT.pivot = new Vector2(0f, 1f);
                slotRT.anchorMin = new Vector2(0f, 1f);
                slotRT.anchorMax = new Vector2(0f, 1f);
                slotRT.anchoredPosition = curPos;
                slotRT.sizeDelta = new Vector2(_slotSize, _slotSize);

                slotRT.gameObject.SetActive(true);
                slotRT.gameObject.name = $"Item Slot [{slotIndex}]";

                ItemSlotUI slotUI = slotRT.GetComponent<ItemSlotUI>();
                if (slotUI == null)
                    slotUI = slotRT.gameObject.AddComponent<ItemSlotUI>();

                slotUI.SetSlotIndex(slotIndex);
                _slotUIList.Add(slotUI);

                curPos.x += (_slotMargin + _slotSize);
            }

            curPos.x = beginPos.x;
            curPos.y -= (_slotMargin + _slotSize);
        }

        RectTransform CloneSlot()
        {
            GameObject slotGo = Instantiate(_slotUIPrefab, _contentAreaRT, false);
            return slotGo.GetComponent<RectTransform>();
        }
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
            return;

        if (_isRefreshingPreview)
            return;

        _isRefreshingPreview = true;
#endif

        try
        {
            if (Unavailable())
            {
                ClearPreviewSlots();
                ClearRuntimeSlots();
                CachePreviewValues();
                return;
            }

            bool showChanged = _showPreview != _prevShow;
            bool countChanged = CountChanged();
            bool valueChanged = ValueChanged();
            bool alphaChanged = AlphaChanged();

            if (showChanged)
            {
                ClearPreviewSlots();

                if (_showPreview)
                    CreateSlots();

                _prevShow = _showPreview;
            }
            else
            {
                if (_showPreview)
                {
                    if (countChanged)
                    {
                        ClearPreviewSlots();
                        CreateSlots();
                    }
                    else if (valueChanged)
                    {
                        DrawGrid();
                    }

                    if (alphaChanged)
                    {
                        SetImageAlpha();
                    }
                }
            }

            CachePreviewValues();
        }
        finally
        {
#if UNITY_EDITOR
            _isRefreshingPreview = false;
#endif
        }
    }

    private bool Unavailable()
    {
        return _horizontalSlotCount < 1 ||
               _verticalSlotCount < 1 ||
               _slotSize <= 0f ||
               _contentAreaRT == null ||
               _slotUIPrefab == null;
    }

    private bool CountChanged()
    {
        return _horizontalSlotCount != _prevSlotCountPerLine ||
               _verticalSlotCount != _prevSlotLineCount;
    }

    private bool ValueChanged()
    {
        return _slotSize != _prevSlotSize ||
               _slotMargin != _prevSlotMargin ||
               _contentAreaPadding != _prevContentPadding;
    }

    private bool AlphaChanged()
    {
        return _previewAlpha != _prevAlpha;
    }

    private void CachePreviewValues()
    {
        _prevShow = _showPreview;
        _prevSlotCountPerLine = _horizontalSlotCount;
        _prevSlotLineCount = _verticalSlotCount;
        _prevSlotSize = _slotSize;
        _prevSlotMargin = _slotMargin;
        _prevContentPadding = _contentAreaPadding;
        _prevAlpha = _previewAlpha;
    }

    private void ClearRuntimeSlots()
    {
        _slotUIList.Clear();

        if (_contentAreaRT == null)
            return;

        for (int i = _contentAreaRT.childCount - 1; i >= 0; i--)
        {
            Transform child = _contentAreaRT.GetChild(i);
            if (child.GetComponent<PreviewItemSlot>() != null)
                continue;

#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(child.gameObject);
            else Destroy(child.gameObject);
#else
            Destroy(child.gameObject);
#endif
        }
    }

    private void ClearPreviewSlots()
    {
        foreach (var go in _previewSlotGoList)
        {
            if (go == null) continue;
            Destroyer.Destroy(go);
        }

        _previewSlotGoList.Clear();
    }

    private void CreateSlots()
    {
        int count = _horizontalSlotCount * _verticalSlotCount;
        _previewSlotGoList = new List<GameObject>(count);

        for (int i = 0; i < count; i++)
        {
            GameObject slotGo = Instantiate(_slotUIPrefab, _contentAreaRT, false);
            slotGo.SetActive(true);
            slotGo.name = $"Preview Slot [{i}]";

            if (slotGo.GetComponent<PreviewItemSlot>() == null)
                slotGo.AddComponent<PreviewItemSlot>();

            RectTransform slotRT = slotGo.GetComponent<RectTransform>();
            if (slotRT != null)
            {
                slotRT.pivot = new Vector2(0f, 1f);
                slotRT.anchorMin = new Vector2(0f, 1f);
                slotRT.anchorMax = new Vector2(0f, 1f);
                slotRT.localScale = Vector3.one;
                slotRT.sizeDelta = new Vector2(_slotSize, _slotSize);
            }

            HideGameObject(slotGo);
            _previewSlotGoList.Add(slotGo);
        }

        DrawGrid();
        SetImageAlpha();
    }

    private void DrawGrid()
    {
        Vector2 beginPos = new Vector2(_contentAreaPadding, -_contentAreaPadding);
        Vector2 curPos = beginPos;

        int index = 0;
        for (int j = 0; j < _verticalSlotCount; j++)
        {
            for (int i = 0; i < _horizontalSlotCount; i++)
            {
                if (index >= _previewSlotGoList.Count)
                    return;

                GameObject slotGo = _previewSlotGoList[index++];
                RectTransform slotRT = slotGo.GetComponent<RectTransform>();
                if (slotRT == null)
                    continue;

                slotRT.anchoredPosition = curPos;
                slotRT.sizeDelta = new Vector2(_slotSize, _slotSize);

                curPos.x += (_slotMargin + _slotSize);
            }

            curPos.x = beginPos.x;
            curPos.y -= (_slotMargin + _slotSize);
        }
    }

    private void HideGameObject(GameObject go)
    {
        go.hideFlags = HideFlags.HideAndDontSave;

        Transform tr = go.transform;
        for (int i = 0; i < tr.childCount; i++)
        {
            tr.GetChild(i).gameObject.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    private void SetImageAlpha()
    {
        foreach (var go in _previewSlotGoList)
        {
            if (go == null) continue;

            var images = go.GetComponentsInChildren<Image>(true);
            foreach (var img in images)
            {
                Color c = img.color;
                c.a = _previewAlpha;
                img.color = c;

                var outline = img.GetComponent<Outline>();
                if (outline != null)
                {
                    Color oc = outline.effectColor;
                    oc.a = _previewAlpha;
                    outline.effectColor = oc;
                }
            }
        }
    }

    private class PreviewItemSlot : MonoBehaviour { }

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
    private static class Destroyer
    {
        private static readonly Queue<GameObject> targetQueue = new Queue<GameObject>();

        static Destroyer()
        {
            UnityEditor.EditorApplication.update += () =>
            {
                for (int i = 0; targetQueue.Count > 0 && i < 100000; i++)
                {
                    var next = targetQueue.Dequeue();
                    if (next != null)
                        DestroyImmediate(next);
                }
            };
        }

        public static void Destroy(GameObject go)
        {
            if (go != null)
                targetQueue.Enqueue(go);
        }
    }
#endif
}