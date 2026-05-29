using JJH._02_Scripts.Systems.ObjectPoolSystems;
using JJH._02_Scripts.Systems.SoundSystems;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Warhead : PoolableMono, IProjectile // 바주카 탄두
{
    [SerializeField] private PoolItemSO explosionPref;
    [SerializeField] private SoundClipSO explosionSound;
    [SerializeField] private LayerMask layerMask;
    private Rigidbody body;
    private Collider collider;
    private GunSoundPlayer soundPlayer;
    private int damage;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        soundPlayer = GetComponentInChildren<GunSoundPlayer>();
        Debug.Assert(soundPlayer != null, "건사운드 플레이어가 워 헤드에 붙어있지 않습니다.");
    }

    public void Fire(Vector3 dir, float speed, int damage)
    {
        dir.Normalize();
        this.damage = damage;
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;

        body.linearVelocity = dir * speed;

        Debug.Log("발사 시작");
    }

    private void OnHit() // 여기서 폭발 처리
    {
        soundPlayer.PlaySound(explosionSound);
        body.linearVelocity = Vector3.zero;

        ExplosionPrefab effect = PoolManager.Pop<ExplosionPrefab>(explosionPref);
        effect.transform.position = transform.position;
        effect.Active(5f, damage, layerMask);

        PoolManager.Push(this);

    }

    private void OnTriggerEnter(Collider other)
    {
        OnHit();
    }
}
