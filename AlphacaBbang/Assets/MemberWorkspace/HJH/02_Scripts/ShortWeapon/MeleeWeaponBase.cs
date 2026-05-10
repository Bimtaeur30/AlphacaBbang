using UnityEngine;

public abstract class MeleeWeaponBase : MonoBehaviour
{
    public CharaterState charaterState;

    [SerializeField] protected ShortWeaponSO[] data;
    protected int currentLevel = 0;

    protected float lastUseTime;
    protected float currentTime = 0;


    public virtual void Init() { }

    public virtual void SetAim(bool val) 
    {

    }
    void Update()
    {
        currentTime += Time.deltaTime;
    }
    public virtual void Attack(Vector3 targetPos, bool isAttack)
    {
        Debug.Log($"Attack is : {isAttack}");
        if (!isAttack) return;

        Debug.Log($"Current Time : {currentTime}, Attack Dela : {data[currentLevel].attackDelay}");

        if (currentTime < data[currentLevel].attackDelay) return;

        PerformAttack(targetPos);
    }

    protected abstract void PerformAttack(Vector3 targetPos);
}