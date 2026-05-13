#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class LootBoxRevealUIPrefabCreator
{
    private const string SavePath        = "Assets/MemberWorkspace/JJG/03_GameModule/Prefabs/LootBoxRevealUI.prefab";
    private const string ItemSlotPath    = "Assets/MemberWorkspace/JJG/03_GameModule/Prefabs/ItemSlot.prefab";

    [MenuItem("JJG/Create LootBoxRevealUI Prefab")]
    public static void Create()
    {
        GameObject itemSlotPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ItemSlotPath);
        if (itemSlotPrefab == null)
        {
            Debug.LogError($"ItemSlot 프리팹을 찾을 수 없습니다: {ItemSlotPath}");
            return;
        }

        // ── 최상위 ───────────────────────────────────────────
        GameObject go = new GameObject("LootBoxRevealUI");
        go.AddComponent<RectTransform>();
        LootBoxRevealUI reveal = go.AddComponent<LootBoxRevealUI>();

        // ── Root 패널 ─────────────────────────────────────────
        GameObject root = MakePanel("Root", go.transform, new Color(0.08f, 0.08f, 0.1f, 0.92f));
        Stretch(root);

        // ── BufferingPanel ────────────────────────────────────
        GameObject bufPanel = MakePanel("BufferingPanel", root.transform, new Color(0, 0, 0, 0));
        Stretch(bufPanel);

        // 로딩 텍스트
        GameObject loadTxtGO = MakeTMP("LoadingText", bufPanel.transform, "로딩 중...", 26, Color.white);
        RectTransform loadRT = loadTxtGO.GetComponent<RectTransform>();
        loadRT.anchorMin = new Vector2(0.5f, 0.5f);
        loadRT.anchorMax = new Vector2(0.5f, 0.5f);
        loadRT.sizeDelta = new Vector2(260f, 44f);
        loadRT.anchoredPosition = new Vector2(0f, 60f);
        loadTxtGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

        // 스피너
        GameObject spinner = new GameObject("Spinner");
        spinner.transform.SetParent(bufPanel.transform, false);
        spinner.AddComponent<CanvasRenderer>();
        Image spinImg = spinner.AddComponent<Image>();
        spinImg.color = new Color(0.95f, 0.75f, 0.1f, 1f);
        RectTransform spinRT = spinner.GetComponent<RectTransform>();
        spinRT.anchorMin = new Vector2(0.5f, 0.5f);
        spinRT.anchorMax = new Vector2(0.5f, 0.5f);
        spinRT.sizeDelta = new Vector2(72f, 72f);
        spinRT.anchoredPosition = Vector2.zero;

        // ── ContentArea (InventoryUI) ─────────────────────────
        GameObject contentArea = MakePanel("ContentArea", root.transform, new Color(0.15f, 0.15f, 0.18f, 0.95f));
        Stretch(contentArea);
        contentArea.SetActive(false); // 버퍼링 중엔 숨김

        // 제목
        GameObject titleGO = MakeTMP("TitleText", contentArea.transform, "전리품", 30, Color.white);
        TextMeshProUGUI titleTMP = titleGO.GetComponent<TextMeshProUGUI>();
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.fontStyle = FontStyles.Bold;
        RectTransform titleRT = titleGO.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0f, 1f);
        titleRT.anchorMax = new Vector2(1f, 1f);
        titleRT.pivot     = new Vector2(0.5f, 1f);
        titleRT.anchoredPosition = new Vector2(0f, -16f);
        titleRT.sizeDelta = new Vector2(0f, 48f);

        // InventoryUI 패널 + Content RT
        GameObject invPanel = new GameObject("InventoryPanel");
        invPanel.transform.SetParent(contentArea.transform, false);
        RectTransform invPanelRT = invPanel.AddComponent<RectTransform>();
        invPanelRT.anchorMin = Vector2.zero;
        invPanelRT.anchorMax = Vector2.one;
        invPanelRT.offsetMin = new Vector2(0f, 0f);
        invPanelRT.offsetMax = new Vector2(0f, -64f); // 제목 아래

        InventoryUI invUI = invPanel.AddComponent<InventoryUI>();

        GameObject content = new GameObject("Content");
        content.transform.SetParent(invPanel.transform, false);
        RectTransform contentRT = content.AddComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0f, 1f);
        contentRT.anchorMax = new Vector2(0f, 1f);
        contentRT.pivot     = new Vector2(0f, 1f);
        contentRT.anchoredPosition = Vector2.zero;
        contentRT.sizeDelta = Vector2.zero;

        // InventoryUI 필드 연결
        SerializedObject invSO = new SerializedObject(invUI);
        invSO.FindProperty("_contentAreaRT").objectReferenceValue = contentRT;
        invSO.FindProperty("_slotUIPrefab").objectReferenceValue  = itemSlotPrefab;
        invSO.FindProperty("_horizontalSlotCount").intValue = 5;
        invSO.FindProperty("_verticalSlotCount").intValue   = 4;
        invSO.FindProperty("_slotSize").floatValue          = 72f;
        invSO.FindProperty("_slotMargin").floatValue        = 8f;
        invSO.FindProperty("_contentAreaPadding").floatValue = 16f;
        invSO.ApplyModifiedPropertiesWithoutUndo();

        // LootBoxRevealUI 필드 연결
        SerializedObject revealSO = new SerializedObject(reveal);
        revealSO.FindProperty("_root").objectReferenceValue            = root;
        revealSO.FindProperty("_bufferingPanel").objectReferenceValue  = bufPanel;
        revealSO.FindProperty("_spinnerImage").objectReferenceValue    = spinImg;
        revealSO.FindProperty("_contentArea").objectReferenceValue     = contentArea;
        revealSO.FindProperty("_lootInventoryUI").objectReferenceValue = invUI;
        revealSO.ApplyModifiedPropertiesWithoutUndo();

        // ── 저장 ─────────────────────────────────────────────
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, SavePath);
        Object.DestroyImmediate(go);

        if (prefab != null)
        {
            Debug.Log($"✓ LootBoxRevealUI 프리팹 생성: {SavePath}");
            EditorGUIUtility.PingObject(prefab);
            Selection.activeObject = prefab;
        }
    }

    // ─── 헬퍼 ────────────────────────────────────────────────

    private static GameObject MakePanel(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<CanvasRenderer>();
        Image img = go.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
        go.AddComponent<RectTransform>();
        return go;
    }

    private static GameObject MakeTMP(string name, Transform parent, string text, float size, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<CanvasRenderer>();
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        go.AddComponent<RectTransform>();
        return go;
    }

    private static void Stretch(GameObject go)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.pivot     = new Vector2(0.5f, 0.5f);
    }
}
#endif
