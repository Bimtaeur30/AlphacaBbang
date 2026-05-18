using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.MemberWorkspace.HJH._02_Scripts.Grenade
{
    public interface IThrowingWeapon
    {
        bool IsReady { get; }
        void SetAim(bool isAiming);
        void SetTarget(Vector3 worldPosition);
        void Fire();
    }
}
