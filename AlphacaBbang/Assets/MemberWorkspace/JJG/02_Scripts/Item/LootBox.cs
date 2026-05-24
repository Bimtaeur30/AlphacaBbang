using UnityEngine;
 
/// <summary>
/// 상자 오브젝트에 붙이는 컴포넌트.
/// 인스펙터에서 이 상자에 해당하는 LootTable SO를 연결하세요.
/// </summary>
public class LootBox : MonoBehaviour
{
    [SerializeField] private LootTable lootTable;
    [SerializeField] private string boxDisplayName = "상자";
 
    public LootTable LootTable => lootTable;
    public string BoxDisplayName => boxDisplayName;
 
    private bool _isOpened = false;
    public bool IsOpened => _isOpened;
 
    public void MarkAsOpened()
    {
        _isOpened = true;
    }
 
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (lootTable == null)
            Debug.LogWarning($"[LootBox] '{gameObject.name}'에 LootTable이 연결되지 않았습니다.", this);
    }
#endif
}