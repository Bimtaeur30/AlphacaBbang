// =============================================================
// StageMapConnectionGraphic.cs
// 위치: Assets/Scripts/UI/StageMapConnectionGraphic.cs
// Content 하위에 붙는 커스텀 UI 그래픽 (점선 + 화살표)
// =============================================================
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class StageMapConnectionGraphic : MaskableGraphic
{
    [System.Serializable]
    public class Connection
    {
        public RectTransform from;
        public RectTransform to;
        public bool arrowAtEnd;
    }

    [SerializeField] private float lineWidth = 3f;
    [SerializeField] private float dashLen = 16f;
    [SerializeField] private float gapLen = 8f;
    [SerializeField] private int bezierStep = 40;
    [SerializeField] private float arrowSize = 16f;


    // 직렬화용 연결 데이터 (from/to 오브젝트 이름으로 저장)
    [System.Serializable]
    public class ConnectionData
    {
        public string fromName;
        public string toName;
        public bool arrowAtEnd;
    }
    [SerializeField] private List<ConnectionData> _connData = new();

    private List<Connection> _conns = new();

    // 외부에서 현재 연결 목록 읽기 (씬에서 불러오기용)
    public List<Connection> GetConnections() => _conns ?? new();

    public void SetConnections(List<Connection> list)
    {
        _conns = list ?? new();

        // 직렬화용 데이터 저장 (오브젝트 이름 기반)
        _connData.Clear();
        foreach (var c in _conns)
        {
            if (c.from == null || c.to == null) continue;
            _connData.Add(new ConnectionData
            {
                fromName = c.from.gameObject.name,
                toName = c.to.gameObject.name,
                arrowAtEnd = c.arrowAtEnd
            });
        }

        SetVerticesDirty();

        // Update/LateUpdate 방식으로 동작하므로 별도 재시작 불필요
    }

    protected override void Awake()
    {
        base.Awake();
        RestoreFromData();
    }

    // _connData(직렬화된 이름 목록)로 _conns 복원
    private void RestoreFromData()
    {
        if (_connData == null || _connData.Count == 0) return;

        // Content 하위의 모든 RectTransform을 이름으로 검색
        var parent = transform.parent;
        if (parent == null) return;

        var rtMap = new Dictionary<string, RectTransform>();
        foreach (RectTransform rt in parent.GetComponentsInChildren<RectTransform>(true))
            if (!rtMap.ContainsKey(rt.gameObject.name))
                rtMap[rt.gameObject.name] = rt;

        _conns.Clear();
        foreach (var data in _connData)
        {
            if (!rtMap.TryGetValue(data.fromName, out var fromRt)) continue;
            if (!rtMap.TryGetValue(data.toName, out var toRt)) continue;
            _conns.Add(new Connection { from = fromRt, to = toRt, arrowAtEnd = data.arrowAtEnd });
        }

        SetVerticesDirty();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        foreach (var c in _conns)
        {
            if (c.from == null || c.to == null) continue;
            var s = ToLocal(BottomCenter(c.from));
            var e = ToLocal(TopCenter(c.to));
            DrawDashed(vh, s, e, c.arrowAtEnd);
        }
    }

    void DrawDashed(VertexHelper vh, Vector2 s, Vector2 e, bool arrow)
    {
        float dy = Mathf.Abs(e.y - s.y) * 0.4f;
        var c1 = s + new Vector2(0, dy);
        var c2 = e - new Vector2(0, dy);

        var pts = new Vector2[bezierStep + 1];
        var cum = new float[bezierStep + 1];
        for (int i = 0; i <= bezierStep; i++)
        {
            pts[i] = Bez(s, c1, c2, e, i / (float)bezierStep);
            if (i > 0) cum[i] = cum[i - 1] + Vector2.Distance(pts[i - 1], pts[i]);
        }
        float total = cum[bezierStep];
        if (total < 1f) return;

        float period = dashLen + gapLen;
        float arrowRes = arrow ? arrowSize * 1.3f : 0f;
        float drawn = 0f;   // 정적 점선 (offset 없음)
        bool inD = drawn < dashLen;
        int seg = 1;
        float travel = 0f;

        while (seg <= bezierStep && travel < total - arrowRes)
        {
            float ss = cum[seg - 1], se = cum[seg];
            float sl = se - ss;
            float ins = travel - ss;
            float rem = sl - ins;
            if (rem <= 0f) { seg++; continue; }
            float need = inD ? dashLen - drawn : gapLen - drawn;
            float step = Mathf.Min(need, Mathf.Min(rem, total - arrowRes - travel));
            if (step <= 0f) break;
            if (inD)
            {
                var a = Samp(pts, cum, ss + ins, total);
                var b = Samp(pts, cum, ss + ins + step, total);
                Quad(vh, a, b, lineWidth);
            }
            travel += step; drawn += step;
            if (drawn >= (inD ? dashLen : gapLen)) { drawn = 0f; inD = !inD; }
            if (rem <= step) seg++;
        }

        if (arrow)
        {
            var dir = (pts[bezierStep] - pts[bezierStep - 1]).normalized;
            Arrow(vh, e, dir);
        }
    }

    void Quad(VertexHelper vh, Vector2 a, Vector2 b, float w)
    {
        if ((b - a).sqrMagnitude < 0.001f) return;
        var d = (b - a).normalized;
        var p = new Vector2(-d.y, d.x) * (w * 0.5f);
        int i = vh.currentVertCount;
        var v = UIVertex.simpleVert; v.color = color;
        v.position = a - p; vh.AddVert(v);
        v.position = a + p; vh.AddVert(v);
        v.position = b + p; vh.AddVert(v);
        v.position = b - p; vh.AddVert(v);
        vh.AddTriangle(i, i + 1, i + 2); vh.AddTriangle(i, i + 2, i + 3);
    }

    void Arrow(VertexHelper vh, Vector2 tip, Vector2 dir)
    {
        var p = new Vector2(-dir.y, dir.x);
        var l = tip - dir * arrowSize + p * (arrowSize * 0.45f);
        var r = tip - dir * arrowSize - p * (arrowSize * 0.45f);
        Quad(vh, tip, l, lineWidth);
        Quad(vh, tip, r, lineWidth);
        Quad(vh, l, r, lineWidth * 0.8f);
    }

    Vector2 BottomCenter(RectTransform rt)
    {
        var c = new Vector3[4]; rt.GetWorldCorners(c);
        // corners: 0=BL 1=TL 2=TR 3=BR → 하단 중앙
        return (c[0] + c[3]) * 0.5f;
    }
    Vector2 TopCenter(RectTransform rt)
    {
        var c = new Vector3[4]; rt.GetWorldCorners(c);
        // corners: 0=BL 1=TL 2=TR 3=BR → 상단 중앙
        return (c[1] + c[2]) * 0.5f;
    }
    Vector2 ToLocal(Vector2 world)
    {
        // InverseTransformPoint: 월드 → 이 RectTransform 로컬
        var myRt = (RectTransform)transform;
        var lp = myRt.InverseTransformPoint(new Vector3(world.x, world.y, 0f));
        return new Vector2(lp.x, lp.y);
    }

    static Vector2 Samp(Vector2[] pts, float[] cum, float d, float total)
    {
        d = Mathf.Clamp(d, 0, total);
        for (int i = 1; i < cum.Length; i++)
            if (cum[i] >= d - 0.0001f)
                return Vector2.Lerp(pts[i - 1], pts[i],
                    Mathf.Clamp01((d - cum[i - 1]) / Mathf.Max(cum[i] - cum[i - 1], 0.0001f)));
        return pts[pts.Length - 1];
    }
    static Vector2 Bez(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float u = 1 - t;
        return u * u * u * p0 + 3 * u * u * t * p1 + 3 * u * t * t * p2 + t * t * t * p3;
    }
}
