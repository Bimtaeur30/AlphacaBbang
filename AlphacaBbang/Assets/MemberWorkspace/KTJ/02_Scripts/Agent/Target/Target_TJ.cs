using System.Collections;
using TMPro;
using UnityEngine;

public class Target_TJ : ModuleOwner, IDamageable
{
    [field: SerializeField] public float Health { get; private set; } = 100f;
    [SerializeField] private TextMeshPro healthTxt;
    private GunHandleModule iweapon;

    protected override void Awake()
    {
        healthTxt.text = Health.ToString();
        iweapon = GetComponentInChildren<GunHandleModule>();
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
            Debug.Log(gameObject.name + "ÀÌ(°¡) µÚÁü.");
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
            iweapon.Attack(Vector3.zero, true);
            yield return new WaitForSeconds(3f);
            iweapon.Attack(Vector3.zero, false);
            yield return new WaitForSeconds(3f);
        }
    }
}
