using UnityEngine;

public enum EquipType
{
    None,
    MainWeapon,
    SubWeapon,
    Helmet
};

[CreateAssetMenu(fileName = "ItemData", menuName = "SO/ItemData")]
public abstract class ItemData : ScriptableObject
{
    [field: SerializeField]
    public string ItemName { get; private set; }
    
    [field: SerializeField] 
    public Sprite Icon { get; private set; }
    
    [TextArea] public string description;
    
    [field: SerializeField]
    public EquipType EquipType { get; private set; }
}
