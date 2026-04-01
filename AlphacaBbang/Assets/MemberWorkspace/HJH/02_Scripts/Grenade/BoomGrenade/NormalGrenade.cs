using System.Collections;
using UnityEngine;

public class NormalGrenade : GrenadeBehavior
{
    public float range;
    public float maxDamage;

    protected override void OnExplode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        foreach (var hit in hits)
        {
            float distance = Vector3.Distance(transform.position, hit.transform.position);

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