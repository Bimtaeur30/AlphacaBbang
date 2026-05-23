using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MapPathRecorder : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Record Settings")]
    [SerializeField] private float recordInterval = 0.5f;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.positionCount = 0;
        _lineRenderer.useWorldSpace = true;
    }

    private void OnEnable()
    {
        StartCoroutine(RecordPathRoutine());
    }

    private IEnumerator RecordPathRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(recordInterval);

        while (true)
        {
            AddPoint();
            yield return wait;
        }
    }

    private void AddPoint()
    {
        if (target == null)
            return;

        Vector3 pos = target.position;

        // yÃà ¹«½Ã
        pos.y = 0f;
        gameObject.transform.position = pos;

        _lineRenderer.positionCount++;

        _lineRenderer.SetPosition(
            _lineRenderer.positionCount - 1,
            pos
        );
    }
}