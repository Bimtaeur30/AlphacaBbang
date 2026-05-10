using System;
using System.Collections;
using UnityEngine;

public class Sword : MeleeWeaponBase
{
    [SerializeField] private float comboWindow = 0.75f;
    [SerializeField] private Transform visual;
    public int ComboCounter = 0;//{ get; private set; } = 0;
    

    protected override void PerformAttack(Vector3 targetPos)
    {
        if (data[ComboCounter].attackDelay > comboWindow)
        {
            Debug.Assert(false, "Attack delay is bigger than lastUseTime");
        }


        Debug.Log($"ComboCounter: {ComboCounter}");
        Debug.Log($"comboWindow{comboWindow}, lastUseTime{lastUseTime}, ComboCounter{ComboCounter}");

        bool resetCombo = Time.time > lastUseTime + comboWindow;

        if (resetCombo)
        {
            ComboCounter = 0;
        }
        else
        {
            if (ComboCounter < data.Length - 1)
                ComboCounter++;
            else
                ComboCounter = 0;
        }

        lastUseTime = Time.time;

        Vector3 origin = transform.position;
        Vector3 dir = (targetPos - origin).normalized;

        if (ComboCounter == 2)
        {
            StartCoroutine(AttackDash(targetPos));
        }
        else
        {
            PlayAttackParticle(targetPos);
        }

        Collider[] hits = Physics.OverlapSphere(origin, data[ComboCounter].range);

        foreach (Collider hit in hits)
        {
            Vector3 toTarget = (hit.transform.position - origin).normalized;
            float angle = Vector3.Angle(dir, toTarget);

            if (angle <= data[ComboCounter].angle * 0.5f)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(data[ComboCounter].damage);
                }
            }
        }

        currentTime = 0;
    }

    public IEnumerator AttackDash(Vector3 targetPos)
    {
        Debug.Log("倒柳");

        Vector3 dashDir = (targetPos - visual.position).normalized;
        dashDir.y = 0f;

        float duration = 0.15f;
        float timer = 0f;
        float speed = 10f;

        while (timer < duration)
        {
            visual.position += dashDir * speed * Time.deltaTime;

            timer += Time.deltaTime;

            yield return null;
        }
        PlayAttackParticle(targetPos);
    }

    int count = 0;
    public IEnumerator A()
    {
        if(count > 5)
        {
            yield break;
        }
        Debug.Log("内风凭 1");
        Debug.Log("内风凭 2");
        yield return new WaitForSeconds(1f);
        Debug.Log("内风凭 3");
        StartCoroutine(A());
        count++;
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

        Vector3 parentRotation = gameObject.transform.parent.rotation.eulerAngles;
        parentRotation.y = rot.eulerAngles.y;
        gameObject.transform.parent.rotation = Quaternion.Euler(parentRotation);

        Instantiate(data[ComboCounter].attackParticlePrefab, origin, rot);
    }
}