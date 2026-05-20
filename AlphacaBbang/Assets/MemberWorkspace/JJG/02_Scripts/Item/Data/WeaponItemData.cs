using UnityEngine;
[CreateAssetMenu(fileName = "WeaponItemData", menuName = "JJK/WeaponItemData")]
public class WeaponItemData : ItemData
{
    [field: SerializeField] public GameObject Gun { get; private set; }
    [field: SerializeField] public string MeleeWeaponId { get; private set; }
}