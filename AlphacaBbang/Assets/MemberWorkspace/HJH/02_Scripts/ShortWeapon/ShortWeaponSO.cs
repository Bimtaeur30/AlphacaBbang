using UnityEngine;

[CreateAssetMenu(fileName = "ShortWeaponSO", menuName = "Scriptable Objects/ShortWeaponSO")]
public class ShortWeaponSO : ScriptableObject
{
    public string weaponName;
    public float damage = 10f;
    public float range = 2f;
    public float angle = 90f;

    public GameObject attackParticlePrefab;
}
