using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// ────────────────────────────────────────────────────────────────────────────
//  AnimationEditor
//  Assets/MemberWorkspace/HJH/02_Scripts/AnimationEditor/Editor/ 에 넣으세요.
//  메뉴: Tools > Animation Editor
// ────────────────────────────────────────────────────────────────────────────

public class AnimationEditor : EditorWindow
{
    // ── 고정 상수 ──────────────────────────────────────────────────────────────
    const int TOTAL_FRAMES = 30;
    const float TOOLBAR_HEIGHT = 30f;
    const float FRAME_CELL_WIDTH = 18f;
    const float TRACK_LABEL_WIDTH = 120f;
    const float TRACK_ROW_HEIGHT = 22f;
    const float SPLITTER_W = 4f;
    const float PANEL_MIN = 100f;

    // ── 패널 크기 (드래그 조절) ────────────────────────────────────────────────
    float hierWidth = 220f;
    float inspWidth = 210f;
    float tlHeight = 140f;
    bool draggingHier, draggingInsp, draggingTL;

    // ── 데이터 ────────────────────────────────────────────────────────────────
    GameObject rootObject;
    List<Transform> allTransforms = new List<Transform>();
    HashSet<Transform> activeParts = new HashSet<Transform>();
    HashSet<Transform> collapsed = new HashSet<Transform>();
    Transform selectedPart;

    Dictionary<Transform, Dictionary<int, FrameData>> keyframes
        = new Dictionary<Transform, Dictionary<int, FrameData>>();

    int currentFrame = 0;
    bool playing = false;
    double lastPlayTime = 0;

    Vector2 hierScroll, inspScroll, tlScroll;

    struct FrameData
    {
        public Vector3 localPosition;
        public Vector3 localEulerAngles;
    }

    // ── 메뉴 등록 ──────────────────────────────────────────────────────────────
    [MenuItem("Tools/Animation Editor")]
    static void Open()
    {
        var win = GetWindow<AnimationEditor>("Anim Editor");
        win.minSize = new Vector2(600, 400);
    }

    // ── 생명주기 ───────────────────────────────────────────────────────────────
    void OnEnable()
    {
        // Scene View에서 클릭 → 자동으로 해당 파트 선택
        Selection.selectionChanged += OnSceneSelectionChanged;
        // Scene View Move/Rotate 툴 조작 감지
        Undo.postprocessModifications += OnPostProcessModifications;
    }

    void OnDisable()
    {
        Selection.selectionChanged -= OnSceneSelectionChanged;
        Undo.postprocessModifications -= OnPostProcessModifications;
    }

    void OnInspectorUpdate()
    {
        if (playing) Repaint();
    }

    // Scene View에서 오브젝트 클릭 → selectedPart 자동 동기화
    void OnSceneSelectionChanged()
    {
        if (rootObject == null) return;
        var sel = Selection.activeTransform;
        if (sel == null || !allTransforms.Contains(sel)) return;

        selectedPart = sel;
        // 선택된 파트가 비활성이면 자동으로 활성화
        activeParts.Add(sel);
        Repaint();
    }

    // Scene View에서 Move/Rotate 툴로 직접 움직였을 때 → Repaint
    UndoPropertyModification[] OnPostProcessModifications(UndoPropertyModification[] mods)
    {
        if (selectedPart == null) return mods;
        bool moved = mods.Any(m =>
            m.currentValue.target == (Object)selectedPart &&
            (m.currentValue.propertyPath.StartsWith("m_LocalPosition") ||
             m.currentValue.propertyPath.StartsWith("m_LocalRotation") ||
             m.currentValue.propertyPath.StartsWith("m_LocalEulerAngles")));
        if (moved) Repaint();
        return mods;
    }

