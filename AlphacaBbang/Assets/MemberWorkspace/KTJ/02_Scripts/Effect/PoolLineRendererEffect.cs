using JJH._02_Scripts.Systems.ObjectPoolSystems;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PoolLineRendererEffect : PoolableMono
{
    private LineRenderer _lineRenderer;
    [SerializeField] private PoolManagerSO poolManager;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public IEnumerator DrawLineRenderer(Vector3 origin, Vector3 endPoint, float time)
    {
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, origin);
        _lineRenderer.SetPosition(1, endPoint);

        yield return new WaitForSeconds(time);

        _lineRenderer.positionCount = 0;
        poolManager.Push(this);
    }
}
