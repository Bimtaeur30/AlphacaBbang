using JJH._02_Scripts_Systems.AnimationSystems;
using System;
using System.Collections;
using UnityEngine;

public class Sword : MeleeWeaponBase
{
    [SerializeField] private float comboWindow = 0.75f;
    [SerializeField] private float crossFadeDuration = 0.1f;
    [SerializeField] private int animLayerIndex = 0;

    [SerializeField] private AnimParamSO[] attackAnimParam;
    [SerializeField] private Transform visual;
    public int ComboCounter = 0;

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
            {
                Debug.LogError($"[Sword] visual 오브젝트를 찾지 못했습니다! 오브젝트: {gameObject.name}");
            }
        }
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

        // 콤보 윈도우 체크 후 다음 콤보 결정
        bool resetCombo = Time.time > lastUseTime + comboWindow;
        if (resetCombo)
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

        if (data[currentCombo].attackDelay > comboWindow)
        {
            Debug.Assert(false, "Attack delay is bigger than lastUseTime");
        }

        Debug.Log($"currentCombo: {currentCombo}, 다음 ComboCounter: {ComboCounter}");

        Vector3 origin = transform.position;
        Vector3 dir = (targetPos - origin).normalized;

        PlayAttackParticle(targetPos, currentCombo);

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

    public IEnumerator AttackDash(Vector3 targetPos, int comboIndex)
    {
        Debug.Log("돌진");

        if (visual == null) yield break;

        Vector3 dashDir = (targetPos - visual.position).normalized;
        dashDir.y = 0f;

        float duration = 0.15f;
        float timer = 0f;
        float speed = 10f;

        while (timer < duration)
        {
            if (visual == null) yield break;

            visual.position += dashDir * speed * Time.deltaTime;
            timer += Time.deltaTime;

            yield return null;
        }

        PlayAttackParticle(targetPos, comboIndex);
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

    private void PlayAttackParticle(Vector3 targetPos, int comboIndex)
    {
        if (data[comboIndex].attackParticlePrefab == null) return;

        Vector3 origin = transform.position;
        Vector3 dir = targetPos - origin;

        if (dir == Vector3.zero)
            dir = transform.forward;

        dir.y = 0f;

        Quaternion rot = Quaternion.LookRotation(dir.normalized)
                       * data[comboIndex].attackParticlePrefab.transform.rotation
                       * Quaternion.Euler(0f, 0f, 180f);

        Instantiate(data[comboIndex].attackParticlePrefab, origin, rot);
    }
}