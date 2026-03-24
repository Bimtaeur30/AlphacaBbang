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
public class GunDataSO : ScriptableObject
{
    [field:SerializeField] public FireMode FireMode { get; private set; }
    [field:SerializeField] public BulletType BulletType { get; private set; }

    [Header("Fire")]
    [field: SerializeField, Range(0.05f, 1.0f)] public float FireInterval { get; private set; } = 0.1f; // 발사 간격 초

    [Header("Durability")]
    [field: SerializeField, Range(50, 200)] public int Durability { get; private set; } = 50; // 내구도

    [Header("RecoilX")]
    [field: SerializeField, Range(0f, 20f)] public float RecoilForceX { get; private set; } = 1f; // 반동

    [Header("RecoilY")]
    [field: SerializeField, Range(0f, 20f)] public float RecoilForceY { get; private set; } = 1f; // 반동

    [Header("Accuracy")]
    [field: SerializeField, Range(0f, 10f)] public float SpreadAngle { get; private set; } = 1f; // 탄 퍼짐
}
