using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FireZone : MonoBehaviour
{
    public GrenadeSO Grenade;
    public float damagePerSecond;
    public float duration;
    public float range;

    private void Start()
    {
        duration = Grenade.Duration;

        StartCoroutine(Fire());
    }

    private IEnumerator Fire()
    {
        float time = 0f;

        while (time < duration)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, range);

            foreach (Collider hit in hits)
            {
                hit.GetComponent<IDamageable>()?.TakeDamage(damagePerSecond);
            }

            yield return new WaitForSeconds(1f);
            time += 1f;
        }

        Destroy(gameObject);
    }
}