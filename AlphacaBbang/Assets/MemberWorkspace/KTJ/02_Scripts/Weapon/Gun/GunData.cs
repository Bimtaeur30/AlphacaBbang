using UnityEngine;

public enum FireMode
{
    Single, Auto
}

public enum BulletType
{
    B1, B2, B3
}

[CreateAssetMenu(fileName = "Gun data", menuName = "KTJ/Gun/GunData")]
public class GunData : ScriptableObject
{
    [field:SerializeField] public FireMode FireMode { get; private set; }
    [field:SerializeField] public BulletType BulletType { get; private set; }
    [field: SerializeField] public float FireDuration { get; private set; } = 0.1f;
    [field: SerializeField] public int Durability { get; private set; } = 100;
}
