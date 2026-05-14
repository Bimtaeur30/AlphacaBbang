#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class ContextMenuPrefabCreator
{
    private const string SavePath   = "Assets/MemberWorkspace/JJG/03_GameModule/Prefabs/InventoryContextMenu.prefab";
    private const string FontGuid   = "8fbede167c96fae4194381df30211c33";
    private const string PanelSpriteGuid = "f36d9d25b51abad4f9d1bd0ed8492d0d";
    private const long   PanelSpriteFileId = 7482667652216324306;

    // 버튼 색상 정의
    private static readonly Color BtnNormal   = new Color(0.25f, 0.25f, 0.28f, 1f);
    private static readonly Color BtnHover    = new Color(0.35f, 0.35f, 0.40f, 1f);
    private static readonly Color BtnPressed  = new Color(0.18f, 0.18f, 0.20f, 1f);
    private static readonly Color BtnDisabled = new Color(0.20f, 0.20f, 0.22f, 0.5f);

    [MenuItem("JJG/Create InventoryContextMenu Prefab")]
    public static void Create()
    {
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
            AssetDatabase.GUIDToAssetPath(FontGuid));

        // ── 최상위 GO (InventoryContextMenu 컴포넌트) ──────────
        GameObject root = new GameObject("InventoryContextMenu");
        root.AddComponent<RectTransform>();
        InventoryContextMenu menuComp = root.AddComponent<InventoryContextMenu>();

        // ── RootPanel ──────────────────────────────────────────
        GameObject panel = new GameObject("RootPanel");
        panel.transform.SetParent(root.transform, false);

        RectTransform panelRT = panel.AddComponent<RectTransform>();
        panelRT.sizeDelta = new Vector2(160f, 0f);
        panelRT.pivot     = new Vector2(0f, 1f);

        panel.AddComponent<CanvasRenderer>();
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0.12f, 0.12f, 0.15f, 0.96f);

        Sprite panelSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            AssetDatabase.GUIDToAssetPath(PanelSpriteGuid));
        if (panelSprite != null) panelImg.sprite = panelSprite;

        ContentSizeFitter csf = panel.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        VerticalLayoutGroup vlg = panel.AddComponent<VerticalLayoutGroup>();
        vlg.padding           = new RectOffset(6, 6, 6, 6);
        vlg.spacing           = 2f;
        vlg.childAlignment    = TextAnchor.UpperCenter;
        vlg.childForceExpandWidth  = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth  = true;
        vlg.childControlHeight = true;

        // ── 버튼 4개 생성 ─────────────────────────────────────
        Button useBtn     = MakeButton(panel.transform, "UseButton",     "사용",     font, new Color(0.3f, 0.6f, 0.3f, 1f));
        Button equipBtn   = MakeButton(panel.transform, "EquipButton",   "장착",     font, BtnNormal);
        Button unequipBtn = MakeButton(panel.transform, "UnequipButton", "장착해제", font, new Color(0.5f, 0.35f, 0.15f, 1f));
        Button dropBtn    = MakeButton(panel.transform, "DropButton",    "버리기",   font, new Color(0.55f, 0.18f, 0.18f, 1f));

        // ── InventoryContextMenu 필드 연결 ─────────────────────
        SerializedObject so = new SerializedObject(menuComp);
        so.FindProperty("rootPanel").objectReferenceValue     = panel;
        so.FindProperty("useButton").objectReferenceValue     = useBtn;
        so.FindProperty("equipButton").objectReferenceValue   = equipBtn;
        so.FindProperty("unequipButton").objectReferenceValue = unequipBtn;
        so.FindProperty("dropButton").objectReferenceValue    = dropBtn;
        so.ApplyModifiedPropertiesWithoutUndo();

        // ── 저장 ──────────────────────────────────────────────
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, SavePath);
        Object.DestroyImmediate(root);

        if (prefab != null)
        {
            Debug.Log($"InventoryContextMenu 프리팹 생성 완료: {SavePath}");
            EditorGUIUtility.PingObject(prefab);
            Selection.activeObject = prefab;
        }
    }

    private static Button MakeButton(Transform parent, string name, string label,
                                     TMP_FontAsset font, Color normalColor)
    {
        // 버튼 GO
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        go.AddComponent<CanvasRenderer>();
        Image img = go.AddComponent<Image>();
        img.color = normalColor;

        Button btn = go.AddComponent<Button>();

        ColorBlock cb = btn.colors;
        cb.normalColor      = normalColor;
        cb.highlightedColor = Color.Lerp(normalColor, Color.white, 0.25f);
        cb.pressedColor     = Color.Lerp(normalColor, Color.black, 0.25f);
        cb.disabledColor    = BtnDisabled;
        cb.fadeDuration     = 0.08f;
        btn.colors = cb;

        LayoutElement le = go.AddComponent<LayoutElement>();
        le.minHeight       = 36f;
        le.preferredHeight = 36f;

        // 라벨 TMP
        GameObject txtGO = new GameObject("Text");
        txtGO.transform.SetParent(go.transform, false);
        txtGO.AddComponent<CanvasRenderer>();

        TextMeshProUGUI tmp = txtGO.AddComponent<TextMeshProUGUI>();
        tmp.text      = label;
        tmp.fontSize  = 15f;
        tmp.color     = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.raycastTarget = false;
        if (font != null) tmp.font = font;

        RectTransform txtRT = txtGO.GetComponent<RectTransform>();
        txtRT.anchorMin = Vector2.zero;
        txtRT.anchorMax = Vector2.one;
        txtRT.offsetMin = Vector2.zero;
        txtRT.offsetMax = Vector2.zero;

        btn.targetGraphic = img;

        return btn;
    }
}
#endif
