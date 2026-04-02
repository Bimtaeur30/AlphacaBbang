using UnityEngine;

public class BombExplosion : IExplosionDamage
{
    private float maxDamage;
    private float range;

    public BombExplosion(float maxDamage, float range)
    {
        this.maxDamage = maxDamage;
        this.range = range;
    }

    public void ApplyDamage(Vector3 center)
    {
        Collider[] hits = Physics.OverlapSphere(center, range);

        foreach (var hit in hits)
        {
            float distance = Vector3.Distance(center, hit.transform.position);

            float damage = Mathf.Lerp(maxDamage, 0, distance / range);

            hit.GetComponent<IDamageable>()?.TakeDamage(damage);
        }
    }
}