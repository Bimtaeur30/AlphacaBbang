using System.Collections;
using UnityEngine;

public abstract class GrenadeBehavior : MonoBehaviour
{
    public IEnumerator Boom(GameObject projectile, float boomTime)
    {
        yield return new WaitForSeconds(boomTime);

        OnExplode();

        Destroy(projectile);
    }

    protected abstract void OnExplode();
}