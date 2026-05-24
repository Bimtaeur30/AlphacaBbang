using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("튜토리얼 데이터 (같은 인덱스 = 같은 스텝)")]
    public Transform[] zones;
    public string[] texts;

    [Header("UI")]
    public TextMeshProUGUI tutorialText;

    [Header("경로 라인")]
    public LineRenderer lineRenderer;
    public Color lineColor = Color.cyan;
    public float lineWidth = 0.1f;
    public float heightOffset = 0.05f;

    [Header("구역 표시")]
    public Color zoneColor = Color.cyan;
    public Vector2 zoneSize = new Vector2(3f, 3f);

    [Header("도착 판정 거리")]
    public float arriveDistance = 2f;

    [Header("완료 이벤트")]
    public UnityEvent OnAllComplete;

    private int currentIndex = 0;
    private bool arrived = false;
    private Transform player;
    private NavMeshPath path;
    private GameObject currentZoneIndicator;

    void Awake() => Instance = this;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        path = new NavMeshPath();

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        ShowStep(0);
    }
    void Update()
    {
        if (currentIndex >= zones.Length) return;

        if (!arrived)
        {
            UpdatePath();
            CheckArrival();
        }
        else
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                CompleteCurrentStep();
        }
    }
    void UpdatePath()
    {
        NavMesh.CalculatePath(player.position, zones[currentIndex].position, NavMesh.AllAreas, path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            lineRenderer.positionCount = path.corners.Length;
            for (int i = 0; i < path.corners.Length; i++)
            {
                Vector3 p = path.corners[i];
                p.y += heightOffset;
                lineRenderer.SetPosition(i, p);
            }
        }
    }

    void CheckArrival()
    {
        if (Vector3.Distance(player.position, zones[currentIndex].position) < arriveDistance)
            OnArrived();
    }

    void OnArrived()
    {
        arrived = true;
        lineRenderer.positionCount = 0;

        if (currentZoneIndicator != null) { Destroy(currentZoneIndicator); currentZoneIndicator = null; }

        if (currentIndex < texts.Length)
            tutorialText.text = texts[currentIndex];
    }

    public void CompleteCurrentStep()
    {
        if (!arrived) return;
        StartCoroutine(NextStep());
    }

    IEnumerator NextStep()
    {
        tutorialText.text = "완료!";
        yield return new WaitForSeconds(1.2f);

        int next = currentIndex + 1;
        if (next < zones.Length) ShowStep(next);
        else AllComplete();
    }

    void ShowStep(int index)
    {
        currentIndex = index;
        arrived = false;
        tutorialText.text = "";

        if (currentZoneIndicator != null) Destroy(currentZoneIndicator);
        currentZoneIndicator = CreateZoneIndicator(zones[index].position);
    }

    GameObject CreateZoneIndicator(Vector3 position)
    {
        GameObject obj = new GameObject("ZoneIndicator");
        LineRenderer lr = obj.AddComponent<LineRenderer>();
        lr.loop = true;
        lr.positionCount = 4;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = zoneColor;
        lr.endColor = zoneColor;

        float hw = zoneSize.x * 0.5f;
        float hd = zoneSize.y * 0.5f;
        lr.SetPositions(new Vector3[]
        {
            position + new Vector3(-hw, 0.05f,  hd),
            position + new Vector3( hw, 0.05f,  hd),
            position + new Vector3( hw, 0.05f, -hd),
            position + new Vector3(-hw, 0.05f, -hd),
        });

        return obj;
    }

    void AllComplete()
    {
        tutorialText.text = "";
        lineRenderer.positionCount = 0;
        OnAllComplete?.Invoke();
    }
}
