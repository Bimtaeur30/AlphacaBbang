using UnityEngine;

public abstract class MeleeWeaponBase : MonoBehaviour, IWeapon
{
    [SerializeField] protected ShortWeaponSO data;

    public virtual void Init() { }

    public virtual void SetAim(bool val) { }

    public virtual void Attack(Vector3 targetPos, bool isAttack)
    {
        if (!isAttack) return;
        PerformAttack(targetPos);
    }

    protected abstract void PerformAttack(Vector3 targetPos);
}