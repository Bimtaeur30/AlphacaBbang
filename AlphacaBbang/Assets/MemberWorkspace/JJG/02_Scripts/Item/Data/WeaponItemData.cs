using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItemData", menuName = "SO/WeaponItemData")]
public class WeaponItemData : ItemData
{
    [field: SerializeField]
    public float Damage  { get; private set; }
    
    [field: SerializeField]
    public int Ammo  { get; private set; } 
    
    [field: SerializeField]
    public float ReloadSpeed  { get; private set; } 
}
