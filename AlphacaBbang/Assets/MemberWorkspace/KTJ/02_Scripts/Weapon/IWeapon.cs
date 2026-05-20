using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IWeapon
{
    GunDataSO WeaponData { get; }
    bool IsFiring { get; }
    bool IsAiming { get; }
    bool IsReloading { get; }
    void Initialize(WeaponHandleModule owner); //; 초기화
    void TickFire(); // 오토 총기용 발사
    void StartFire(bool isAim); // 발사 시작(현재 에임상태)
    void StopFire(bool isAim); // 발사 중지(현재 에임상태)
    void SetAim(bool isAim); // 에임 시작/해제
}