    // ── GUI 진입점 ─────────────────────────────────────────────────────────────
    void OnGUI()
    {
        if (playing)
        {
            double now = EditorApplication.timeSinceStartup;
            if (now - lastPlayTime >= 1.0 / 12.0)
            {
                currentFrame = (currentFrame + 1) % TOTAL_FRAMES;
                ApplyInterpolation();
                lastPlayTime = now;
                Repaint();
            }
        }

        HandleSplitterInput();
        DrawToolbar();

        float mainH = position.height - TOOLBAR_HEIGHT - tlHeight;

        Rect hierRect = new Rect(0, TOOLBAR_HEIGHT, hierWidth, mainH);
        Rect hierSplit = new Rect(hierWidth, TOOLBAR_HEIGHT, SPLITTER_W, mainH);
        Rect inspRect = new Rect(position.width - inspWidth, TOOLBAR_HEIGHT, inspWidth, mainH);
        Rect inspSplit = new Rect(position.width - inspWidth - SPLITTER_W, TOOLBAR_HEIGHT, SPLITTER_W, mainH);
        float sceneX = hierWidth + SPLITTER_W;
        float sceneW = position.width - hierWidth - inspWidth - SPLITTER_W * 2;
        Rect sceneRect = new Rect(sceneX, TOOLBAR_HEIGHT, sceneW, mainH);
        Rect tlSplit = new Rect(0, position.height - tlHeight - SPLITTER_W, position.width, SPLITTER_W);
        Rect tlRect = new Rect(0, position.height - tlHeight, position.width, tlHeight);

        DrawHierarchy(hierRect);
        DrawSplitter(hierSplit, true);
        EditorGUIUtility.AddCursorRect(hierSplit, MouseCursor.ResizeHorizontal);

        DrawInspector(inspRect);
        DrawSplitter(inspSplit, true);
        EditorGUIUtility.AddCursorRect(inspSplit, MouseCursor.ResizeHorizontal);

        DrawScenePanel(sceneRect);

        DrawSplitter(tlSplit, false);
        EditorGUIUtility.AddCursorRect(tlSplit, MouseCursor.ResizeVertical);
        DrawTimeline(tlRect);
    }

