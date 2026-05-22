using JJH._02_Scripts_Systems.AnimationSystems;
using System.Collections;
using UnityEngine;

public class BaseballBat : MeleeWeaponBase
{
    [SerializeField] private float comboWindow = 0.4f;
    [SerializeField] private float crossFadeDuration = 0.1f;
    [SerializeField] private int animLayerIndex = 0;
    [SerializeField] private AnimParamSO[] attackAnimParam;
    [SerializeField] private TrailRenderer trailRenderer;

    public int ComboCounter { get; private set; } = 0;

    private Coroutine trailCoroutine;
    private void Awake()
    {
        if (trailRenderer == null)
            trailRenderer = GetComponentInChildren<TrailRenderer>();

        if (trailRenderer == null)
        {
            Debug.LogError("TrailRenderer 없어요.");
            return;
        }

        trailRenderer.emitting = false;
    }
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

        int currentCombo = ComboCounter;

        bool comboWindowExhausted = Time.time >= lastUseTime + comboWindow;

        if (comboWindowExhausted)
            ComboCounter = 0;
        else
            ComboCounter = (currentCombo < data.Length - 1) ? currentCombo + 1 : 0;

        lastUseTime = Time.time;

        characterRenderer.PlayClip(
            attackAnimParam[currentCombo].ParamHash,
            normalizedTime: 0f,
            crossFadeDuration: crossFadeDuration,
            layerIndex: animLayerIndex
        );

        if (trailCoroutine != null)
            StopCoroutine(trailCoroutine);
        trailCoroutine = StartCoroutine(TrailCoroutine(currentCombo));

        Vector3 origin = transform.position;
        Collider[] hits = Physics.OverlapSphere(origin, data[currentCombo].range);

        foreach (Collider hit in hits)
        {
            Vector3 toTarget = (hit.transform.position - origin).normalized;
            float angle = Vector3.Angle(dir, toTarget);

            if (angle <= data[currentCombo].angle * 0.5f)
            {
                ICharacterStateOwner stateOwner = hit.GetComponentInChildren<ICharacterStateOwner>()
                                               ?? hit.GetComponentInParent<ICharacterStateOwner>();

                if (stateOwner != null && stateOwner.CharacterState == characterState) continue;

                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                    damageable.TakeDamage(data[currentCombo].damage);
            }
        }

        currentTime = 0;
    }

    private IEnumerator TrailCoroutine(int comboIndex)
    {
        if (trailRenderer == null) yield break;

        trailRenderer.emitting = true;

        yield return null;

        AnimatorStateInfo stateInfo = characterRenderer.Animator.GetCurrentAnimatorStateInfo(animLayerIndex);
        float clipLength = stateInfo.length;

        yield return new WaitForSeconds(clipLength);

        trailRenderer.emitting = false;
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
}