using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

public class GunHandleModule : MonoBehaviour, IModule
{
    [Header("State")]
    [field:SerializeField] public bool onAim { get; private set; }
    [field:SerializeField] public bool onFire { get; private set; }
    [field:SerializeField] public bool isAuto { get; private set; }

    [Header("Anim")]
    [field: SerializeField] AnimParamSO idleAnimParam;
    [field: SerializeField] AnimParamSO AimAnimParam;
    [field: SerializeField] AnimParamSO SingleFireAnimParam;
    [field: SerializeField] AnimParamSO AutoFireAnimParam;

    [Header("Gun")]
    [field: SerializeField] public Gun CurrentGun { get; private set; }

    [Header("System")]
    [SerializeField] private EventChannelSO SystemChannel;

    private float _currentTime;

    public void Initialize(ModuleOwner owner)
    {
        Debug.Log("GunHandleModule Initialize");
        SystemChannel.RaiseEvent(SystemEventChannel.WeaponEqnupEventChannel.Init(CurrentGun.GunDataSO));
        Init();
    }

    [ContextMenu("Initialize")]
    private void Init()
    {
        isAuto = CurrentGun.GunDataSO.FireMode == FireMode.Auto ? true : false;
    }

    private void Update()
    {
        if (onFire && onAim && CurrentGun.GunDataSO.FireMode == FireMode.Auto) // 오토일 경우 여러발 발사
        {
            _currentTime += Time.deltaTime;
            if (_currentTime > CurrentGun.GunDataSO.FireInterval)
            {
                CurrentGun.Fire();
                _currentTime = 0;
            }
        }
    }

    public void Fire(bool v)
    {
        if (v)
        {
            onFire = true;
            FireMode fireMode = CurrentGun.GunDataSO.FireMode;
            if (onAim)
            {
                if (fireMode == FireMode.Single) // 단발일 경우 한번 쏘기
                {
                    CurrentGun.Renderer.PlayClip(SingleFireAnimParam.ParamHash, 0, 0.1f, 0);
                    CurrentGun.Fire();
                }
                else if (fireMode == FireMode.Auto)
                {
                    float m_time = 0.083f / CurrentGun.GunDataSO.FireInterval;

                    CurrentGun.Renderer.PlayClip(AutoFireAnimParam.ParamHash, 0, 0.1f, 0, m_time);
                }
                else if (fireMode == FireMode.Shotgun)
                {
                    CurrentGun.Renderer.PlayClip(SingleFireAnimParam.ParamHash, 0, 0.1f, 0);
                    for (int i = 0; i < CurrentGun.GunDataSO.BulletFireCount; i++)
                    {
                        CurrentGun.Fire();
                    }
                }
            }
        }
        else
        {
            onFire = false;
            if (onAim)
            {
                CurrentGun.Renderer.PlayClip(AimAnimParam.ParamHash, 0, 0.1f, 0);
            }
            else
            {
                CurrentGun.Renderer.PlayClip(idleAnimParam.ParamHash, 0, 0.1f, 0);
            }
        }
    }

    public void Aim(bool v)
    {
        if (v)
        {
            onAim = true;
            CurrentGun.Renderer.PlayClip(AimAnimParam.ParamHash, 0, 0.1f, 0);
        }
        else
        {
            onAim = false;
            CurrentGun.Renderer.PlayClip(idleAnimParam.ParamHash, 0, 0.1f, 0);
        }
    }
}