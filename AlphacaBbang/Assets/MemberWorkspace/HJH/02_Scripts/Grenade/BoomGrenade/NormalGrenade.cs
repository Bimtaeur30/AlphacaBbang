using System.Collections;
using UnityEngine;

public class NormalGrenade : GrenadeBehavior
{
    public float boomTime = 2f;

    public override IEnumerator Boom(GameObject projectile)
    {
        yield return new WaitForSeconds(boomTime);

        Debug.Log("¼ö·ùÅº Æø¹ß");
        Destroy(projectile);
    }
}