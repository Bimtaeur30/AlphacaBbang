using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "SO/Item/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [field: SerializeField] public List<ItemData> Items { get; private set; }  = new List<ItemData>();

    private Dictionary<string, ItemData> _itemMap;

    private void OnEnable()
    {
        _itemMap = new Dictionary<string, ItemData>();

        foreach (ItemData item in Items)
        {
            if (item == null)
                continue;

            if (_itemMap.ContainsKey(item.Id))
            {
                Debug.LogError($"중복된 ItemId 발견: {item.Id}");
                continue;
            }

            _itemMap.Add(item.Id, item);
        }
    }

    public bool TryGetItem(string itemId, out ItemData itemData)
    {
        if (_itemMap == null)
            OnEnable();

        return _itemMap.TryGetValue(itemId, out itemData);
    }
}