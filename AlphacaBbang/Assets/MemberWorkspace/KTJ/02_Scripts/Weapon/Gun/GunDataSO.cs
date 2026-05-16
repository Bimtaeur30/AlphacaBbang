using JJH._02_Scripts.Systems.SoundSystems;
using UnityEngine;
using UnityEngine.UI;

public enum FireMode
{
    Single, Auto, Spread
}

public enum BulletType
{
    B1, B2, B3
}

[CreateAssetMenu(fileName = "Gun data", menuName = "KTJ/Gun/GunData")]
public class GunDataSO : ScriptableObject
{
    [Header("FireMode")]
    [field: SerializeField] public FireMode FireMode { get; private set; }

    [Header("Bullet")]
    [field:SerializeField] public BulletType BulletType { get; private set; } // 총알 타입
    [field:SerializeField] public int MagCapacity{ get; private set; } // 탄창 용량
    [field: SerializeField] public float ReloadDuration { get; private set; } = 2f; // 재장전 시간


    [Header("Fire")]
    [field: SerializeField, Range(0.05f, 1.0f)] public float FireInterval { get; private set; } = 0.1f; // 발사 간격 초

    [Header("Durability")]
    [field: SerializeField, Range(50, 200)] public int Durability { get; private set; } = 50; // 내구도

    [Header("RecoilX")]
    [field: SerializeField, Range(0f, 100f)] public float RecoilForceX { get; private set; } = 1f; // 반동

    [Header("RecoilY")]
    [field: SerializeField, Range(0f, 100f)] public float RecoilForceY { get; private set; } = 1f; // 반동

    [Header("Accuracy")]
    [field: SerializeField, Range(1f, 100f)] public float SpreadAngle { get; private set; } = 1f; // 탄 퍼짐(샷건전용)

    [Header("CAM")]
    [field: SerializeField, Range(1f, 5f)] public float CameraImpulseMultiply { get; private set; } = 1f; // 조준 이미지

    [Header("Accuracy")]
    [field: SerializeField, Range(1f, 10f)] public int BulletFireCount { get; private set; } = 1; // 한번에 나가는 총알 개수(샷건전용)

    [Header("UI")]
    [field: SerializeField] public Sprite CrossHairSprite { get; private set; } // 조준 이미지


    [Header("Sound")]
    [field: SerializeField] public SoundClipSO FireClip { get; private set; } // fire sound
    [field: SerializeField] public SoundClipSO DryFireClip { get; private set; } // on no bullets
    [field: SerializeField] public SoundClipSO LoadClip { get; private set; } // on reload 2
    [field: SerializeField] public SoundClipSO UnLoadClip { get; private set; } // on reload 1
    [field: SerializeField] public SoundClipSO CookClip { get; private set; } // on reload end
}
