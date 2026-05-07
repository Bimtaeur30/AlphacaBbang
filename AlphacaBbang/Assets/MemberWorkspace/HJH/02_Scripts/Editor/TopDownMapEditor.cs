// TopDownMapEditor.cs
// 위치: Assets/.../Editor/TopDownMapEditor.cs
// 메뉴: Tools > TopDown Map Editor

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TopDownMapEditor : EditorWindow
{
    // ── 모드 ──────────────────────────────────────────────
    public enum EditMode { TilePaint, PrefabPlace, Erase }
    private EditMode currentMode = EditMode.TilePaint;

    // ── 설정 ──────────────────────────────────────────────
    private float gridSize = 1f;
    private float fixedY = 0f;
    private bool snapToGrid = true;
    private bool showGrid = true;
    private float brushRotation = 0f;

    // ── 팔레트 ───────────────────────────────────────────
    private List<GameObject> tilePrefabs = new List<GameObject>();
    private List<GameObject> placePrefabs = new List<GameObject>();
    private int selectedTileIndex = 0;
    private int selectedPrefabIndex = 0;
    private Vector2 tileScrollPos;
    private Vector2 prefabScrollPos;
    private Vector2 mainScrollPos;

    // ── 고스트 미리보기 ───────────────────────────────────
    private GameObject ghostObject;
    private static readonly Color GhostColor = new Color(0f, 1f, 0.5f, 0.35f);

    // ── 부모 오브젝트 ─────────────────────────────────────
    private Transform tileRoot;
    private Transform prefabRoot;

    // ─────────────────────────────────────────────────────
    [MenuItem("Tools/TopDown Map Editor")]
    public static void ShowWindow()
    {
        var win = GetWindow<TopDownMapEditor>("Map Editor");
        win.minSize = new Vector2(280, 500);
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        EnsureRoots();
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        DestroyGhost();
    }

    // ══════════════════════════════════════════════════════
    //  에디터 GUI
    // ══════════════════════════════════════════════════════
    private void OnGUI()
    {
        mainScrollPos = EditorGUILayout.BeginScrollView(mainScrollPos);

        EditorGUILayout.Space(6);
        GUILayout.Label("TopDown Map Editor", EditorStyles.boldLabel);
        DrawSeparator();

        // ── 모드 선택 ────────────────────────────────────
        EditorGUILayout.LabelField("모드", EditorStyles.miniBoldLabel);
        EditMode newMode = (EditMode)GUILayout.Toolbar((int)currentMode,
            new[] { "타일", "프리팹", "삭제" });
        if (newMode != currentMode)
        {
            currentMode = newMode;
            DestroyGhost();
        }
        DrawSeparator();

        // ── 그리드 설정 ──────────────────────────────────
        EditorGUILayout.LabelField("그리드 설정", EditorStyles.miniBoldLabel);
        snapToGrid = EditorGUILayout.Toggle("스냅 활성화", snapToGrid);
        showGrid = EditorGUILayout.Toggle("그리드 표시", showGrid);
        gridSize = EditorGUILayout.FloatField("그리드 크기", Mathf.Max(0.1f, gridSize));
        fixedY = EditorGUILayout.FloatField("Y 고정값", fixedY);
        brushRotation = EditorGUILayout.Slider("브러시 회전 (Y)", brushRotation, 0f, 360f);
        DrawSeparator();

        // ── 팔레트 ───────────────────────────────────────
        if (currentMode == EditMode.TilePaint)
            DrawPalette("타일 팔레트", tilePrefabs, ref selectedTileIndex, ref tileScrollPos);
        else if (currentMode == EditMode.PrefabPlace)
            DrawPalette("프리팹 팔레트", placePrefabs, ref selectedPrefabIndex, ref prefabScrollPos);
        else
            EditorGUILayout.HelpBox("씬 뷰에서 클릭하면 오브젝트를 삭제합니다.", MessageType.Info);

        DrawSeparator();

        // ── 유틸리티 ─────────────────────────────────────
        EditorGUILayout.LabelField("유틸리티", EditorStyles.miniBoldLabel);
        if (GUILayout.Button("루트 오브젝트 재생성"))
            EnsureRoots(force: true);
        if (GUILayout.Button("타일 전체 삭제"))
        {
            if (EditorUtility.DisplayDialog("확인", "타일을 전부 삭제할까요?", "삭제", "취소"))
                ClearRoot(tileRoot);
        }
        if (GUILayout.Button("프리팹 전체 삭제"))
        {
            if (EditorUtility.DisplayDialog("확인", "프리팹을 전부 삭제할까요?", "삭제", "취소"))
                ClearRoot(prefabRoot);
        }

        EditorGUILayout.EndScrollView();
    }

    // ── 팔레트 드로어 ─────────────────────────────────────
    private void DrawPalette(string label, List<GameObject> palette,
        ref int selectedIndex, ref Vector2 scrollPos)
    {
        EditorGUILayout.LabelField(label, EditorStyles.miniBoldLabel);

        // 목록 편집
        for (int i = 0; i < palette.Count; i++)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                palette[i] = (GameObject)EditorGUILayout.ObjectField(
                    $"[{i}]", palette[i], typeof(GameObject), false);
                if (GUILayout.Button("X", GUILayout.Width(24)))
                {
                    palette.RemoveAt(i);
                    if (selectedIndex >= palette.Count)
                        selectedIndex = Mathf.Max(0, palette.Count - 1);
                    DestroyGhost();
                    break;
                }
            }
        }

        if (GUILayout.Button("+ 추가"))
            palette.Add(null);

        if (palette.Count == 0)
        {
            EditorGUILayout.HelpBox("+ 추가 버튼으로 프리팹을 넣어주세요.", MessageType.Warning);
            return;
        }

        EditorGUILayout.Space(4);

        // 썸네일 그리드
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(130));
        int cols = Mathf.Max(1, Mathf.FloorToInt((position.width - 24) / 68));

        for (int i = 0; i < palette.Count; i += cols)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                for (int c = 0; c < cols; c++)
                {
                    int idx = i + c;
                    if (idx >= palette.Count) break;

                    var go = palette[idx];
                    var thumb = go ? AssetPreview.GetAssetPreview(go) : null;
                    bool sel = (selectedIndex == idx);

                    GUI.backgroundColor = sel ? Color.cyan : Color.white;
                    bool clicked = thumb != null
                        ? GUILayout.Button(thumb, GUILayout.Width(64), GUILayout.Height(64))
                        : GUILayout.Button(go ? go.name : "None", GUILayout.Width(64), GUILayout.Height(64));
                    GUI.backgroundColor = Color.white;

                    if (clicked)
                    {
                        selectedIndex = idx;
                        DestroyGhost();
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }

    // ══════════════════════════════════════════════════════
    //  Scene GUI
    // ══════════════════════════════════════════════════════
    private void OnSceneGUI(SceneView sceneView)
    {
        if (showGrid) DrawGrid();

        Event e = Event.current;

        if (!GetWorldPosition(e.mousePosition, out Vector3 worldPos))
        {
            DestroyGhost();
            return;
        }

        Vector3 snapped = SnapPosition(worldPos);
        UpdateGhost(snapped);

        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
        {
            switch (currentMode)
            {
                case EditMode.TilePaint:
                    PlaceObject(snapped, tilePrefabs, selectedTileIndex, tileRoot, allowOverlap: false);
                    break;
                case EditMode.PrefabPlace:
                    PlaceObject(snapped, placePrefabs, selectedPrefabIndex, prefabRoot, allowOverlap: true);
                    break;
                case EditMode.Erase:
                    EraseAt(snapped);
                    break;
            }
            e.Use();
        }

        // 드래그 — 타일 연속 배치
        if (currentMode == EditMode.TilePaint &&
            e.type == EventType.MouseDrag && e.button == 0 && !e.alt)
        {
            PlaceObject(snapped, tilePrefabs, selectedTileIndex, tileRoot, allowOverlap: false);
            e.Use();
        }

        sceneView.Repaint();
    }

    // ─────────────────────────────────────────────────────
    //  배치
    // ─────────────────────────────────────────────────────
    private void PlaceObject(Vector3 pos, List<GameObject> palette, int index,
        Transform root, bool allowOverlap)
    {
        if (palette == null || index < 0 || index >= palette.Count) return;
        if (palette[index] == null) return;
        if (root == null) EnsureRoots();

        if (!allowOverlap)
        {
            foreach (Transform child in root)
            {
                if (Vector3.Distance(child.position, pos) < gridSize * 0.1f) return;
            }
        }

        var go = (GameObject)PrefabUtility.InstantiatePrefab(palette[index]);
        go.transform.SetParent(root);
        go.transform.position = pos;
        go.transform.rotation = Quaternion.Euler(0f, brushRotation, 0f);
        Undo.RegisterCreatedObjectUndo(go, "Map Place");
    }

    // ─────────────────────────────────────────────────────
    //  삭제
    // ─────────────────────────────────────────────────────
    private void EraseAt(Vector3 pos)
    {
        float radius = gridSize * 0.6f;
        foreach (Transform root in new[] { tileRoot, prefabRoot })
        {
            if (root == null) continue;
            foreach (Transform child in root)
            {
                if (Vector3.Distance(child.position, pos) < radius)
                {
                    Undo.DestroyObjectImmediate(child.gameObject);
                    return;
                }
            }
        }
    }

    // ─────────────────────────────────────────────────────
    //  고스트 미리보기
    // ─────────────────────────────────────────────────────
    private void UpdateGhost(Vector3 pos)
    {
        if (currentMode == EditMode.Erase) { DestroyGhost(); return; }

        var palette = currentMode == EditMode.TilePaint ? tilePrefabs : placePrefabs;
        int idx = currentMode == EditMode.TilePaint ? selectedTileIndex : selectedPrefabIndex;

        if (palette == null || idx < 0 || idx >= palette.Count || palette[idx] == null)
        {
            DestroyGhost();
            return;
        }

        if (ghostObject == null)
        {
            ghostObject = Instantiate(palette[idx]);
            ghostObject.name = "__MapEditorGhost__";
            ghostObject.hideFlags = HideFlags.HideAndDontSave;

            foreach (var col in ghostObject.GetComponentsInChildren<Collider>()) col.enabled = false;
            foreach (var mono in ghostObject.GetComponentsInChildren<MonoBehaviour>()) mono.enabled = false;

            ApplyGhostMaterial(ghostObject);
        }

        ghostObject.transform.position = pos;
        ghostObject.transform.rotation = Quaternion.Euler(0f, brushRotation, 0f);
    }

    private void ApplyGhostMaterial(GameObject go)
    {
        var mat = new Material(Shader.Find("Standard"));
        mat.color = GhostColor;
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;

        foreach (var rend in go.GetComponentsInChildren<Renderer>())
        {
            var mats = new Material[rend.sharedMaterials.Length];
            for (int i = 0; i < mats.Length; i++) mats[i] = mat;
            rend.materials = mats;
        }
    }

    private void DestroyGhost()
    {
        if (ghostObject != null) DestroyImmediate(ghostObject);
        ghostObject = null;
    }

    // ─────────────────────────────────────────────────────
    //  그리드
    // ─────────────────────────────────────────────────────
    private void DrawGrid()
    {
        int half = 30;
        float extent = half * gridSize;
        Handles.color = new Color(0.5f, 0.5f, 1f, 0.2f);
        for (int i = -half; i <= half; i++)
        {
            float t = i * gridSize;
            Handles.DrawLine(new Vector3(-extent, fixedY, t), new Vector3(extent, fixedY, t));
            Handles.DrawLine(new Vector3(t, fixedY, -extent), new Vector3(t, fixedY, extent));
        }
        Handles.color = Color.white;
    }

    // ─────────────────────────────────────────────────────
    //  좌표 변환
    // ─────────────────────────────────────────────────────
    private bool GetWorldPosition(Vector2 mousePos, out Vector3 result)
    {
        result = Vector3.zero;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
        float denom = ray.direction.y;
        if (Mathf.Abs(denom) < 0.0001f) return false;
        float t = (fixedY - ray.origin.y) / denom;
        if (t < 0) return false;
        result = ray.origin + ray.direction * t;
        result.y = fixedY;
        return true;
    }

    private Vector3 SnapPosition(Vector3 pos)
    {
        if (!snapToGrid) return new Vector3(pos.x, fixedY, pos.z);
        return new Vector3(
            Mathf.Round(pos.x / gridSize) * gridSize,
            fixedY,
            Mathf.Round(pos.z / gridSize) * gridSize
        );
    }

    // ─────────────────────────────────────────────────────
    //  루트 오브젝트
    // ─────────────────────────────────────────────────────
    private void EnsureRoots(bool force = false)
    {
        tileRoot = FindOrCreateRoot("_TileRoot", force);
        prefabRoot = FindOrCreateRoot("_PrefabRoot", force);
    }

    private Transform FindOrCreateRoot(string name, bool force)
    {
        var existing = GameObject.Find(name);
        if (existing != null)
        {
            if (force) DestroyImmediate(existing);
            else return existing.transform;
        }
        return new GameObject(name).transform;
    }

    private void ClearRoot(Transform root)
    {
        if (root == null) return;
        var children = new List<GameObject>();
        foreach (Transform c in root) children.Add(c.gameObject);
        foreach (var c in children) Undo.DestroyObjectImmediate(c);
    }

    // ─────────────────────────────────────────────────────
    //  UI 헬퍼
    // ─────────────────────────────────────────────────────
    private void DrawSeparator()
    {
        EditorGUILayout.Space(4);
        Rect r = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(r, new Color(0.5f, 0.5f, 0.5f, 0.3f));
        EditorGUILayout.Space(4);
    }
}
