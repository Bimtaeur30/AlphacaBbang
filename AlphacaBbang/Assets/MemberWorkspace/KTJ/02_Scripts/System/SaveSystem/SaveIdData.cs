using UnityEngine;

[CreateAssetMenu(fileName = "Save id", menuName = "KTJ/System/Save id")]
public class SaveIdData : ScriptableObject
{
    [field:SerializeField] public int Id { get; private set; }
    [SerializeField, TextArea] private string description;
}
