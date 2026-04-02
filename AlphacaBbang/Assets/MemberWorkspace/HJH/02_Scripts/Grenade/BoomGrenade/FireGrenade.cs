using System.Collections;
using UnityEngine;

public class FireGrenade : GrenadeBehavior
{
    public GrenadeSO Grenade;
    protected override void OnExplode()
    {
        var main = Grenade.particlePrefab.main;

        main.startLifetime = Grenade.Duration;

        Instantiate(Grenade.particlePrefab, transform.position, Quaternion.identity);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        float range = Grenade.range;

        Gizmos.DrawWireSphere(gameObject.transform.position, range);
    }
}