    // ── 스플리터 ───────────────────────────────────────────────────────────────
    void HandleSplitterInput()
    {
        Event e = Event.current;
        float mainH = position.height - TOOLBAR_HEIGHT - tlHeight;
        Rect hierSplit = new Rect(hierWidth, TOOLBAR_HEIGHT, SPLITTER_W, mainH);
        Rect inspSplit = new Rect(position.width - inspWidth - SPLITTER_W, TOOLBAR_HEIGHT, SPLITTER_W, mainH);
        Rect tlSplit = new Rect(0, position.height - tlHeight - SPLITTER_W, position.width, SPLITTER_W);

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            if (hierSplit.Contains(e.mousePosition)) { draggingHier = true; e.Use(); }
            else if (inspSplit.Contains(e.mousePosition)) { draggingInsp = true; e.Use(); }
            else if (tlSplit.Contains(e.mousePosition)) { draggingTL = true; e.Use(); }
        }
        if (e.type == EventType.MouseDrag && e.button == 0)
        {
            if (draggingHier) { hierWidth = Mathf.Clamp(e.mousePosition.x, PANEL_MIN, position.width - inspWidth - PANEL_MIN - SPLITTER_W * 2); Repaint(); e.Use(); }
            if (draggingInsp) { inspWidth = Mathf.Clamp(position.width - e.mousePosition.x - SPLITTER_W, PANEL_MIN, position.width - hierWidth - PANEL_MIN - SPLITTER_W * 2); Repaint(); e.Use(); }
            if (draggingTL) { tlHeight = Mathf.Clamp(position.height - e.mousePosition.y, 60f, position.height - TOOLBAR_HEIGHT - 100f); Repaint(); e.Use(); }
        }
        if (e.type == EventType.MouseUp) { draggingHier = draggingInsp = draggingTL = false; }
    }

    void DrawSplitter(Rect r, bool vert)
    {
        bool active = (vert && (draggingHier || draggingInsp)) || (!vert && draggingTL);
        EditorGUI.DrawRect(r, active ? new Color(0.55f, 0.45f, 1f, 0.8f) : new Color(0.22f, 0.22f, 0.30f));
    }

    // ── 툴바 ───────────────────────────────────────────────────────────────────
    void DrawToolbar()
    {
        Rect r = new Rect(0, 0, position.width, TOOLBAR_HEIGHT);
        EditorGUI.DrawRect(r, new Color(0.13f, 0.13f, 0.17f));

        GUILayout.BeginArea(r);
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        GUILayout.Label("루트:", GUILayout.Width(34));
        var newRoot = (GameObject)EditorGUILayout.ObjectField(rootObject, typeof(GameObject), true, GUILayout.Width(150));
        if (newRoot != rootObject) { rootObject = newRoot; RefreshTree(); }

        GUILayout.Space(8);

        if (GUILayout.Button("⏮", EditorStyles.toolbarButton, GUILayout.Width(24))) { currentFrame = 0; ApplyInterpolation(); }
        if (GUILayout.Button("◀", EditorStyles.toolbarButton, GUILayout.Width(24))) { currentFrame = Mathf.Max(0, currentFrame - 1); ApplyInterpolation(); }
        bool wasPlaying = playing;
        playing = GUILayout.Toggle(playing, playing ? "⏸" : "▶", EditorStyles.toolbarButton, GUILayout.Width(26));
        if (playing && !wasPlaying) lastPlayTime = EditorApplication.timeSinceStartup;
        if (GUILayout.Button("▶", EditorStyles.toolbarButton, GUILayout.Width(24))) { currentFrame = Mathf.Min(TOTAL_FRAMES - 1, currentFrame + 1); ApplyInterpolation(); }
        if (GUILayout.Button("⏭", EditorStyles.toolbarButton, GUILayout.Width(24))) { currentFrame = TOTAL_FRAMES - 1; ApplyInterpolation(); }

        GUILayout.Space(6);
        GUILayout.Label($"{currentFrame + 1} / {TOTAL_FRAMES}", GUILayout.Width(50));

        GUILayout.FlexibleSpace();

        // ★ 현재 장면 전체 저장 버튼 (툴바에 배치)
        if (activeParts.Count > 0)
        {
            Color prev = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.35f, 0.25f, 0.75f);
            if (GUILayout.Button($"◆  장면 전체 저장  ({activeParts.Count}파트)", EditorStyles.toolbarButton, GUILayout.Width(190)))
                SaveAllActiveParts();
            GUI.backgroundColor = prev;
        }

        GUILayout.Space(6);

        if (GUILayout.Button("전체 초기화", EditorStyles.toolbarButton, GUILayout.Width(70)))
        {
            if (EditorUtility.DisplayDialog("초기화", "모든 키프레임을 삭제할까요?", "삭제", "취소"))
            { keyframes.Clear(); currentFrame = 0; ApplyInterpolation(); }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    // ── Hierarchy ──────────────────────────────────────────────────────────────
    void DrawHierarchy(Rect rect)
    {
        EditorGUI.DrawRect(rect, new Color(0.12f, 0.12f, 0.16f));
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 18), new Color(0.10f, 0.10f, 0.14f));
        GUI.Label(new Rect(rect.x + 8, rect.y + 2, rect.width, 16), "HIERARCHY", EditorStyles.miniLabel);

        if (rootObject == null)
        {
            GUI.Label(new Rect(rect.x + 8, rect.y + 30, rect.width - 16, 40),
                "툴바에서 루트 오브젝트를 넣어주세요.", EditorStyles.wordWrappedMiniLabel);
            return;
        }

        Rect scrollArea = new Rect(rect.x, rect.y + 20, rect.width, rect.height - 50);
        Rect contentRect = new Rect(0, 0, rect.width - 14, Mathf.Max(allTransforms.Count * 22f, scrollArea.height));
        hierScroll = GUI.BeginScrollView(scrollArea, hierScroll, contentRect);
        float y = 0;
        DrawTransformNode(rootObject.transform, 0, ref y, rect.width - 16);
        GUI.EndScrollView();

        Rect btnArea = new Rect(rect.x, rect.y + rect.height - 28, rect.width, 28);
        EditorGUI.DrawRect(btnArea, new Color(0.10f, 0.10f, 0.14f));
        if (GUI.Button(new Rect(rect.x + 4, rect.y + rect.height - 24, rect.width / 2 - 6, 20), "전체 해제", EditorStyles.miniButton)) activeParts.Clear();
        if (GUI.Button(new Rect(rect.x + rect.width / 2 + 2, rect.y + rect.height - 24, rect.width / 2 - 6, 20), "전체 선택", EditorStyles.miniButton)) activeParts = new HashSet<Transform>(allTransforms);
    }

    void DrawTransformNode(Transform t, int depth, ref float y, float width)
    {
        bool hasChildren = t.childCount > 0;
        bool isCollapsed = collapsed.Contains(t);
        bool isActive = activeParts.Contains(t);
        bool isSelected = selectedPart == t;
        bool hasKF = keyframes.ContainsKey(t) && keyframes[t].Count > 0;

        Rect rowRect = new Rect(0, y, width, 22f);
        if (isSelected) EditorGUI.DrawRect(rowRect, new Color(0.18f, 0.18f, 0.32f));

        float indent = 6 + depth * 12;

        if (hasChildren)
            if (GUI.Button(new Rect(indent, y + 4, 12, 14), isCollapsed ? "▶" : "▼", EditorStyles.miniLabel))
            { if (isCollapsed) collapsed.Remove(t); else collapsed.Add(t); }

        bool newActive = EditorGUI.Toggle(new Rect(indent + 14, y + 4, 14, 14), isActive);
        if (newActive != isActive) { if (newActive) activeParts.Add(t); else activeParts.Remove(t); }

        GUIStyle ns = new GUIStyle(EditorStyles.miniLabel);
        ns.normal.textColor = isSelected ? new Color(0.85f, 0.80f, 1f)
                            : isActive ? new Color(0.75f, 0.70f, 1f)
                            : new Color(0.45f, 0.45f, 0.55f);
        GUI.Label(new Rect(indent + 32, y + 3, width - indent - 50, 16), t.name, ns);

        if (hasKF) EditorGUI.DrawRect(new Rect(width - 12, y + 8, 6, 6), new Color(0.94f, 0.63f, 0.19f));

        if (Event.current.type == EventType.MouseDown && rowRect.Contains(Event.current.mousePosition))
        {
            selectedPart = t;
            // 에디터 창에서 클릭해도 유니티 Selection 동기화
            Selection.activeTransform = t;
            Event.current.Use();
            Repaint();
        }

        y += 22f;
        if (!isCollapsed)
            for (int i = 0; i < t.childCount; i++)
                DrawTransformNode(t.GetChild(i), depth + 1, ref y, width);
    }

    // ── Scene Panel ────────────────────────────────────────────────────────────
    void DrawScenePanel(Rect rect)
    {
        EditorGUI.DrawRect(rect, new Color(0.09f, 0.09f, 0.12f));
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 18), new Color(0.10f, 0.10f, 0.14f));
        GUI.Label(new Rect(rect.x + 8, rect.y + 2, rect.width, 16),
            "SCENE  —  Scene View 또는 여기서 파트 클릭 후 Move/Rotate 툴로 조작", EditorStyles.miniLabel);

        if (rootObject == null) return;

        if (activeParts.Count == 0)
        {
            GUIStyle hint = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
            hint.normal.textColor = new Color(0.25f, 0.25f, 0.3f);
            GUI.Label(new Rect(rect.x, rect.y + rect.height / 2 - 20, rect.width, 40),
                "← Hierarchy에서 파트를 체크하거나\nScene View에서 오브젝트를 클릭하세요.", hint);
            return;
        }

        // 활성 파트 목록
        GUIStyle tagStyle = new GUIStyle(EditorStyles.miniLabel);
        float py = rect.y + 26;
        foreach (var t in activeParts.Take(30))
        {
            if (py > rect.y + rect.height - 50) break;
            bool isSel = t == selectedPart;
            bool hasKF = keyframes.ContainsKey(t) && keyframes[t].Count > 0;
            tagStyle.normal.textColor = isSel ? new Color(0.80f, 0.70f, 1f) : new Color(0.38f, 0.38f, 0.52f);

            // 클릭하면 선택
            Rect labelRect = new Rect(rect.x + 12, py, rect.width - 20, 17);
            GUI.Label(labelRect, (hasKF ? "◆ " : "· ") + t.name, tagStyle);
            if (Event.current.type == EventType.MouseDown && labelRect.Contains(Event.current.mousePosition))
            {
                selectedPart = t;
                Selection.activeTransform = t;
                Event.current.Use();
                Repaint();
            }
            py += 17;
        }
        if (activeParts.Count > 30)
        {
            tagStyle.normal.textColor = new Color(0.28f, 0.28f, 0.38f);
            GUI.Label(new Rect(rect.x + 12, py, rect.width - 20, 17), $"... 외 {activeParts.Count - 30}개", tagStyle);
        }

        GUIStyle info = new GUIStyle(EditorStyles.miniLabel);
        info.normal.textColor = new Color(0.25f, 0.25f, 0.35f);
        info.alignment = TextAnchor.LowerCenter;
        GUI.Label(new Rect(rect.x, rect.y + rect.height - 34, rect.width, 30),
            "조작 후 상단 [◆ 장면 전체 저장] 또는 Inspector [◆ 이 파트만 저장] 으로 키프레임 저장", info);
    }

    // ── Inspector ──────────────────────────────────────────────────────────────
    void DrawInspector(Rect rect)
    {
        EditorGUI.DrawRect(rect, new Color(0.12f, 0.12f, 0.16f));
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 18), new Color(0.10f, 0.10f, 0.14f));
        GUI.Label(new Rect(rect.x + 8, rect.y + 2, rect.width, 16), "INSPECTOR", EditorStyles.miniLabel);
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, 1, rect.height), new Color(0.2f, 0.2f, 0.28f));

        if (selectedPart == null)
        {
            GUI.Label(new Rect(rect.x + 8, rect.y + 28, rect.width - 16, 20), "파트를 선택하세요", EditorStyles.miniLabel);
            return;
        }

        bool isActive = activeParts.Contains(selectedPart);

        GUILayout.BeginArea(new Rect(rect.x + 4, rect.y + 22, rect.width - 8, rect.height - 26));
        inspScroll = GUILayout.BeginScrollView(inspScroll);

        GUIStyle nameStyle = new GUIStyle(EditorStyles.boldLabel);
        nameStyle.normal.textColor = new Color(0.65f, 0.55f, 1f);
        GUILayout.Label(selectedPart.name, nameStyle);

        GUIStyle subStyle = new GUIStyle(EditorStyles.miniLabel);
        subStyle.normal.textColor = new Color(0.3f, 0.3f, 0.45f);
        GUILayout.Label($"자식 {selectedPart.childCount}개", subStyle);
        GUILayout.Space(4);

        if (!isActive)
            EditorGUILayout.HelpBox("비활성 파트입니다.\n체크박스를 켜면 편집 가능해요.", MessageType.Warning);

        GUILayout.Space(4);
        EditorGUI.BeginDisabledGroup(!isActive);

        GUIStyle sectionStyle = new GUIStyle(EditorStyles.miniLabel);
        sectionStyle.normal.textColor = new Color(0.3f, 0.3f, 0.45f);
        GUILayout.Label("TRANSFORM", sectionStyle);

        // 수치 조절 → 미리보기 (키프레임 X)
        EditorGUI.BeginChangeCheck();
        Vector3 pos = EditorGUILayout.Vector3Field("Position", selectedPart.localPosition);
        Vector3 rot = EditorGUILayout.Vector3Field("Rotation", selectedPart.localEulerAngles);
        if (EditorGUI.EndChangeCheck() && isActive)
        {
            Undo.RecordObject(selectedPart, "Anim Editor Preview");
            selectedPart.localPosition = pos;
            selectedPart.localEulerAngles = rot;
            Repaint();
        }

        GUILayout.Space(8);

        // 이 파트만 저장
        if (GUILayout.Button("◆  이 파트만 저장", GUILayout.Height(24)))
            InsertKeyframe(selectedPart);

        EditorGUI.EndDisabledGroup();

        // 키프레임 목록
        if (keyframes.ContainsKey(selectedPart) && keyframes[selectedPart].Count > 0)
        {
            GUILayout.Space(10);
            GUILayout.Label("KEYFRAMES", sectionStyle);
            foreach (int f in keyframes[selectedPart].Keys.OrderBy(x => x).ToList())
            {
                GUILayout.BeginHorizontal();
                GUIStyle kfStyle = new GUIStyle(EditorStyles.miniLabel);
                kfStyle.normal.textColor = f == currentFrame ? new Color(0.65f, 0.55f, 1f) : new Color(0.3f, 0.3f, 0.5f);
                if (GUILayout.Button($"◆  {f + 1}f", kfStyle, GUILayout.Width(60))) { currentFrame = f; ApplyInterpolation(); }
                if (GUILayout.Button("✕", EditorStyles.miniButton, GUILayout.Width(20))) { keyframes[selectedPart].Remove(f); ApplyInterpolation(); }
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    // ── Timeline ───────────────────────────────────────────────────────────────
    void DrawTimeline(Rect rect)
    {
        EditorGUI.DrawRect(rect, new Color(0.10f, 0.10f, 0.14f));
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 18), new Color(0.09f, 0.09f, 0.12f));
        GUI.Label(new Rect(rect.x + 8, rect.y + 2, rect.width, 16), "TIMELINE  —  활성 파트만 표시", EditorStyles.miniLabel);

        if (activeParts.Count == 0)
        {
            GUI.Label(new Rect(rect.x + 8, rect.y + 24, rect.width, 20), "활성화된 파트가 없습니다", EditorStyles.miniLabel);
            return;
        }

        float totalW = TOTAL_FRAMES * FRAME_CELL_WIDTH;
        Rect scrollViewRect = new Rect(rect.x, rect.y + 18, rect.width, rect.height - 18);
        Rect contentRect = new Rect(0, 0, TRACK_LABEL_WIDTH + totalW, activeParts.Count * TRACK_ROW_HEIGHT + 4);
        tlScroll = GUI.BeginScrollView(scrollViewRect, tlScroll, contentRect);

        int rowIdx = 0;
        foreach (var t in activeParts)
        {
            float ry = rowIdx * TRACK_ROW_HEIGHT;
            bool isSel = selectedPart == t;

            EditorGUI.DrawRect(new Rect(0, ry, TRACK_LABEL_WIDTH + totalW, TRACK_ROW_HEIGHT),
                isSel ? new Color(0.14f, 0.14f, 0.26f)
                      : (rowIdx % 2 == 0 ? new Color(0.11f, 0.11f, 0.15f) : new Color(0.10f, 0.10f, 0.13f)));

            GUIStyle lStyle = new GUIStyle(EditorStyles.miniLabel);
            lStyle.normal.textColor = isSel ? new Color(0.65f, 0.55f, 1f) : new Color(0.35f, 0.35f, 0.5f);
            if (GUI.Button(new Rect(2, ry + 3, TRACK_LABEL_WIDTH - 4, TRACK_ROW_HEIGHT - 4), t.name, lStyle))
            { selectedPart = t; Selection.activeTransform = t; Repaint(); }

            HashSet<int> kfSet = keyframes.ContainsKey(t) ? new HashSet<int>(keyframes[t].Keys) : new HashSet<int>();

            for (int f = 0; f < TOTAL_FRAMES; f++)
            {
                float fx = TRACK_LABEL_WIDTH + f * FRAME_CELL_WIDTH;
                Rect cellRect = new Rect(fx, ry, FRAME_CELL_WIDTH, TRACK_ROW_HEIGHT);

                if (f == currentFrame) EditorGUI.DrawRect(cellRect, new Color(0.18f, 0.18f, 0.35f));
                if (f % 5 == 0) EditorGUI.DrawRect(new Rect(fx, ry, 1, TRACK_ROW_HEIGHT), new Color(0.2f, 0.2f, 0.28f));

                if (kfSet.Contains(f))
                {
                    float cx = fx + FRAME_CELL_WIDTH / 2f, cy = ry + TRACK_ROW_HEIGHT / 2f, s = 4f;
                    EditorGUI.DrawRect(new Rect(cx - s, cy - 1, s * 2, 2), new Color(0.94f, 0.63f, 0.19f));
                    EditorGUI.DrawRect(new Rect(cx - 1, cy - s, 2, s * 2), new Color(0.94f, 0.63f, 0.19f));
                }

                if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition))
                { currentFrame = f; ApplyInterpolation(); Event.current.Use(); Repaint(); }
            }

            float phX = TRACK_LABEL_WIDTH + currentFrame * FRAME_CELL_WIDTH + FRAME_CELL_WIDTH / 2f;
            EditorGUI.DrawRect(new Rect(phX - 1, ry, 2, TRACK_ROW_HEIGHT), new Color(0.55f, 0.45f, 1f, 0.8f));
            rowIdx++;
        }

        GUI.EndScrollView();
    }

    // ── 트리 수집 ──────────────────────────────────────────────────────────────
    void RefreshTree()
    {
        allTransforms.Clear(); activeParts.Clear(); keyframes.Clear();
        collapsed.Clear(); selectedPart = null; currentFrame = 0;
        if (rootObject != null) CollectTransforms(rootObject.transform);
    }

    void CollectTransforms(Transform t)
    {
        allTransforms.Add(t);
        for (int i = 0; i < t.childCount; i++) CollectTransforms(t.GetChild(i));
    }

    // ── 키프레임 저장 ──────────────────────────────────────────────────────────

    // 특정 파트 하나만 저장
    void InsertKeyframe(Transform t)
    {
        if (!keyframes.ContainsKey(t)) keyframes[t] = new Dictionary<int, FrameData>();
        keyframes[t][currentFrame] = new FrameData
        {
            localPosition = t.localPosition,
            localEulerAngles = t.localEulerAngles,
        };
        Repaint();
    }

    // ★ 활성 파트 전체를 현재 프레임에 한 번에 저장
    void SaveAllActiveParts()
    {
        foreach (var t in activeParts)
            InsertKeyframe(t);
        Debug.Log($"[AnimEditor] 프레임 {currentFrame + 1} — {activeParts.Count}개 파트 저장 완료");
    }

    // ── 보간 적용 ──────────────────────────────────────────────────────────────
    void ApplyInterpolation()
    {
        foreach (var t in allTransforms)
        {
            if (!keyframes.ContainsKey(t) || keyframes[t].Count == 0) continue;
            var kfs = keyframes[t];
            var sorted = kfs.Keys.OrderBy(f => f).ToList();
            var before = sorted.Where(f => f <= currentFrame).ToList();
            var after = sorted.Where(f => f > currentFrame).ToList();

            FrameData result;
            if (before.Count == 0) result = kfs[sorted[0]];
            else if (after.Count == 0) result = kfs[before[before.Count - 1]];
            else
            {
                int f0 = before[before.Count - 1], f1 = after[0];
                float r = (float)(currentFrame - f0) / (f1 - f0);
                var a = kfs[f0]; var b = kfs[f1];
                result = new FrameData
                {
                    localPosition = Vector3.Lerp(a.localPosition, b.localPosition, r),
                    localEulerAngles = Vector3.Lerp(a.localEulerAngles, b.localEulerAngles, r),
                };
            }

            Undo.RecordObject(t, "Anim Playback");
            t.localPosition = result.localPosition;
            t.localEulerAngles = result.localEulerAngles;
        }
        Repaint();
    }
}   