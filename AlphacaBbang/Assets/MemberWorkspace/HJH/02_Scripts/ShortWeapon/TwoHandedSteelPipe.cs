using JJH._02_Scripts_Systems.AnimationSystems;
using UnityEngine;

public class TwoHandedSteelPipe : MeleeWeaponBase
{
    [SerializeField] private float comboWindow = 0.4f;

    [SerializeField] private float crossFadeDuration = 0.1f;
    [SerializeField] private int animLayerIndex = 0;

    [SerializeField] private AnimParamSO[] attackAnimParam;
    public int ComboCounter { get; private set; } = 0;

    protected override void PerformAttack(Vector3 dir)
    {
        if (attackAnimParam == null || attackAnimParam.Length == 0)
        {
            Debug.LogError("attackAnimParam이 비어있습니다. Inspector 확인!");
            return;
        }
        if (data == null || data.Length == 0)
        {
            Debug.LogError("data가 비어있습니다. Inspector 확인!");
            return;
        }

        if (ComboCounter >= attackAnimParam.Length || ComboCounter >= data.Length)
            ComboCounter = 0;

        characterRenderer.PlayClip(
            attackAnimParam[ComboCounter].ParamHash,
            normalizedTime: 0f,
            crossFadeDuration: crossFadeDuration,
            layerIndex: animLayerIndex
        );

        PlayAttackParticle(dir);

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
                       * data[ComboCounter].attackParticlePrefab.transform.rotation
                       * Quaternion.Euler(0f, 0f, 180f);

        Instantiate(data[ComboCounter].attackParticlePrefab, origin, rot);
    }
}