using UnityEngine;
using System.Collections.Generic;

public class GrenadeTrail : MonoBehaviour
{
    [Header("Trail Settings")]
    public int maxPoints = 30;
    public float minDistance = 0.1f;

    [Header("Width Settings")]
    public float maxWidth = 0.15f;
    public float minWidth = 0.01f;

    private LineRenderer lineRenderer;
    private Queue<Vector3> points = new Queue<Vector3>();
    private Vector3 lastPos;


    void Awake()
    {
        lineRenderer = GetComponentInParent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer component is missing on GrenadeTrail.");
        }
    }
    void Start()
    {
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 0;
        lastPos = transform.position;

        SetWidthCurve();
    }

    void SetWidthCurve()
    {
        AnimationCurve curve = new AnimationCurve(
            new Keyframe(0f, minDistance),
            new Keyframe(1f, maxWidth)
        );
        lineRenderer.widthCurve = curve;
    }

    void Update()
    {
        Vector3 currentPos = transform.position;

        if (Vector3.Distance(currentPos, lastPos) >= minDistance)
        {
            points.Enqueue(currentPos);
            lastPos = currentPos;

            if (points.Count > maxPoints)
                points.Dequeue();

            Vector3[] posArray = points.ToArray();
            lineRenderer.positionCount = posArray.Length;
            lineRenderer.SetPositions(posArray);
        }
    }
}