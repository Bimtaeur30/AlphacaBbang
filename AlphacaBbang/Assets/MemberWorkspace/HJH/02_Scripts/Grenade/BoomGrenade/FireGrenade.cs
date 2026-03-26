using System.Collections;
using UnityEngine;

public class FireGrenade : GrenadeBehavior
{
    public override IEnumerator Boom(GameObject projectile)
    {
        Debug.Log("È­¿° »ý¼º");
        Destroy(projectile);
        yield break;
    }
}