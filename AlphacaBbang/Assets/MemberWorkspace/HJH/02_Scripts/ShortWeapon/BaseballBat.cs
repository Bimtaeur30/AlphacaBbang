using UnityEngine;

public class BaseballBat : MeleeWeaponBase
{
    protected override void PerformAttack(Vector3 targetPos)
    {
        PlayAttackParticle(targetPos);

        Vector3 origin = transform.position;
        Vector3 dir = (targetPos - origin).normalized;

        Collider[] hits = Physics.OverlapSphere(origin, data.range);

        foreach (Collider hit in hits)
        {
            Vector3 toTarget = (hit.transform.position - origin).normalized;
            float angle = Vector3.Angle(dir, toTarget);

            if (angle <= data.angle * 0.5f)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(data.damage);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (data == null) return;

        Gizmos.color = Color.red;

        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        Quaternion leftRot = Quaternion.AngleAxis(-data.angle * 0.5f, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(data.angle * 0.5f, Vector3.up);

        Vector3 leftDir = leftRot * forward * data.range;
        Vector3 rightDir = rightRot * forward * data.range;

        Gizmos.DrawLine(origin, origin + leftDir);
        Gizmos.DrawLine(origin, origin + rightDir);
        Gizmos.DrawWireSphere(origin, data.range);
    }
    private void PlayAttackParticle(Vector3 targetPos)
    {
        if (data.attackParticlePrefab == null) return;

        Vector3 origin = transform.position;
        Vector3 dir = targetPos - origin;

        if (dir == Vector3.zero)
            dir = transform.forward;

        dir.y = 0f;

        Quaternion rot = Quaternion.LookRotation(dir.normalized)
                       * Quaternion.Euler(-90f, 180f, 0f);

        Instantiate(data.attackParticlePrefab, origin, rot);
    }
}