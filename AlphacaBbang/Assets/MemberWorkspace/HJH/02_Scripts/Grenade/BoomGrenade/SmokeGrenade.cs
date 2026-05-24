using UnityEngine;

public class SmokeGrenade : GrenadeBehavior
{
    [SerializeField] private GameObject smokeZonePrefab;

    protected override void OnExplode()
    {
        Instantiate(smokeZonePrefab, transform.position, Quaternion.identity);
    }
}
