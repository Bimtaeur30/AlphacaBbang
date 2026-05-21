using MemberWorkspace.JJG._02_Scripts;
using UnityEngine;

public enum EquipType
{
    None,
    MainWeapon,
    SubWeapon,
    Helmet,
    Body
};

[CreateAssetMenu(fileName = "ItemData", menuName = "SO/ItemData")]
public abstract class ItemData : ScriptableObject
{
    [field: SerializeField] public string Id { get; private set; }
    [field: SerializeField] public string ItemName { get; private set; }
    
    [field: SerializeField] public Sprite Icon { get; private set; } 
    
    [TextArea] public string description;
    
    [field: SerializeField] public virtual EquipType EquipType { get; private set; }
    
    [field: SerializeField] public GradeType GradeType { get; private set; }
}
