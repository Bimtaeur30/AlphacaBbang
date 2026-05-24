using System.Collections;
using UnityEngine;

public class GrenadeProjectileLauncher : MonoBehaviour
{
    public static void Launch(GrenadeSO grenadeSO, Vector3 spawnPos, Vector3 targetPos,
                               float firingAngle, float gravity, MonoBehaviour caller)
    {
        GameObject launcher = new GameObject("GrenadeProjectileLauncher");
        GrenadeProjectileLauncher comp = launcher.AddComponent<GrenadeProjectileLauncher>();
        comp.StartCoroutine(comp.LaunchRoutine(grenadeSO, spawnPos, targetPos, firingAngle, gravity));
    }

    private IEnumerator LaunchRoutine(GrenadeSO grenadeSO, Vector3 spawnPos, Vector3 targetPos,
                                       float firingAngle, float gravity)
    {
        GameObject projectile = Instantiate(grenadeSO.prefab, spawnPos, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogWarning("[GrenadeProjectileLauncher] Rigidbody 없음");
            Destroy(gameObject);
            yield break;
        }

        Vector3 direction = (targetPos - spawnPos);
        direction.y = 0;
        float distance = direction.magnitude;
        direction = direction.normalized;

        float angleRad = firingAngle * Mathf.Deg2Rad;
        float sinValue = Mathf.Sin(2 * angleRad);
        if (Mathf.Abs(sinValue) < 0.01f)
        {
            Destroy(gameObject);
            yield break;
        }

        float velocity = distance * gravity / sinValue;
        float Vx = Mathf.Sqrt(velocity) * Mathf.Cos(angleRad);
        float Vy = Mathf.Sqrt(velocity) * Mathf.Sin(angleRad);

        projectile.transform.rotation = Quaternion.LookRotation(direction);
        rb.linearVelocity = new Vector3(direction.x * Vx, Vy, direction.z * Vx);

        yield return null;
        Destroy(gameObject);
    }
}