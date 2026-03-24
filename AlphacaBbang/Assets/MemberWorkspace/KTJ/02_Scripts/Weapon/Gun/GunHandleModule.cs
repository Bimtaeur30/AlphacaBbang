using UnityEngine;

public class GunHandleModule : MonoBehaviour, IModule
{
    [field:SerializeField] public bool onAim { get; private set; }
    [field:SerializeField] public bool onFire { get; private set; }
    [SerializeField] private Gun currentGun;

    private float _currentTime;

    public void Initialize(ModuleOwner owner)
    {
    }

    private void Update()
    {
        if (onFire && currentGun.GunDataSO.FireMode == FireMode.Auto) // 오토일 경우 여러발 발사
        {
            _currentTime += Time.deltaTime;
            if (_currentTime > currentGun.GunDataSO.FireInterval)
            {
                currentGun.Fire();
                _currentTime = 0;
            }
        }
    }

    public void Fire(bool v)
    {
        if (v)
        {
            onFire = true;
            if (currentGun.GunDataSO.FireMode == FireMode.Single) // 단발일 경우 한번 쏘기
            {
                currentGun.Fire();
            }
        }
        else
        {
            onFire = false;
        }
    }

    public void Aim(bool v)
    {
        if (v)
        {
            onAim = true;
        }
        else
        {
            onAim = false;
        }
    }
}
