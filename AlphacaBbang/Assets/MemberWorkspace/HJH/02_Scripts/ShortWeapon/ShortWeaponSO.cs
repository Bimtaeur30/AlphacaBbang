using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "ShortWeaponSO", menuName = "Scriptable Objects/ShortWeaponSO")]
public class ShortWeaponSO : ScriptableObject
{
    public string weaponName;
    public float damage = 10f;
    public float range = 2f;
    public float angle = 90f;

    public float attackDelay = 0.1f;

    public GameObject attackParticlePrefab;
}
