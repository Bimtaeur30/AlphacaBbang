using JJH._02_Scripts.Weapons;
using UnityEngine;

public abstract class 
    MeleeWeaponBase : MonoBehaviour,IWeapon
{
    public CharacterState characterState;

    [field:SerializeField]public GunDataSO WeaponData { get; private set; }
    [SerializeField] protected ShortWeaponSO[] data;
    protected int currentLevel = 0;

    protected float lastUseTime;
    protected float currentTime = 0;


    public bool IsFiring => false;

    public bool IsAiming => false;

    public bool IsReloading => false;

    public virtual void Initialize(WeaponHandleModule owner)
    {
    }
    public void TickFire()
    {
    }
    public virtual void SetAim(bool val)
    {

    }

    public void StartFire(bool isAim)
    {
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
    //public virtual void Attack(Vector3 targetPos, bool isAttack)
    //{
    //    Debug.Log($"Attack is : {isAttack}");
    //    if (!isAttack) return;

    //    Debug.Log($"Current Time : {currentTime}, Attack Dela : {data[currentLevel].attackDelay}");

    //    if (currentTime < data[currentLevel].attackDelay) return;

    //    PerformAttack(targetPos);
    //}

    protected abstract void PerformAttack(Vector3 targetPos);

    protected virtual Vector3 GetShootDirection()
    {
        return transform.right.normalized;
    }
}