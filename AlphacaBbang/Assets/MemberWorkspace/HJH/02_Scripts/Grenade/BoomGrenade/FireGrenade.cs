using UnityEngine;

public class FireGrenade : GrenadeBehavior
{
    [SerializeField] private GameObject fireZonePrefab;

    protected override void OnExplode()
    {
        Instantiate(fireZonePrefab, transform.position, Quaternion.identity);
    }
}