using JJH._02_Scripts.Agents.Enemies;
using UnityEngine;

public interface IWeapon
{
    void Init(); // 초기화. 적이 무기를 사용하기 전에 무조건 호출해야힘.
    void SetAim(bool val); // true를 넘기면 조준을 시작하고, false를 넘기면 조준해제함.
    void Attack(Vector3 vector, bool val);
}
