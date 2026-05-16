using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItemData", menuName = "JJK/WeaponItemData")]
public class WeaponItemData : ItemData
{
    [field: SerializeField] public Gun Gun { get; private set; }
}
