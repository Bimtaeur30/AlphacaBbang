// =============================================================
// StageMapEditor.cs
// 위치: Assets/Editor/StageMapEditor.cs
// Window > Stage Map Editor
// =============================================================
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StageMapEditor : EditorWindow
{
    // ──────────────────────────────────────────────────────────
    //  데이터 클래스
    // ──────────────────────────────────────────────────────────
    public enum Diff { Top, Middle, Bottom }

    [System.Serializable]
    public class Node
    {
        public int id;
        public string title = "게임 제목";
        public string sub = "게임 부제목 및 설명";
        public string method = "OnStageClick";
        public Vector2 pos = new(400, 300);
        public Vector2 size = new(180, 90);
        public Diff diff = Diff.Middle;
        public bool isStart;
        public bool isEnd;
        public Color color = new(0.1f, 0.1f, 0.12f, 1f);
        public Texture2D img = null;
    }

    [System.Serializable]
    public class Conn { public int from; public int to; }

    // ──────────────────────────────────────────────────────────
    //  상태 (SerializeField → 재컴파일 후에도 유지)
    // ──────────────────────────────────────────────────────────
    [SerializeField] private List<Node> _nodes = new();
    [SerializeField] private List<Conn> _conns = new();

    [SerializeField] private Texture2D _bgTex;
    [SerializeField] private GameObject _receiver;
    [SerializeField] private Canvas _canvas;

    [SerializeField] private float _lineTop = 800f;
    [SerializeField] private float _lineBottom = 1600f;

    private Node _sel;
    private Node _drag;
    private Vector2 _dragOff;
    private bool _connMode;
    private int _connFrom = -1;

    private Vector2 _scroll;
    private float _dashOff;
    private double _lastT;

    // 캔버스 고정 크기
    private const float CW = 800f;
    private const float CH = 2400f;

    private const float LP = 220f;   // 왼쪽 패널 너비
    private const float RP = 240f;   // 오른쪽 패널 너비

    // ──────────────────────────────────────────────────────────
    //  메뉴 / 라이프사이클
    // ──────────────────────────────────────────────────────────
    [MenuItem("Window/Stage Map Editor")]
    public static void Open()
    {
        var w = GetWindow<StageMapEditor>("Stage Map Editor");
        w.minSize = new Vector2(960, 600);
    }

    private void OnEnable()
    {
        _lastT = EditorApplication.timeSinceStartup;
        EditorApplication.update += Tick;
    }
    private void OnDisable() => EditorApplication.update -= Tick;

    private void Tick()
    {
        double now = EditorApplication.timeSinceStartup;
        _dashOff += (float)(now - _lastT) * 40f;
        if (_dashOff > 26f) _dashOff -= 26f;
        _lastT = now;
        Repaint();
    }

    // ──────────────────────────────────────────────────────────
    //  OnGUI
    // ──────────────────────────────────────────────────────────
    private void OnGUI()
    {
        DrawToolbar();

        float top = 26f, h = position.height - top;

        var lRect = new Rect(0, top, LP, h);
        var cRect = new Rect(LP, top, position.width - LP - RP, h);
        var rRect = new Rect(position.width - RP, top, RP, h);

        EditorGUI.DrawRect(new Rect(LP - 1, top, 1, h), Color.black);
        EditorGUI.DrawRect(new Rect(position.width - RP, top, 1, h), Color.black);

        DrawLeft(lRect);
        DrawCanvas(cRect);
        DrawRight(rRect);

        if (Event.current.type == EventType.KeyDown &&
            Event.current.keyCode == KeyCode.Escape)
        { _connMode = false; _connFrom = -1; Event.current.Use(); }
    }

    // ──────────────────────────────────────────────────────────
    //  툴바
    // ──────────────────────────────────────────────────────────
    private void DrawToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("+ 스테이지", EditorStyles.toolbarButton, GUILayout.Width(80)))
            AddNode();

        GUI.color = _connMode ? new Color(1f, 0.9f, 0.2f) : Color.white;
        string lbl = _connMode
            ? (_connFrom == -1 ? "[ 시작 클릭 ]" : "[ 끝 클릭 ]")
            : "연결 모드";
        if (GUILayout.Button(lbl, EditorStyles.toolbarButton, GUILayout.Width(120)))
        { _connMode = !_connMode; _connFrom = -1; }
        GUI.color = Color.white;

        if (GUILayout.Button("선택 연결 삭제", EditorStyles.toolbarButton, GUILayout.Width(100)))
        { if (_sel != null) _conns.RemoveAll(c => c.from == _sel.id || c.to == _sel.id); }

        if (GUILayout.Button("전체 연결 삭제", EditorStyles.toolbarButton, GUILayout.Width(100)))
        { if (EditorUtility.DisplayDialog("확인", "모든 연결을 삭제합니다.", "삭제", "취소")) _conns.Clear(); }

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("씬에서 불러오기", EditorStyles.toolbarButton, GUILayout.Width(110)))
            LoadFromScene();

        if (GUILayout.Button("씬에 생성", EditorStyles.toolbarButton, GUILayout.Width(90)))
            Build();

        GUILayout.EndHorizontal();
    }

    // ──────────────────────────────────────────────────────────
    //  왼쪽 패널
    // ──────────────────────────────────────────────────────────
    private void DrawLeft(Rect r)
    {
        GUILayout.BeginArea(r);
        GUILayout.Space(6);
        EditorGUILayout.LabelField("⚙ 전역 설정", EditorStyles.boldLabel);

        _canvas = (Canvas)EditorGUILayout.ObjectField("Canvas", _canvas, typeof(Canvas), true);
        _bgTex = (Texture2D)EditorGUILayout.ObjectField("배경 이미지", _bgTex, typeof(Texture2D), false);
        _receiver = (GameObject)EditorGUILayout.ObjectField("이벤트 수신 오브젝트", _receiver, typeof(GameObject), true);

        GUILayout.Space(8);
        EditorGUILayout.LabelField("난이도 경계선 Y", EditorStyles.boldLabel);
        _lineTop = EditorGUILayout.Slider("상/중", _lineTop, 100f, _lineBottom - 100f);
        _lineBottom = EditorGUILayout.Slider("중/하", _lineBottom, _lineTop + 100f, CH - 100f);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("스테이지 목록", EditorStyles.boldLabel);
        foreach (var n in _nodes)
        {
            GUI.color = _sel == n ? new Color(1f, 0.85f, 0.3f) : Color.white;
            if (GUILayout.Button($"[{n.id}] {n.title}", GUILayout.Height(22))) _sel = n;
            GUI.color = Color.white;
        }
        GUILayout.EndArea();
    }

    // ──────────────────────────────────────────────────────────
    //  오른쪽 패널
    // ──────────────────────────────────────────────────────────
    private void DrawRight(Rect r)
    {
        GUILayout.BeginArea(r);
        GUILayout.Space(6);
        EditorGUILayout.LabelField("📋 노드 속성", EditorStyles.boldLabel);

        if (_sel == null)
        {
            EditorGUILayout.HelpBox("노드를 선택하세요.", MessageType.Info);
            GUILayout.EndArea(); return;
        }

        var n = _sel;
        GUILayout.Space(4);

        EditorGUILayout.LabelField("기본", EditorStyles.miniBoldLabel);
        n.title = EditorGUILayout.TextField("제목", n.title);
        n.sub = EditorGUILayout.TextField("부제목", n.sub);
        n.method = EditorGUILayout.TextField("클릭 메소드", n.method);

        GUILayout.Space(6);
        EditorGUILayout.LabelField("위치 / 크기", EditorStyles.miniBoldLabel);
        n.pos = EditorGUILayout.Vector2Field("위치", n.pos);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("너비", GUILayout.Width(34));
        n.size.x = EditorGUILayout.Slider(n.size.x, 60f, 320f);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("높이", GUILayout.Width(34));
        n.size.y = EditorGUILayout.Slider(n.size.y, 40f, 200f);
        GUILayout.EndHorizontal();
        n.size.x = Mathf.Max(60f, n.size.x);
        n.size.y = Mathf.Max(40f, n.size.y);

        GUILayout.Space(6);
        EditorGUILayout.LabelField("비주얼", EditorStyles.miniBoldLabel);
        n.color = EditorGUILayout.ColorField("색상", n.color);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("버튼 이미지", GUILayout.Width(74));
        n.img = (Texture2D)EditorGUILayout.ObjectField(
            n.img, typeof(Texture2D), false,
            GUILayout.Height(56), GUILayout.Width(RP - 84));
        GUILayout.EndHorizontal();

        GUILayout.Space(6);
        EditorGUILayout.LabelField("속성", EditorStyles.miniBoldLabel);
        n.diff = (Diff)EditorGUILayout.EnumPopup("난이도", n.diff);
        n.isStart = EditorGUILayout.Toggle("시작 노드", n.isStart);
        n.isEnd = EditorGUILayout.Toggle("끝 노드(화살표)", n.isEnd);

        GUILayout.Space(6);
        int outC = _conns.FindAll(c => c.from == n.id).Count;
        int inC = _conns.FindAll(c => c.to == n.id).Count;
        EditorGUILayout.LabelField($"나가는 연결:{outC}  들어오는:{inC}", EditorStyles.miniLabel);

        GUILayout.Space(10);
        GUI.color = new Color(1f, 0.35f, 0.35f);
        if (GUILayout.Button("🗑 노드 삭제", GUILayout.Height(28))) DelNode(n);
        GUI.color = Color.white;

        GUILayout.EndArea();
    }

    // ──────────────────────────────────────────────────────────
    //  캔버스
    // ──────────────────────────────────────────────────────────
    private void DrawCanvas(Rect panel)
    {
        // 스크롤뷰 (에디터 미리보기)
        var view = new Rect(0, 0, CW, CH);
        _scroll = GUI.BeginScrollView(panel, _scroll, view, false, true);

        // 배경
        if (_bgTex != null)
            GUI.DrawTexture(view, _bgTex, ScaleMode.StretchToFill);
        else
            EditorGUI.DrawRect(view, new Color(0.74f, 0.62f, 0.43f));

        DrawZones();
        DrawDivLines();
        DrawConns();
        DrawNodes();
        BgClick();

        GUI.EndScrollView();
    }

    // 난이도 색상 구역
    private void DrawZones()
    {
        EditorGUI.DrawRect(new Rect(0, 0, CW, _lineTop),
            new Color(0.9f, 0.2f, 0.2f, 0.07f));
        EditorGUI.DrawRect(new Rect(0, _lineTop, CW, _lineBottom - _lineTop),
            new Color(0.95f, 0.85f, 0.1f, 0.07f));
        EditorGUI.DrawRect(new Rect(0, _lineBottom, CW, CH - _lineBottom),
            new Color(0.2f, 0.85f, 0.3f, 0.07f));

        var s = new GUIStyle(EditorStyles.boldLabel)
        { fontSize = 11, normal = { textColor = new Color(1, 1, 1, 0.4f) } };
        GUI.Label(new Rect(10, 8, 140, 22), "상 난이도", s);
        GUI.Label(new Rect(10, _lineTop + 8, 140, 22), "중 난이도", s);
        GUI.Label(new Rect(10, _lineBottom + 8, 140, 22), "하 난이도", s);
    }

    // 경계 점선 (흐르는 애니메이션)
    private void DrawDivLines()
    {
        DashedH(_lineTop, new Color(1f, 0.4f, 0.4f, 0.9f));
        DashedH(_lineBottom, new Color(0.4f, 0.95f, 0.4f, 0.9f));
    }

    private void DashedH(float y, Color col)
    {
        const float d = 16f, g = 10f;
        float start = -(_dashOff % (d + g));
        for (float x = start; x < CW; x += d + g)
        {
            float x0 = Mathf.Max(x, 0), x1 = Mathf.Min(x + d, CW);
            if (x1 > 0) Line(new Vector2(x0, y), new Vector2(x1, y), col, 2f);
        }
    }

    // 연결선
    private void DrawConns()
    {
        foreach (var c in _conns)
        {
            var f = _nodes.Find(n => n.id == c.from);
            var t = _nodes.Find(n => n.id == c.to);
            if (f == null || t == null) continue;

            bool hi = _sel != null && (_sel.id == c.from || _sel.id == c.to);
            var col = hi ? new Color(1f, 0.9f, 0.3f) : new Color(1f, 1f, 1f, 0.85f);

            var s = f.pos + new Vector2(0, f.size.y * 0.5f);
            var e = t.pos - new Vector2(0, t.size.y * 0.5f);
            DashedCurve(s, e, col, t.isEnd);
        }
    }

    private void DashedCurve(Vector2 s, Vector2 e, Color col, bool arrow)
    {
        const int steps = 48;
        const float d = 14f, g = 8f;

        float dy = Mathf.Abs(e.y - s.y) * 0.45f;
        var c1 = s + new Vector2(0, dy);
        var c2 = e - new Vector2(0, dy);

        var pts = new Vector2[steps + 1];
        var cum = new float[steps + 1];
        for (int i = 0; i <= steps; i++)
        {
            pts[i] = Bez(s, c1, c2, e, i / (float)steps);
            if (i > 0) cum[i] = cum[i - 1] + Vector2.Distance(pts[i - 1], pts[i]);
        }
        float total = cum[steps];
        if (total < 0.01f) return;

        float drawn = _dashOff % (d + g);
        bool inD = drawn < d;
        int seg = 1;
        float t = 0f;

        while (seg <= steps && t < total)
        {
            float ss = cum[seg - 1], se = cum[seg];
            float sl = se - ss, ins = t - ss, rem = sl - ins;
            if (rem <= 0f) { seg++; continue; }
            float need = inD ? d - drawn : g - drawn;
            float step = Mathf.Min(need, rem);
            if (inD)
            {
                var a = SampCurve(pts, cum, ss + ins, total);
                var b = SampCurve(pts, cum, ss + ins + step, total);
                Line(a, b, col, 2.5f);
            }
            t += step; drawn += step;
            if (drawn >= (inD ? d : g)) { drawn = 0f; inD = !inD; }
            if (rem <= step) seg++;
        }

        if (arrow)
        {
            var dir = (pts[steps] - pts[steps - 1]).normalized;
            var p = new Vector2(-dir.y, dir.x);
            Line(e, e - dir * 14f + p * 7f, col, 2.5f);
            Line(e, e - dir * 14f - p * 7f, col, 2.5f);
        }
    }

    // 노드
    private void DrawNodes()
    {
        var ev = Event.current;

        foreach (var n in _nodes)
        {
            var r = new Rect(
                n.pos.x - n.size.x * 0.5f,
                n.pos.y - n.size.y * 0.5f,
                n.size.x, n.size.y);

            // 그림자
            EditorGUI.DrawRect(new Rect(r.x + 4, r.y + 4, r.width, r.height),
                new Color(0, 0, 0, 0.45f));

            // 본체
            if (n.img != null)
            {
                GUI.DrawTexture(r, n.img, ScaleMode.StretchToFill);
                EditorGUI.DrawRect(r, new Color(n.color.r, n.color.g, n.color.b,
                    Mathf.Clamp01(n.color.a * 0.4f)));
            }
            else EditorGUI.DrawRect(r, n.color);

            // 테두리
            bool isSel = _sel == n, isFrom = _connFrom == n.id;
            var bCol = isSel ? new Color(1f, 0.85f, 0.2f) : isFrom ? Color.green : new Color(1, 1, 1, 0.2f);
            RectBorder(r, bCol, isSel || isFrom ? 3f : 1f);

            // 텍스트
            int ts = Mathf.Clamp((int)(n.size.y * 0.22f), 11, 20);
            int ss = Mathf.Clamp((int)(n.size.y * 0.13f), 8, 13);
            var tStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = ts,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = false,
                normal = { textColor = Color.white }
            };
            var sStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = ss,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = false,
                normal = { textColor = new Color(1, 1, 1, 0.7f) }
            };

            GUI.Label(new Rect(r.x, r.y, r.width, r.height * 0.55f), n.title, tStyle);
            GUI.Label(new Rect(r.x, r.y + r.height * 0.40f, r.width, r.height * 0.45f), n.sub, sStyle);

            if (n.isStart)
            {
                var b = new GUIStyle { fontSize = 8, normal = { textColor = new Color(0.3f, 1f, 0.5f) } };
                GUI.Label(new Rect(r.x + 3, r.y + 2, 50, 14), "▶START", b);
            }
            if (n.isEnd)
            {
                var b = new GUIStyle
                {
                    fontSize = 8,
                    alignment = TextAnchor.UpperRight,
                    normal = { textColor = new Color(1f, 0.4f, 0.4f) }
                };
                GUI.Label(new Rect(r.xMax - 48, r.y + 2, 46, 14), "END◀", b);
            }
            if (!string.IsNullOrWhiteSpace(n.method))
            {
                var b = new GUIStyle
                {
                    fontSize = 7,
                    alignment = TextAnchor.LowerCenter,
                    normal = { textColor = new Color(0.6f, 1f, 0.6f, 0.8f) }
                };
                GUI.Label(new Rect(r.x, r.yMax - 15, r.width, 14), $"⚡ {n.method}", b);
            }

            // 마우스
            if (ev.type == EventType.MouseDown && ev.button == 0 && r.Contains(ev.mousePosition))
            {
                if (_connMode) HandleConn(n);
                else { _sel = n; _drag = n; _dragOff = ev.mousePosition - n.pos; GUI.FocusControl(null); }
                ev.Use();
            }
        }

        // 드래그
        if (_drag != null)
        {
            if (ev.type == EventType.MouseDrag)
            {
                _drag.pos = ev.mousePosition - _dragOff;
                _drag.pos.x = Mathf.Clamp(_drag.pos.x, _drag.size.x * 0.5f, CW - _drag.size.x * 0.5f);
                _drag.pos.y = Mathf.Clamp(_drag.pos.y, _drag.size.y * 0.5f, CH - _drag.size.y * 0.5f);
                ev.Use();
            }
            if (ev.type == EventType.MouseUp) _drag = null;
        }
    }

    private void BgClick()
    {
        var ev = Event.current;
        if (ev.type != EventType.MouseDown || ev.button != 0) return;
        foreach (var n in _nodes)
        {
            var r = new Rect(n.pos.x - n.size.x * 0.5f, n.pos.y - n.size.y * 0.5f, n.size.x, n.size.y);
            if (r.Contains(ev.mousePosition)) return;
        }
        _sel = null;
    }

    // ──────────────────────────────────────────────────────────
    //  연결 / 노드 CRUD
    // ──────────────────────────────────────────────────────────
    private void HandleConn(Node n)
    {
        if (_connFrom == -1) { _connFrom = n.id; return; }
        if (_connFrom == n.id) return;
        if (!_conns.Exists(c => c.from == _connFrom && c.to == n.id))
            _conns.Add(new Conn { from = _connFrom, to = n.id });
        _connFrom = -1; _connMode = false;
    }

    private void AddNode()
    {
        int id = 0;
        while (_nodes.Exists(n => n.id == id)) id++;
        var node = new Node { id = id, pos = new Vector2(CW * 0.5f, 200f + _nodes.Count * 200f) };
        _nodes.Add(node); _sel = node;
    }

    private void DelNode(Node n)
    {
        _conns.RemoveAll(c => c.from == n.id || c.to == n.id);
        _nodes.Remove(n);
        if (_sel == n) _sel = null;
    }

    // ──────────────────────────────────────────────────────────
    //  씬에서 불러오기
    // ──────────────────────────────────────────────────────────
    private void LoadFromScene()
    {
        // 불러올 Canvas 확인
        Canvas cv = _canvas;
        if (cv == null && Selection.activeGameObject != null)
            cv = Selection.activeGameObject.GetComponentInParent<Canvas>();
        if (cv == null)
            cv = FindObjectOfType<Canvas>();

        if (cv == null)
        {
            EditorUtility.DisplayDialog("실패", "Canvas를 찾을 수 없습니다.\n왼쪽 패널에서 Canvas를 지정해주세요.", "확인");
            return;
        }

        // StageMapRoot 찾기
        Transform root = cv.transform.Find("StageMapRoot");
        if (root == null)
        {
            EditorUtility.DisplayDialog("실패",
                "씬에서 StageMapRoot를 찾을 수 없습니다.\n먼저 씬에 생성을 해주세요.", "확인");
            return;
        }

        Transform content = root.Find("Viewport/Content");
        if (content == null)
        {
            EditorUtility.DisplayDialog("실패", "Content 오브젝트를 찾을 수 없습니다.", "확인");
            return;
        }

        // 기존 데이터 초기화
        if (!EditorUtility.DisplayDialog("씬에서 불러오기",
            "현재 에디터 데이터를 지우고 씬에서 불러옵니다.\n계속하시겠습니까?",
            "불러오기", "취소"))
            return;

        _nodes.Clear();
        _conns.Clear();
        _sel = null;

        // Content RectTransform 크기로 캔버스 크기 확인
        var contentRt = content.GetComponent<RectTransform>();

        // StageMapButton 컴포넌트가 있는 자식들 수집
        var buttons = content.GetComponentsInChildren<StageMapButton>(true);
        var rtMap = new Dictionary<int, Node>();

        foreach (var btn in buttons)
        {
            var rt = btn.GetComponent<RectTransform>();
            var img = btn.GetComponent<UnityEngine.UI.Image>();

            // anchoredPosition → 에디터 캔버스 좌표로 역변환
            // Build()에서: anchoredPos.x = node.pos.x - CW*0.5  → node.pos.x = anchoredPos.x + CW*0.5
            //              anchoredPos.y = -node.pos.y           → node.pos.y = -anchoredPos.y
            float cx = contentRt.sizeDelta.x > 0 ? contentRt.sizeDelta.x : CW;
            float posX = rt.anchoredPosition.x + cx * 0.5f;
            float posY = -rt.anchoredPosition.y;

            // 텍스트 읽기
            string title = GetChildText(btn.transform, "Title");
            string sub = GetChildText(btn.transform, "Subtitle");
            if (string.IsNullOrEmpty(title)) title = btn.gameObject.name;

            // isStart / isEnd 판별 (이름 또는 별도 태그)
            bool isStart = btn.gameObject.name.Contains("START") ||
                           HasChildLabel(btn.transform, "▶START");
            bool isEnd = btn.gameObject.name.Contains("END") ||
                           HasChildLabel(btn.transform, "END◀");

            var node = new Node
            {
                id = btn.StageId,
                title = title,
                sub = sub,
                method = btn.MethodName,
                pos = new Vector2(posX, posY),
                size = rt.sizeDelta,
                color = img != null ? img.color : new Color(0.1f, 0.1f, 0.12f, 1f),
                isStart = isStart,
                isEnd = isEnd,
            };

            // 이미지 스프라이트가 있으면 Texture2D로 복원
            if (img != null && img.sprite != null)
                node.img = img.sprite.texture;

            _nodes.Add(node);
            rtMap[btn.StageId] = node;
        }

        // 연결선 복원: StageMapConnectionGraphic에서 읽기
        var connGraphic = content.GetComponentInChildren<StageMapConnectionGraphic>(true);
        if (connGraphic != null)
        {
            var rConns = connGraphic.GetConnections();
            foreach (var rc in rConns)
            {
                if (rc.from == null || rc.to == null) continue;
                var fromBtn = rc.from.GetComponent<StageMapButton>();
                var toBtn = rc.to.GetComponent<StageMapButton>();
                if (fromBtn == null || toBtn == null) continue;
                _conns.Add(new Conn { from = fromBtn.StageId, to = toBtn.StageId });
            }
        }

        // 이벤트 수신 오브젝트 복원
        if (buttons.Length > 0 && buttons[0].EventReceiver != null)
            _receiver = buttons[0].EventReceiver;

        _nodes.Sort((a, b) => a.id.CompareTo(b.id));

        Debug.Log($"[StageMapEditor] 씬에서 불러오기 완료 — 노드:{_nodes.Count} 연결:{_conns.Count}");
        EditorUtility.DisplayDialog("완료",
            $"불러오기 완료!\n노드 {_nodes.Count}개 / 연결 {_conns.Count}개", "확인");
    }

    // 텍스트 자식 읽기
    private static string GetChildText(Transform parent, string childName)
    {
        var child = parent.Find(childName);
        if (child == null) return "";
        var t = child.GetComponent<UnityEngine.UI.Text>();
        return t != null ? t.text : "";
    }

    // 자식 라벨 존재 여부 확인
    private static bool HasChildLabel(Transform parent, string contains)
    {
        foreach (Transform child in parent)
        {
            var t = child.GetComponent<UnityEngine.UI.Text>();
            if (t != null && t.text.Contains(contains)) return true;
        }
        return false;
    }

    // ──────────────────────────────────────────────────────────
    //  씬 생성 (Build)
    // ──────────────────────────────────────────────────────────
    private void Build()
    {
        // 직렬화 체크용 로그
        Debug.Log($"[StageMapEditor] Build() 호출 — 노드 수: {(_nodes == null ? "null" : _nodes.Count.ToString())}");

        if (_nodes == null || _nodes.Count == 0)
        {
            EditorUtility.DisplayDialog("실패",
                $"노드가 없습니다 (현재 {(_nodes == null ? 0 : _nodes.Count)}개)\n" +
                "에디터 창을 닫지 말고 + 스테이지로 추가 후 바로 씬에 생성하세요.",
                "확인");
            return;
        }

        // ── Canvas 확보
        Canvas cv = _canvas;
        if (cv == null && Selection.activeGameObject != null)
            cv = Selection.activeGameObject.GetComponentInParent<Canvas>();
        if (cv == null)
        {
            var go = new GameObject("StageMapCanvas");
            cv = go.AddComponent<Canvas>();
            cv.renderMode = RenderMode.ScreenSpaceOverlay;
            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();
            _canvas = cv;
        }

        // 기존 제거
        var old = cv.transform.Find("StageMapRoot");
        if (old != null) DestroyImmediate(old.gameObject);

        // ── Root (ScrollView)
        var rootGO = MakeGO("StageMapRoot", cv.transform);
        SetStretch(rootGO.GetComponent<RectTransform>());
        rootGO.AddComponent<Image>().color = Color.clear;      // 투명, Mask용

        var sr = rootGO.AddComponent<ScrollRect>();
        sr.horizontal = false;
        sr.vertical = true;
        sr.movementType = ScrollRect.MovementType.Clamped;
        sr.scrollSensitivity = 30f;
        sr.inertia = true;
        sr.decelerationRate = 0.135f;

        // ── Viewport
        var vpGO = MakeGO("Viewport", rootGO.transform);
        SetStretch(vpGO.GetComponent<RectTransform>());
        vpGO.AddComponent<Image>().color = Color.white;         // Mask 필수
        vpGO.AddComponent<Mask>().showMaskGraphic = false;
        sr.viewport = vpGO.GetComponent<RectTransform>();

        // ── Content
        //    anchorMin=(0.5,1) anchorMax=(0.5,1) pivot=(0.5,1)
        //    sizeDelta=(CW, CH) → 고정 크기
        var ctGO = MakeGO("Content", vpGO.transform);
        var ctRt = ctGO.GetComponent<RectTransform>();
        ctRt.anchorMin = new Vector2(0.5f, 1f);
        ctRt.anchorMax = new Vector2(0.5f, 1f);
        ctRt.pivot = new Vector2(0.5f, 1f);
        ctRt.anchoredPosition = Vector2.zero;
        ctRt.sizeDelta = new Vector2(CW, CH);
        sr.content = ctRt;

        // ── 배경
        var bgGO = MakeGO("Background", ctGO.transform);
        var bgRt = bgGO.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.raycastTarget = false;
        if (_bgTex != null)
        {
            bgImg.sprite = Sprite.Create(_bgTex,
                new Rect(0, 0, _bgTex.width, _bgTex.height), new Vector2(0.5f, 0.5f));
            bgImg.type = Image.Type.Simple;
            bgImg.preserveAspect = false;
        }
        else bgImg.color = new Color(0.74f, 0.62f, 0.43f);
        bgGO.transform.SetAsFirstSibling();

        // ── 난이도 구분선
        // Content 기준: anchorMin/Max Y=1, anchoredPosition.y = -y
        MakeDivLine(ctGO.transform, "DiffLine_Top", _lineTop);
        MakeDivLine(ctGO.transform, "DiffLine_Bottom", _lineBottom);

        // ── ConnectionLayer (Content 전체 stretch)
        var clGO = MakeGO("ConnectionLayer", ctGO.transform);
        var clRt = clGO.GetComponent<RectTransform>();
        clRt.anchorMin = Vector2.zero;
        clRt.anchorMax = Vector2.one;
        clRt.offsetMin = Vector2.zero;
        clRt.offsetMax = Vector2.zero;
        var cg = clGO.AddComponent<StageMapConnectionGraphic>();
        cg.raycastTarget = false;
        cg.color = Color.white;

        // ── 버튼 생성
        //    Content pivot=(0.5,1) → anchoredPosition.x = node.pos.x - CW*0.5  (중앙=0)
        //                            anchoredPosition.y = -node.pos.y
        var btnMap = new Dictionary<int, RectTransform>();
        foreach (var n in _nodes)
        {
            var bGO = MakeGO($"Stage_{n.id}_{n.title}", ctGO.transform);
            var bRt = bGO.GetComponent<RectTransform>();
            bRt.anchorMin = new Vector2(0.5f, 1f);
            bRt.anchorMax = new Vector2(0.5f, 1f);
            bRt.pivot = new Vector2(0.5f, 0.5f);
            bRt.sizeDelta = n.size;
            bRt.anchoredPosition = new Vector2(n.pos.x - CW * 0.5f, -n.pos.y);

            // Image
            var bImg = bGO.AddComponent<Image>();
            if (n.img != null)
            {
                bImg.sprite = Sprite.Create(n.img,
                    new Rect(0, 0, n.img.width, n.img.height), new Vector2(0.5f, 0.5f));
                bImg.type = Image.Type.Simple;
                bImg.preserveAspect = false;
                bImg.color = new Color(n.color.r, n.color.g, n.color.b,
                    Mathf.Clamp01(n.color.a * 0.4f));
            }
            else bImg.color = n.color;

            // Button
            bGO.AddComponent<Button>();

            // StageMapButton
            var smb = bGO.AddComponent<StageMapButton>();
            smb.Init(n.id, n.method, _receiver);

            // 텍스트
            MakeText(bGO.transform, "Title", n.title, 16, FontStyle.Bold,
                new Vector2(0, 0.42f), Vector2.one);
            MakeText(bGO.transform, "Subtitle", n.sub, 11, FontStyle.Normal,
                Vector2.zero, new Vector2(1, 0.56f));

            btnMap[n.id] = bRt;
        }

        // ── 연결선 데이터 전달
        var rConns = new List<StageMapConnectionGraphic.Connection>();
        foreach (var c in _conns)
        {
            if (!btnMap.TryGetValue(c.from, out var fRt)) continue;
            if (!btnMap.TryGetValue(c.to, out var tRt)) continue;
            var tn = _nodes.Find(n => n.id == c.to);
            rConns.Add(new StageMapConnectionGraphic.Connection
            { from = fRt, to = tRt, arrowAtEnd = tn != null && tn.isEnd });
        }
        cg.SetConnections(rConns);

        Selection.activeGameObject = rootGO;
        Debug.Log($"[StageMapEditor] 생성 완료 — 노드:{_nodes.Count} 연결:{_conns.Count}");
        EditorUtility.DisplayDialog("완료",
            $"생성 완료!\n노드 {_nodes.Count}개 / 연결 {_conns.Count}개", "확인");
    }

    // ──────────────────────────────────────────────────────────
    //  씬 빌드 헬퍼
    // ──────────────────────────────────────────────────────────
    private static GameObject MakeGO(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private static void SetStretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    private static void MakeDivLine(Transform parent, string name, float y)
    {
        // 가로 stretch, 높이 2px
        var go = MakeGO(name, parent);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(0f, 2f);
        rt.anchoredPosition = new Vector2(0f, -y);
        var img = go.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.45f);
        img.raycastTarget = false;
    }

    private static void MakeText(Transform parent, string name, string text,
        int size, FontStyle style, Vector2 aMin, Vector2 aMax)
    {
        var go = MakeGO(name, parent);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = aMin; rt.anchorMax = aMax;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        var t = go.AddComponent<Text>();
        t.text = text;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = size;
        t.fontStyle = style;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.raycastTarget = false;
    }

    // ──────────────────────────────────────────────────────────
    //  수학 / 그리기 유틸
    // ──────────────────────────────────────────────────────────
    private static Vector2 Bez(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float u = 1 - t;
        return u * u * u * p0 + 3 * u * u * t * p1 + 3 * u * t * t * p2 + t * t * t * p3;
    }

    private static Vector2 SampCurve(Vector2[] pts, float[] cum, float d, float total)
    {
        d = Mathf.Clamp(d, 0, total);
        for (int i = 1; i < cum.Length; i++)
            if (cum[i] >= d - 0.0001f)
                return Vector2.Lerp(pts[i - 1], pts[i],
                    Mathf.Clamp01((d - cum[i - 1]) / Mathf.Max(cum[i] - cum[i - 1], 0.0001f)));
        return pts[pts.Length - 1];
    }

    private void Line(Vector2 a, Vector2 b, Color col, float w)
    {
        Handles.BeginGUI(); Handles.color = col;
        Handles.DrawAAPolyLine(w, a, b);
        Handles.EndGUI();
    }

    private void RectBorder(Rect r, Color col, float w)
    {
        Line(new Vector2(r.xMin, r.yMin), new Vector2(r.xMax, r.yMin), col, w);
        Line(new Vector2(r.xMax, r.yMin), new Vector2(r.xMax, r.yMax), col, w);
        Line(new Vector2(r.xMax, r.yMax), new Vector2(r.xMin, r.yMax), col, w);
        Line(new Vector2(r.xMin, r.yMax), new Vector2(r.xMin, r.yMin), col, w);
    }
}
#endif