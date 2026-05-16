using System;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts.SettingUI
{
    public abstract class AbstractSettingUI : MonoBehaviour
    {
        public abstract void Awake();
        public abstract void OnEnable();
        public abstract void SettingData();
        public abstract void ResetData();
    } 
}