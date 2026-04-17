using System.Collections;
using UnityEngine;

public class NormalGrenade : GrenadeBehavior
{
    public GameObject Grenade;
    public float range;
    public float maxDamage;

    protected override void OnExplode()
    {
        Instantiate(Grenade, transform.position, Quaternion.identity);
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        foreach (var hit in hits)
        {
            float distance = Vector3.Distance(transform.position + new Vector3(0,1,0), hit.transform.position);

            float damage = Mathf.Lerp(maxDamage, 0, distance / range);

            hit.GetComponent<IDamageable>()?.TakeDamage(damage);
        }
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        float range = this.range;

        Gizmos.DrawWireSphere(gameObject.transform.position, range);
    }
}