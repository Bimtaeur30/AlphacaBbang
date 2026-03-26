using System.Collections;
using UnityEngine;

public class SmokeGrenade : GrenadeBehavior
{
    public GrenadeSO Grenade;
    //public GameObject smokeZonePrefab;

    protected override void OnExplode()
    {
        //Instantiate(smokeZonePrefab, transform.position, Quaternion.identity);
    }

    public void OnDrawGizmos()
    {
        Debug.Log("기즈모 켜짐");
        Gizmos.color = Color.yellow;

        float range = Grenade.range;

        Gizmos.DrawWireSphere(gameObject.transform.position, range);
    }
}