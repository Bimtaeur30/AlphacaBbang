using UnityEngine;
using UnityEngine.InputSystem;

public class Test_EnemyAttack : MonoBehaviour, IEnemyWeaponModule
{
    [SerializeField] private MeleeWeaponBase weapon;
    
    private void Start()
    {
    }

    public void EnemyAttack()
    {
        Vector3 targetPos = transform.position;
        weapon.Attack(targetPos, true);
    }

    public void Init()
    {

    }

    public void SetAim(bool val)
    {

    }

    public void Attack(Vector3 vector, bool val)
    {
    }
}
