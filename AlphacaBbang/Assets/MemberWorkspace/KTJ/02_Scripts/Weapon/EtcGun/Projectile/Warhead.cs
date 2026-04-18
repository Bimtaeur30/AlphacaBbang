using JJH._02_Scripts.Systems.ObjectPoolSystems;
using Unity.AppUI.Core;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Warhead : PoolableMono, IProjectile // 바주카 탄두
{
    [SerializeField] private PoolManagerSO poolManager;
    private Rigidbody body;
    private Collider collider;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    public void Fire(Vector3 dir, float speed)
    {
        dir.Normalize();
        body.linearVelocity = dir * speed;

        Debug.Log("발사 시작");
    }

    private void OnHit() // 여기서 폭발 처리
    {
        body.linearVelocity = Vector3.zero;
        poolManager.Push(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnHit();
    }
}
