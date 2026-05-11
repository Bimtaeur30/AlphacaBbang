using UnityEngine;

public class Axe : MeleeWeaponBase
{
    [SerializeField] private float comboWindow = 0.4f;
    public int ComboCounter { get; private set; } = 0;
    protected override void PerformAttack(Vector3 targetPos)
    {
        PlayAttackParticle(targetPos);

        bool comboCounterOver = ComboCounter >= data.Length;
        bool comboWindowExhausted = Time.time >= lastUseTime + comboWindow;

        Debug.Log($"ComboCounter: {ComboCounter}");

        if (comboCounterOver || comboWindowExhausted)
        {
            Debug.Log("Combo reset");
            if (data == null || data.Length == 0) return;
            lastUseTime = Time.time;

            if (ComboCounter < data.Length - 1)
            {
                ComboCounter++;
            }
            else
            {
                ComboCounter = 0;
            }
        }


        Vector3 origin = transform.position;
        Vector3 dir = (targetPos - origin).normalized;

        Collider[] hits = Physics.OverlapSphere(origin, data[ComboCounter].range);

        foreach (Collider hit in hits)
        {
            Vector3 toTarget = (hit.transform.position - origin).normalized;
            float angle = Vector3.Angle(dir, toTarget);

            if (angle <= data[ComboCounter].angle * 0.5f)
            {
                ICharacterStateOwner stateOwner = hit.GetComponentInChildren<ICharacterStateOwner>()
                                               ?? hit.GetComponentInParent<ICharacterStateOwner>();

                if (stateOwner != null && stateOwner.CharacterState == characterState) continue;

                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(data[ComboCounter].damage);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (data == null || data.Length == 0) return;

        Gizmos.color = Color.red;

        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        Quaternion leftRot = Quaternion.AngleAxis(-data[ComboCounter].angle * 0.5f, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(data[ComboCounter].angle * 0.5f, Vector3.up);

        Vector3 leftDir = leftRot * forward * data[ComboCounter].range;
        Vector3 rightDir = rightRot * forward * data[ComboCounter].range;
        Gizmos.DrawLine(origin, origin + leftDir);
        Gizmos.DrawLine(origin, origin + rightDir);
        Gizmos.DrawWireSphere(origin, data[ComboCounter].range);
    }
    private void PlayAttackParticle(Vector3 targetPos)
    {
        if (data[ComboCounter].attackParticlePrefab == null) return;
        Vector3 origin = transform.position;
        Vector3 dir = targetPos - origin;

        if (dir == Vector3.zero)
            dir = transform.forward;

        dir.y = 0f;

        Quaternion rot = Quaternion.LookRotation(dir.normalized)
                       * Quaternion.Euler(-90f, 180f, 0f);

        gameObject.transform.parent.rotation = rot;
        Instantiate(data[ComboCounter].attackParticlePrefab, origin, rot);
    }
}