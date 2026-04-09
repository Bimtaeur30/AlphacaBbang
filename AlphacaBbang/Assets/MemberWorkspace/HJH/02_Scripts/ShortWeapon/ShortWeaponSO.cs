using UnityEngine;

[CreateAssetMenu(fileName = "ShortWeaponSO", menuName = "Scriptable Objects/ShortWeaponSO")]
public class ShortWeaponSO : ScriptableObject
{
    public string weaponName;
    public float range;
    public float damage;
}
