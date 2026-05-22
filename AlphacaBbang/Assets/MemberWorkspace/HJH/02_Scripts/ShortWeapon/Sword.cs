using JJH._02_Scripts_Systems.AnimationSystems;
using System.Collections;
using UnityEngine;

public class Sword : MeleeWeaponBase
{
    [SerializeField] private float comboWindow = 0.75f;
    [SerializeField] private float crossFadeDuration = 0.1f;
    [SerializeField] private int animLayerIndex = 0;
    [SerializeField] private AnimParamSO[] attackAnimParam;
    [SerializeField] private TrailRenderer trailRenderer;

    private Transform visual;
    public int ComboCounter = 0;

    private Coroutine trailCoroutine;

    private void Awake()
    {
        if (visual == null)
        {
            Transform current = this.transform;
            while (current != null)
            {
                if (current.name == "Alphaca")
                {
                    visual = current;
                    break;
                }
                current = current.parent;
            }

            if (visual == null)
                Debug.LogError($"[Sword] visual 오브젝트를 찾지 못했습니다! 오브젝트: {gameObject.name}");
        }

        if (trailRenderer == null)
            trailRenderer = GetComponentInChildren<TrailRenderer>();

        if (trailRenderer == null)
        {
            Debug.LogError("TrailRenderer를 찾을 수 없습니다. 무기 오브젝트에 추가해주세요.");
            return;
        }

        trailRenderer.emitting = false;
    }

    protected override void PerformAttack(Vector3 targetPos)
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

        bool resetCombo = Time.time > lastUseTime + comboWindow;

        if (resetCombo)
            ComboCounter = 0;
        else
            ComboCounter = (currentCombo < data.Length - 1) ? currentCombo + 1 : 0;

        characterRenderer.PlayClip(
            attackAnimParam[currentCombo].ParamHash,
            normalizedTime: 0f,
            crossFadeDuration: crossFadeDuration,
            layerIndex: animLayerIndex
        );

        lastUseTime = Time.time;

        if (data[currentCombo].attackDelay > comboWindow)
            Debug.Assert(false, "Attack delay is bigger than lastUseTime");

        if (trailCoroutine != null)
            StopCoroutine(trailCoroutine);
        trailCoroutine = StartCoroutine(TrailCoroutine());

        Vector3 origin = transform.position;
        Vector3 dir = (targetPos - origin).normalized;

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

    private IEnumerator TrailCoroutine()
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