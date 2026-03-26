using System.Collections;
using UnityEngine;

public abstract class GrenadeBehavior : MonoBehaviour
{
    public float boomTime = 2f;

    public IEnumerator Boom(GameObject projectile)
    {
        yield return new WaitForSeconds(boomTime);

        OnExplode();

        Destroy(projectile);
    }

    protected abstract void OnExplode();
}