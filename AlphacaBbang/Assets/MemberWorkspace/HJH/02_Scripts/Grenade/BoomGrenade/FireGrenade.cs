using System.Collections;
using UnityEngine;

public class FireGrenade : GrenadeBehavior
{
    public GrenadeSO Grenade;
    public GameObject fireZonePrefab;
    protected override void OnExplode()
    {
        Instantiate(fireZonePrefab, transform.position, Quaternion.identity);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        float range = Grenade.range;

        Gizmos.DrawWireSphere(gameObject.transform.position, range);
    }
}