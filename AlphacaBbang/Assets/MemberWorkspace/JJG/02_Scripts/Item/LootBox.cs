using UnityEngine;

public class LootBox : MonoBehaviour
{
    [SerializeField] private LootTable lootTable;
    [SerializeField] private string boxDisplayName = "상자";
 
    public LootTable LootTable => lootTable;
    public string BoxDisplayName => boxDisplayName;
 
    private bool _isOpened = false;
    public bool IsOpened => _isOpened;
 
    public void MarkAsOpened() => _isOpened = true;
 
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (lootTable == null)
            Debug.LogWarning($"[LootBox] '{gameObject.name}'에 LootTable이 연결되지 않았습니다.", this);
    }
#endif
}