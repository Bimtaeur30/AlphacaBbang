using System;
using UnityEngine;

public class Player_TJ : ModuleOwner
{
    #region 직렬화
    [SerializeField] private PlayerInputSO_KTJ playerInputSO;
    #endregion

    #region 모듈
    private GunHandleModule gunHandleModule;
    #endregion

    protected override void InitializeComponents()
    {
        gunHandleModule = GetModule<GunHandleModule>();
        Debug.Assert(gunHandleModule != null, "gunHandleModule is null");
    }

    private void OnEnable()
    {
        playerInputSO.OnAimEvent += HandleAimKey;
        playerInputSO.OnFireEvent += HandleFireKey;
    }
    private void OnDisable()
    {
        playerInputSO.OnAimEvent -= HandleAimKey;
        playerInputSO.OnFireEvent -= HandleFireKey;
    }

    private void HandleAimKey(bool v)
    {
        gunHandleModule.Aim(v);
        if (v == true) // 조준시작
        {
        }
        else // 조준해제
        {
        }
    }

    private void HandleFireKey(bool v)
    {
        gunHandleModule.Fire(v);
        if (v == true) // 조준시작
        {

        }
        else // 조준해제
        {

        }
    }

}
