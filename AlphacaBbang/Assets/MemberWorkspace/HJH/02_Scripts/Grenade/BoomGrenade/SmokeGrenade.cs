using System.Collections;
using UnityEngine;

public class SmokeGrenade : GrenadeBehavior
{
    public override IEnumerator Boom(GameObject projectile)
    {
        Debug.Log("연막 생성");
        Destroy(projectile);
        yield break;
    }
}