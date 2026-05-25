using UnityEngine;

public class SmokeGrenade : GrenadeBehavior
{
    [SerializeField] private GameObject smokeZonePrefab;

    protected override void OnExplode()
    {
        Instantiate(smokeZonePrefab, transform.position + new Vector3(0,2,0), Quaternion.identity);
    }
}
