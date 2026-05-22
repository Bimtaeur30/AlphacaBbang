using JJH._02_Scripts.Weapons;
using JJH._02_Scripts_Systems.AnimationSystems;
using UnityEngine;

public abstract class 
    MeleeWeaponBase : MonoBehaviour,IWeapon
{
    public CharacterState characterState;

    [field:SerializeField]public GunDataSO WeaponData { get; private set; }
    [SerializeField] protected ShortWeaponSO[] data;
    protected IRenderer characterRenderer;
    protected int currentLevel = 0;

    protected float lastUseTime;
    protected float currentTime = 0;


    public bool IsFiring => false;

    public bool IsAiming => false;

    public bool IsReloading => false;

    public virtual void Initialize(WeaponHandleModule owner)
    {
        Agent agent = owner.Owner as Agent;
        characterRenderer = agent.Renderer;
    }
    public void TickFire()
    {
    }
    public virtual void SetAim(bool val)
    {

    }

    public void StartFire(bool isAim)
    {
        if (currentTime < data[currentLevel].attackDelay)
        {
            Debug.Log($"공격 대기 중입니다. 남은 시간: {data[currentLevel].attackDelay - currentTime:F2}초");
            return;
        }
        Debug.Log("근접무기 공격");
        Vector3 direction = GetShootDirection();
        PerformAttack(direction);
    }
    public void StopFire(bool isAim)
    {
    }

    void Update()
    {
        currentTime += Time.deltaTime;
    }
    protected abstract void PerformAttack(Vector3 targetPos);

    protected virtual Vector3 GetShootDirection()
    {
        return transform.right.normalized;
    }
}