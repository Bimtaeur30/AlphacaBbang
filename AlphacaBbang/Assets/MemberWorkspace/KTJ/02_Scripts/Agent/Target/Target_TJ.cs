using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Target_TJ : ModuleOwner, IDamageable
{
    [field: SerializeField] public float Health { get; private set; } = 100f;
    [SerializeField] private TextMeshPro healthTxt;
    private EnemyGunHandleModule gunHandleModule;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();
        gunHandleModule = GetModule<EnemyGunHandleModule>();
        Debug.Assert(gunHandleModule != null, "EnemyGunHandleModule을 발견하지 못했습니다.");

    }

    protected override void Awake()
    {
        base.Awake();
        healthTxt.text = Health.ToString();
    }

    private void Start()
    {
        StartCoroutine(Test());
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            Debug.Log(gameObject.name + "이(가) 뒤짐.");
        }

        healthTxt.text = Health.ToString();
    }

    public void ApplyBurn(float dps, float duration)
    {
    }


    IEnumerator Test()
    {
        while (true)
        {
            gunHandleModule.Attack(Vector3.zero, true);
            yield return new WaitForSeconds(3f);
            gunHandleModule.Attack(Vector3.zero, false);
            yield return new WaitForSeconds(3f);
        }
    }

    //============================= [TEST] ============================//
    // _recoilOffset = Vector2.Lerp(_recoilOffset, Vector2.zero, Time.deltaTime* recoilRecoverSpeed);
    // Vector2 targetScreenPos = _mousePos + _recoilOffset;
}
